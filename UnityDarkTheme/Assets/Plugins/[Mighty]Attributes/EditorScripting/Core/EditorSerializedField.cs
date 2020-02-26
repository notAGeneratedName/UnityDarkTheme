#if UNITY_EDITOR
using System.Reflection;
using System;
using System.IO;
using System.Text;
using UnityEditor;

namespace MightyAttributes.Editor
{
    public class EditorSerializedField
    {
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();
        private string m_path;
        private string m_fileName;

        public EditorSerializedField(string fileName)
        {
            m_fileName = fileName;
            m_path = EditorSerializedFieldUtility.CreateEditorFieldPath(m_fileName);
        }

        public void Serialize(object value, Type type)
        {
            var wrapper = type.GetWrapperForType();
            if (wrapper != null)
            {
//                if (type.IsEnum)
//                    value = Enum.ToObject(type, value);
                Serialize(m_path, m_fileName, wrapper, value);
                return;
            }

            if (type.GetCustomAttribute(typeof(SerializableAttribute), true) == null) return;

            wrapper = typeof(bool).GetWrapperForType();
            wrapper.SetValue(EditorDrawUtility.GetFoldout(m_fileName));

            WriteFile(m_path, wrapper);

            foreach (var field in type.GetSerializableFields())
                EditorFieldsDatabase.GetEditorField($"{m_fileName}.{field.Name}").Serialize(field.GetValue(value), field.FieldType);
        }

        public bool DeserializeOverwrite(Type type, out object value)
        {
            var wrapper = type.GetWrapperForType();

            if (wrapper != null) return Deserialize(m_path, m_fileName, wrapper, out value);

            value = null;

            if (type.GetCustomAttribute(typeof(SerializableAttribute), true) == null) return false;

            if (!Exists(m_path)) return false;

            wrapper = typeof(bool).GetWrapperForType();
            ReadFile(m_path, wrapper);
            wrapper.GetValue(out var foldout);
            EditorDrawUtility.SetFoldout(m_fileName, (bool) foldout);

            value = Activator.CreateInstance(type);
            foreach (var field in type.GetSerializableFields())
            {
                EditorFieldsDatabase.GetEditorField($"{m_fileName}.{field.Name}").DeserializeOverwrite(field.FieldType, out var fieldValue);
                if (field.FieldType.IsEnum)
                    fieldValue = Enum.ToObject(field.FieldType, fieldValue);
                field.SetValue(value, fieldValue);
            }

            return true;
        }

        private void Serialize(string path, string fileName, BaseEditorFieldWrapper wrapper, object value)
        {
            wrapper.SetValue(value);
            if (!(wrapper is BaseArrayFieldWrapper arrayWrapper))
            {
                WriteFile(path, wrapper);
                return;
            }

            arrayWrapper.foldout = EditorDrawUtility.GetFoldout(fileName);
            WriteFile(path, arrayWrapper);
        }

        private bool Deserialize(string path, string fileName, BaseEditorFieldWrapper wrapper, out object value)
        {
            value = null;
            if (!Exists(path)) return false;

            wrapper.ResetValue();
            ReadFile(path, wrapper);

            if (wrapper is BaseArrayFieldWrapper arrayWrapper)
                EditorDrawUtility.SetFoldout(fileName, arrayWrapper.foldout);

            wrapper.GetValue(out value);
            return value != null;
        }

        public void Rename(string newFileName)
        {
            var newPath = EditorSerializedFieldUtility.CreateEditorFieldPath(newFileName);
            Delete(newPath);

            File.Move($"{m_path}.json", $"{newPath}.json");
            m_fileName = newFileName;
            m_path = newPath;
        }

        public bool Exists(string path) => File.Exists($"{path}.json");

        public void Delete() => Delete(m_path);

        private void Delete(string path)
        {
            if (Exists(path)) File.Delete(path);
        }

        private static void WriteFile(string filePath, BaseEditorFieldWrapper wrapper)
        {
            var json = EditorJsonUtility.ToJson(wrapper);
            var bytes = new UTF8Encoding().GetBytes(json);
            try
            {
                using (var stream = File.Open($"{filePath}.json", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    var length = bytes.Length;
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Write(bytes, 0, length);
                    stream.SetLength(length);
                }
            }
            catch
            {
                // ignored
            }
        }

        private static void ReadFile(string filePath, BaseEditorFieldWrapper wrapper)
        {
            try
            {
                byte[] bytes;
                using (var stream = File.OpenRead($"{filePath}.json"))
                {
                    var length = (int) stream.Length;
                    bytes = new byte[length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(bytes, 0, length);
                }

                EditorJsonUtility.FromJsonOverwrite(new UTF8Encoding().GetString(bytes), wrapper);
            }
            catch
            {
                // ignored
            }
        }
    }
}
#endif