#if UNITY_EDITOR
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public static class FileUtility
    {
        private const string META_ICON_REGEX = @"\sicon: {fileID: \d+, guid: (.+), type: \d+}";

        public static string GetIconGUIDFromScriptType(this Type type) => GetIconGUIDFromMetaFile(type.GetAbsoluteMetaPathByType());

        public static string GetIconGUIDFromMetaFile(string metaFilePath)
        {
            foreach (var line in File.ReadAllLines(metaFilePath))
            {
                var match = Regex.Match(line, META_ICON_REGEX);
                if (match.Success && match.Groups.Count == 2)
                    return match.Groups[1].Value;
            }

            return null;
        }

        public static string GetRelativeMetaPathByType(this Type type) => $"{GetRelativeScriptPathByType(type)}.meta";

        public static string GetAbsoluteMetaPathByType(this Type type) => $"{GetAbsoluteScriptPathByType(type)}.meta";

        public static string GetRelativeScriptPathByType(this Type type, bool pathWithName = true)
        {
            var typeName = type.Name;
            var completePath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(typeName)[0]);

            return pathWithName ? completePath.Replace("/", "\\") : completePath.Replace($"{typeName}.cs", string.Empty).Replace("/", "\\");
        }

        public static string GetAbsoluteScriptPathByType(this Type type, bool pathWithName = true)
        {
            var typeName = type.Name;
            var completePath = $@"{Application.dataPath.Replace("Assets", string.Empty)}{
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(typeName)[0])}";

            return pathWithName ? completePath.Replace("/", "\\") : completePath.Replace($"{typeName}.cs", string.Empty).Replace("/", "\\");
        }
    }
}
#endif