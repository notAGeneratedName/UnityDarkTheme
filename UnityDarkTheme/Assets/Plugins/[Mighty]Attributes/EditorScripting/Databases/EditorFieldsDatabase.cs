#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    [InitializeOnLoad]
    public static class EditorFieldsDatabase
    {
        private static readonly Dictionary<string, EditorSerializedField> EditorFieldsByFileName;

        static EditorFieldsDatabase()
        {
            var mainAssembly = TypeUtility.GetMainAssembly();
            if (mainAssembly == null) return;

            var names = new List<string>();

            foreach (var type in mainAssembly.GetTypes())
                PopulateFieldNames(type, names,
                    ReflectionUtility.GetAllFields(type, f => f.GetCustomAttribute(typeof(EditorSerializeAttribute)) != null));

            if (names.Count == 0) return;

            EditorFieldsByFileName = new Dictionary<string, EditorSerializedField>();

            if (!Directory.Exists(EditorSerializedFieldUtility.DirectoryPath))
                Directory.CreateDirectory(EditorSerializedFieldUtility.DirectoryPath);

            foreach (var filePath in Directory.GetFiles(EditorSerializedFieldUtility.DirectoryPath))
            {
                if (Path.GetExtension(filePath) == ".meta") continue;
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!Regex.IsMatch(fileName, EditorSerializedFieldUtility.FILE_NAME_REGEX) ||
                    EditorFieldsByFileName.ContainsKey(fileName)) continue;

                EditorFieldsByFileName[fileName] = new EditorSerializedField(fileName);
            }

            foreach (var key in EditorFieldsByFileName.Keys
                .Where(k => !names.Contains(EditorSerializedFieldUtility.GetTypeAndFieldNameFromFileName(k))).ToArray())
            {
                EditorFieldsByFileName[key].Delete();
                EditorFieldsByFileName.Remove(key);
            }
        }

        private static void PopulateFieldNames(Type targetType, List<string> names, IEnumerable<FieldInfo> fields,
            string namePrefix = "")
        {
            foreach (var field in fields)
            {
                names.Add(EditorSerializedFieldUtility.CreateTypeAndFieldName(targetType, $"{namePrefix}{field.Name}"));
                if (field.FieldType.IsSerializableClass())
                    PopulateFieldNames(targetType, names, field.FieldType.GetSerializableFields(), $"{namePrefix}{field.Name}.");

                if (!(field.GetCustomAttribute(typeof(EditorSerializeAttribute)) is EditorSerializeAttribute attribute)) continue;

                if (attribute.OldName == null || attribute.OldName == field.Name) continue;

                names.Add(EditorSerializedFieldUtility.CreateTypeAndFieldName(targetType, $"{namePrefix}{attribute.OldName}"));
                if (field.FieldType.IsSerializableClass())
                    PopulateFieldNames(targetType, names, field.FieldType.GetSerializableFields(), $"{namePrefix}{attribute.OldName}.");
            }
        }

        public static void RenameField(Object context, string oldFieldName, string newFieldName)
        {
            var oldFileName = EditorSerializedFieldUtility.CreatePathName(context, oldFieldName);
            if (!EditorFieldsByFileName.ContainsKey(oldFileName)) return;

            foreach (var key in EditorFieldsByFileName.Keys.Where(k => k.Contains(oldFileName)).ToArray())
            {
                var newFileName = EditorSerializedFieldUtility.CreatePathName(context, newFieldName);
                var fixedFileName = key.Replace(oldFileName, newFileName);

                var editorField = EditorFieldsByFileName[key];

                editorField.Rename(fixedFileName);
                EditorFieldsByFileName.Remove(key);
                EditorFieldsByFileName[fixedFileName] = editorField;
            }
        }

        public static EditorSerializedField GetEditorField(Object context, string fieldName) =>
            GetEditorField(EditorSerializedFieldUtility.CreatePathName(context, fieldName));

        public static EditorSerializedField GetEditorField(string fileName)
        {
            if (!EditorFieldsByFileName.ContainsKey(fileName)) EditorFieldsByFileName[fileName] = new EditorSerializedField(fileName);
            return EditorFieldsByFileName[fileName];
        }
    }
}
#endif