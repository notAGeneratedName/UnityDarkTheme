#if UNITY_EDITOR
using System.Linq;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [Serializable]
    public class ScriptIcon
    {
        public Texture2D icon;
        public int priority;

        public bool ignored;

        [SerializeField]  private string _assemblyName;
        [SerializeField] private string _typeName;
        [SerializeField] private string _metaIconGUID;

        private bool m_valid;

        public bool IsValid => m_valid;
        public void SetValid() => m_valid = true;

        private Type m_type;

        public void SetAssembly(Assembly assembly) => _assemblyName = assembly.GetName().Name;

        public string GetAssemblyName() => _assemblyName;
        
        public void SetType(Type type)
        {
            m_type = type;
            
            _typeName = type.FullName;
            _metaIconGUID = type.GetIconGUIDFromScriptType();
        }
        
        public string GetTypeName() => _typeName;

        public string GetScriptPath() => m_type.GetRelativeScriptPathByType();

        public bool CompareIcons() => _metaIconGUID != null &&
                                      AssetDatabase.TryGetGUIDAndLocalFileIdentifier(icon, out var iconGUID, out long _) &&
                                      _metaIconGUID == iconGUID;
    }

    [Serializable]
    public class ScriptIconsDatabase
    {
        public ScriptIcon[] scriptIcons;
    }

    [DrawerTarget(typeof(ScriptIconAttribute))]
    public class ScriptIconsDrawer : BaseReloadScriptDrawer
    {
        private static readonly string DatabaseDirectoryPath =
            Path.Combine(typeof(ScriptIconsDrawer).GetAbsoluteScriptPathByType(false), "ScriptIcons");

        private static readonly string DatabaseFilePath = Path.Combine(DatabaseDirectoryPath, "ScriptIconsDatabase.json");

        private readonly MethodInfo SetIconForObject =
            typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);

        private readonly MethodInfo CopyMonoScriptIconToImporters =
            typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.Static | BindingFlags.NonPublic);
        
        public override void OnReloadScript()
        {
            if (!Directory.Exists(DatabaseDirectoryPath))
                Directory.CreateDirectory(DatabaseDirectoryPath);

            if (!File.Exists(DatabaseFilePath))
                File.Create(DatabaseFilePath);

            var database = ReadDatabase();
            
            var mainAssembly = TypeUtility.GetMainAssembly();
            var pluginsAssembly = TypeUtility.GetPluginsAssembly();

            var scriptIconByType = ExtractDatabase(database);

            PopulateScriptIconsFromAssembly(mainAssembly, scriptIconByType);
            PopulateScriptIconsFromAssembly(pluginsAssembly, scriptIconByType);

            RemoveInvalidScriptIcons(scriptIconByType);

            database.scriptIcons = ApplyIcons(scriptIconByType.Values.ToArray());
            WriteDatabase(database);
        }

        private ScriptIconsDatabase ReadDatabase()
        {
            var database = new ScriptIconsDatabase();
            EditorJsonUtility.FromJsonOverwrite(File.ReadAllText(DatabaseFilePath), database);

            return database;
        }

        private Dictionary<Type, ScriptIcon> ExtractDatabase(ScriptIconsDatabase database)
        {
            var scriptIconByType = new Dictionary<Type, ScriptIcon>();
            if (database.scriptIcons == null) return scriptIconByType;

            foreach (var icon in database.scriptIcons)
            {
                if (!TypeUtility.GetTypeInAssembly(icon.GetTypeName(), icon.GetAssemblyName(), out var type)) continue;

                icon.SetType(type);
                scriptIconByType[type] = icon;
            }

            return scriptIconByType;
        }

        private void PopulateScriptIconsFromAssembly(Assembly assembly, Dictionary<Type, ScriptIcon> scriptIconByType)
        {
            if (assembly == null) return;
            
            foreach (var type in assembly.GetTypes())
                if (!type.IsAbstract && type.IsSubclassOf(typeof(MonoBehaviour)))
                    PopulateScriptIconFromType(assembly, type, scriptIconByType);
        }

        private void PopulateScriptIconFromType(Assembly assembly,Type type, Dictionary<Type, ScriptIcon> scriptIconByType)
        {
            foreach (ScriptIconAttribute attribute in type.GetCustomAttributes(typeof(ScriptIconAttribute), true))
                PopulateScriptIconFromType(assembly, type, attribute.IconPath, attribute.Priority, scriptIconByType);

            foreach (IconAttribute attribute in type.GetCustomAttributes(typeof(IconAttribute), true))
                PopulateScriptIconFromType(assembly, type, attribute.IconPath, attribute.Priority, scriptIconByType);
        }

        private void PopulateScriptIconFromType(Assembly assembly,Type type, string iconPath, int priority,
            Dictionary<Type, ScriptIcon> scriptIconByType)
        {
            var ignored = type.GetCustomAttribute(typeof(IgnoreScriptIconAttribute), true) != null;

            if (scriptIconByType.TryGetValue(type, out var scriptIcon) && scriptIcon.priority <= priority)
            {
                scriptIcon.ignored = ignored;
                scriptIcon.SetValid();
                return;
            }

            if (scriptIcon == null) scriptIcon = new ScriptIcon();

            scriptIcon.SetAssembly(assembly);
            scriptIcon.SetType(type);

            scriptIcon.icon = EditorDrawUtility.GetTexture(iconPath);
            scriptIcon.priority = priority;

            scriptIcon.ignored = ignored;
            scriptIcon.SetValid();

            scriptIconByType[type] = scriptIcon;
        }

        private void RemoveInvalidScriptIcons(Dictionary<Type, ScriptIcon> scriptIconByType)
        {
            foreach (var pair in scriptIconByType.ToArray())
                if (!pair.Value.IsValid)
                    scriptIconByType.Remove(pair.Key);
        }

        private ScriptIcon[] ApplyIcons(ScriptIcon[] icons)
        {
            foreach (var scriptIcon in icons)
            {
                if (scriptIcon.ignored) continue;

                if (scriptIcon.CompareIcons()) continue;

                var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptIcon.GetScriptPath());

                SetIconForObject.Invoke(null, new object[] {monoScript, scriptIcon.icon});
                CopyMonoScriptIconToImporters.Invoke(null, new object[] {monoScript});
            }

            return icons;
        }

        private void WriteDatabase(ScriptIconsDatabase database) =>
            File.WriteAllText(DatabaseFilePath, EditorJsonUtility.ToJson(database, true));
    }
}
#endif