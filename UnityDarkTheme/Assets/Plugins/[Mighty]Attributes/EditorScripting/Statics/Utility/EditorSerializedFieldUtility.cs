#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public static class EditorSerializedFieldUtility
    {
        public static readonly string DirectoryPath = Path.Combine(Application.dataPath, "EditorSerializedFields").Replace("\\", "/");

        public const string FILE_NAME_REGEX = @"^[^-]+\-([^-]+\-[^-]+)";

        public static string CreatePathName(Object context, string fieldName) => context == null ? fieldName :
            $"{context.name}-{context.GetType().FullName}-{fieldName}";

        public static string CreateTypeAndFieldName(Type type, string fieldName) => $"{type.FullName}-{fieldName}";

        public static string GetTypeAndFieldNameFromFileName(string fileName)
        {
            var match = Regex.Match(fileName, FILE_NAME_REGEX);
            if (match.Success && match.Groups.Count == 2) return match.Groups[1].Value;
            return null;
        }

        public static string CreateEditorFieldPath(string fileName) => Path.Combine(DirectoryPath, $"{fileName}").Replace("\\", "/");

        public static void GetEditorFieldValue(string fieldName, Object context, out object value, string oldName = null) => 
            GetEditorFieldValue(fieldName, context, context, out value, oldName);

        public static void GetEditorFieldValue(string fieldName, Object context, object target, out object value, string oldName = null)
        {
            var field = target.GetField(fieldName);
            if (field != null) field.GetEditorFieldValue(context, target, out value, oldName);
            else value = null;
        }
        
        public static void GetEditorFieldValue(this FieldInfo field, Object context, object target, out object value, string oldName = null)
        {
            if (oldName != null)
                EditorFieldsDatabase.RenameField(context, oldName, field.Name);

            var editorField = EditorFieldsDatabase.GetEditorField(context, field.Name);
            editorField.DeserializeOverwrite(field.FieldType, out value);
        }
        
        public static void ApplyEditorFieldChanges(string fieldName, Object context, string oldName = null) =>
            ApplyEditorFieldChanges(fieldName, context, context, oldName);
        
        public static void ApplyEditorFieldChanges(string fieldName, Object context, object target, string oldName = null)
        {
            var field = target.GetField(fieldName);
            if (field != null) field.ApplyEditorFieldChanges(context, target, oldName);
        }

        public static void ApplyEditorFieldChanges(this FieldInfo field, Object context, object target, string oldName = null)
        {
            if (oldName != null)
                EditorFieldsDatabase.RenameField(context, oldName, field.Name);

            var editorField = EditorFieldsDatabase.GetEditorField(context, field.Name);
            editorField.Serialize(field.GetValue(target), field.FieldType);
        }

        public static BaseEditorFieldWrapper GetWrapperForType(this Type type) =>
            type.IsArray
                ? EditorFieldWrappersDatabase.GetWrapperForType(type.GetElementType(), true)
                : EditorFieldWrappersDatabase.GetWrapperForType(type, false);

        public static IEnumerable<FieldInfo> GetSerializableFields(this Type type, bool ignoreHidden = false,
            bool ignoreEditorSerialize = false) => type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute(typeof(NonSerializedAttribute)) == null &&
                        (!ignoreHidden || f.GetCustomAttribute(typeof(HideAttribute)) == null) &&
                        (f.IsPublic || f.GetCustomAttribute(typeof(SerializeField)) != null ||
                         (!ignoreEditorSerialize && f.GetCustomAttribute(typeof(EditorSerializeAttribute)) != null)));

        public static bool IsSerializableClass(this Type type)
        {
            if (type == typeof(bool)) return false;
            if (type.IsEnum || type == typeof(byte) || type == typeof(short) || type == typeof(int)) return false;
            if (type == typeof(long)) return false;
            if (type == typeof(float)) return false;
            if (type == typeof(double)) return false;
            if (type == typeof(char)) return false;
            if (type == typeof(string)) return false;
            if (type == typeof(Vector2)) return false;
            if (type == typeof(Vector3)) return false;
            if (type == typeof(Vector4)) return false;
            if (type == typeof(Rect)) return false;
            if (type == typeof(Bounds)) return false;
            if (type == typeof(Vector2Int)) return false;
            if (type == typeof(Vector3Int)) return false;
            if (type == typeof(RectInt)) return false;
            if (type == typeof(BoundsInt)) return false;
            if (type == typeof(Quaternion)) return false;
            if (type == typeof(LayerMask)) return false;
            if (type == typeof(Color)) return false;
            if (type == typeof(AnimationCurve)) return false;
            if (type == typeof(Gradient)) return false;
            if (typeof(Object).IsAssignableFrom(type)) return false;

            return type.GetCustomAttribute(typeof(SerializableAttribute), true) != null;
        }
    }
}
#endif