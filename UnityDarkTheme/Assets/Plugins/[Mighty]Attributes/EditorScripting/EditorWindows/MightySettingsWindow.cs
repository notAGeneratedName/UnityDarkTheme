#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public class MightySettingsWindow : EditorWindow
    {
        private static readonly DarkBoxGroupPropertyGrouper DarkBox =
            DrawersDatabase.GetDrawerForAttribute<DarkBoxGroupPropertyGrouper>(typeof(DarkBoxGroupAttribute));

        [MenuItem("Window/[Mighty]Attributes/Settings", false, 2025)]
        private static void Init() => GetWindow<MightySettingsWindow>().Show();

        private void OnGUI()
        {
            titleContent = new GUIContent(EditorDrawUtility.DrawIcon(IconName.SETTINGS))
            {
                text = " Mighty Settings"
            };

            minSize = new Vector2(350, 235);

            GUILayout.Space(10);
            MightySettingsServices.Activated = EditorGUILayout.Toggle("Activated", MightySettingsServices.Activated);

            GUILayout.Space(10);
            DarkBox.BeginDrawGroup();
            MightySettingsServices.AutoValuesOnPlay =
                EditorGUILayout.Toggle("Auto Values On Play", MightySettingsServices.AutoValuesOnPlay);

            GUILayout.Space(5);
            MightySettingsServices.AutoValuesOnPlay =
                EditorGUILayout.Toggle("Auto Values On Build", MightySettingsServices.AutoValuesOnBuild);

            GUILayout.Space(5);
            if (GUILayout.Button("Apply Auto Values")) MightyAutoValues.ApplyAutoValuesAsync();
            DarkBox.EndDrawGroup();

            GUILayout.Space(10);
            DarkBox.BeginDrawGroup();
            MightySettingsServices.MainAssemblyName =
                EditorGUILayout.TextField("Main Assembly Name", MightySettingsServices.MainAssemblyName);
            
            GUILayout.Space(5);
            MightySettingsServices.PluginsAssemblyName =
                EditorGUILayout.TextField("Plugins Assembly Name", MightySettingsServices.PluginsAssemblyName);
            
            GUILayout.Space(5);
            if (GUILayout.Button("Default Name"))
            {
                MightySettingsServices.MainAssemblyName = MightySettingsServices.DEFAULT_MAIN_ASSEMBLY_NAME;
                MightySettingsServices.PluginsAssemblyName = MightySettingsServices.DEFAULT_PLUGINS_ASSEMBLY_NAME;
            }

            DarkBox.EndDrawGroup();

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Script Reload")) MightyReloadScript.OnReloadScript();
        }
    }
}
#endif