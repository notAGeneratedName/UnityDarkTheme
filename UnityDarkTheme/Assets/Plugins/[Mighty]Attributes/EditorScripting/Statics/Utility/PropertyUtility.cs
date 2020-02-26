#if UNITY_EDITOR
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public static class PropertyUtility
    {
        private const string ARRAY_PROPERTY_REGEX = @"^data\[\d+\]$|^Array$";
        private const string ARRAY_ITEM_REGEX = @"^data\[(\d+)\]$";

        public static bool ArrayPropertyPathItem(this string item) => Regex.IsMatch(item, ARRAY_PROPERTY_REGEX);

        public static bool GetPropertyArrayIndex(this string item, out int index)
        {
            index = 0;
            var match = Regex.Match(item, ARRAY_ITEM_REGEX);
            if (!match.Success || match.Groups.Count != 2) return false;
            index = Convert.ToInt32(match.Groups[1].Value);
            return true;
        }

        public static T GetAttribute<T>(this SerializedProperty property) where T : Attribute =>
            GetAttribute(property, typeof(T)) as T;

        public static T[] GetAttributes<T>(this SerializedProperty property) where T : Attribute =>
            GetAttributes(property, typeof(T)).Cast<T>().ToArray();

        public static object GetAttribute(this SerializedProperty property, Type attributeType)
        {
            object[] attributes = GetAttributes(property, attributeType);
            return attributes != null && attributes.Length > 0 ? attributes[0] : null;
        }

        public static object[] GetAttributes(this SerializedProperty property, Type attributeType)
        {
            var fieldInfo = ReflectionUtility.GetField(property.GetSerializableClassProperty(out var name)?.GetSystemType() ??
                                                       property.GetTargetObject().GetType(), name);

            if (fieldInfo == null) return new object[0];

            var attributesList = new List<object>();
            var attributes = fieldInfo.GetCustomAttributes(attributeType, true);
            if (attributes.Length > 0) attributesList.AddRange(attributes);

            var customWrapperDrawer = new MightyWrapperDrawer();

            attributesList.AddRange(MightyDrawer.GetAttributesFromCustomWrappers(attributeType, fieldInfo, customWrapperDrawer));

            return attributesList.ToArray();
        }

        public static bool IsPropertyFromSerializableClass(this SerializedProperty property, out string className, out string propertyName)
        {
            var splitPath = property.propertyPath.Split('.').ToList();
            while (splitPath.Last().ArrayPropertyPathItem()) splitPath.RemoveAt(splitPath.Count - 1);

            propertyName = splitPath.Last();

            if (splitPath.Count <= 1)
            {
                className = null;
                return false;
            }

            splitPath.RemoveAt(splitPath.Count - 1);
            while (splitPath.Last().ArrayPropertyPathItem()) splitPath.RemoveAt(splitPath.Count - 1);
            className = string.Join(".", splitPath);
            return true;
        }

        public static SerializedProperty GetSerializableClassProperty(this SerializedProperty property, out string propertyName) =>
            IsPropertyFromSerializableClass(property, out var className, out propertyName)
                ? property.serializedObject.FindProperty(className)
                : null;

        public static Object GetTargetObject(this SerializedProperty property) => property.serializedObject.targetObject;

        public static object GetAttributeTarget<T>(this SerializedProperty property) where T : Attribute
        {
            foreach (var attribute in property.GetAttributes<MightyWrapperAttribute>())
            {
                var wrappingObject = attribute.GetWrappingObject<T>();
                if (wrappingObject != null) return wrappingObject;
            }

            return property.GetPropertyTargetReference();
        }

        private static object GetWrappingObject<T>(this object target) =>
            target.GetType().GetCustomAttribute(typeof(T), true) == null
                ? target.GetType().GetCustomAttribute<MightyWrapperAttribute>()?.GetWrappingObject<T>()
                : target;

        public static object GetWrappedObject(this SerializedProperty property, Type wrapperType)
        {
            if (property.GetAttribute(wrapperType) != null) return property.GetPropertyTargetReference();

            foreach (var attribute in property.GetAttributes<MightyWrapperAttribute>())
            {
                var wrappedObject = attribute.GetWrappedObject(wrapperType);
                if (wrappedObject != null) return wrappedObject;
            }

            return property.GetPropertyTargetReference();
        }

        public static object GetWrappedObject(this object target, Type wrapperType) =>
            target.GetType().GetCustomAttribute(wrapperType, true) == null
                ? target.GetType().GetCustomAttribute<MightyWrapperAttribute>()?.GetWrappedObject(wrapperType)
                : target;
    }
}
#endif