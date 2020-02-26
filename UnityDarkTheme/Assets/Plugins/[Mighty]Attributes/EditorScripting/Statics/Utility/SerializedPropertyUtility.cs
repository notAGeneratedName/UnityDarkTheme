#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public static class SerializedPropertyUtility
    {
        #region Getters

        public static GameObject[] GetRootGameObjects()
        {
            var activeScene = SceneManager.GetActiveScene();
            return activeScene.isLoaded ? activeScene.GetRootGameObjects() : new GameObject[0];
        }

        public static object GetSerializableClassReference(this SerializedProperty property)
        {
            var targetObject = property.GetPropertyTargetReference();
            var targetType = targetObject.GetType();

            var path = property.propertyPath;
            var splitPath = path.Split('.').ToList();
            if (!splitPath.Last().ArrayPropertyPathItem())
            {
                var propertyField = ReflectionUtility.GetField(targetType, splitPath.Last());
                return propertyField.GetValue(targetObject);
            }
            else
            {
                var propertyField = ReflectionUtility.GetField(targetType, splitPath[splitPath.Count - 3]);
                targetObject = propertyField.GetValue(targetObject);
                splitPath.Last().GetPropertyArrayIndex(out var index);
                return ((IList) targetObject)[index];
            }
        }

        public static object GetPropertyTargetReference(this SerializedProperty property)
        {
            object targetObject = property.GetTargetObject();
            var targetType = targetObject.GetType();
            while (targetType != null)
            {
                var path = property.propertyPath;
                var splitPath = path.Split('.').ToList();
                while (splitPath.Last().ArrayPropertyPathItem()) splitPath.RemoveAt(splitPath.Count - 1);
                path = string.Join(".", splitPath);

                var fieldInfo = ReflectionUtility.GetField(targetType, path);

                if (fieldInfo != null) return targetObject;
                if (splitPath.Count == 1)
                {
                    targetType = targetType.BaseType;
                    continue;
                }

                for (var i = 0; i < splitPath.Count; i++)
                {
                    var typeField = ReflectionUtility.GetField(targetType, splitPath[i]);
                    if (typeField == null) break;
                    targetType = typeField.FieldType;
                    targetObject = typeField.GetValue(targetObject);
                    var newPath = new List<string>(splitPath);

                    if (typeof(IList).IsAssignableFrom(targetType))
                    {
                        if (targetType.IsArray)
                            targetType = targetType.GetElementType();
                        else if (targetType.IsGenericType)
                            targetType = targetType.GetGenericArguments()[0];
                        
                        if (!newPath[i + 2].GetPropertyArrayIndex(out var index)) continue;

                        targetObject = ((IList) targetObject)[index];
                        if (newPath.Count > i + 3) i += 2;
                    }

                    newPath.RemoveRange(0, i + 1);
                    path = string.Join(".", newPath);

                    fieldInfo = ReflectionUtility.GetField(targetType, path);
                    if (fieldInfo != null) return targetObject;
                }

                break;
            }

            return targetObject;
        }

        public static SerializedPropertyType GetPropertyType(this SerializedProperty property)
        {
            if (!property.isArray || property.propertyType == SerializedPropertyType.String) return property.propertyType;

            var type = property.GetSystemType();

            if (type == typeof(bool)) return SerializedPropertyType.Boolean;
            if (type.IsEnum) return SerializedPropertyType.Enum;
            if (type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long))
                return SerializedPropertyType.Integer;
            if (type == typeof(float) || type == typeof(double)) return SerializedPropertyType.Float;

            if (type == typeof(char)) return SerializedPropertyType.Character;
            if (type == typeof(string)) return SerializedPropertyType.String;

            if (type == typeof(Vector2)) return SerializedPropertyType.Vector2;
            if (type == typeof(Vector3)) return SerializedPropertyType.Vector3;
            if (type == typeof(Vector4)) return SerializedPropertyType.Vector4;

            if (type == typeof(Rect)) return SerializedPropertyType.Rect;
            if (type == typeof(Bounds)) return SerializedPropertyType.Bounds;

            if (type == typeof(Vector2Int)) return SerializedPropertyType.Vector2Int;
            if (type == typeof(Vector3Int)) return SerializedPropertyType.Vector3Int;
            if (type == typeof(RectInt)) return SerializedPropertyType.RectInt;
            if (type == typeof(BoundsInt)) return SerializedPropertyType.BoundsInt;

            if (type == typeof(Quaternion)) return SerializedPropertyType.Quaternion;
            if (type == typeof(LayerMask)) return SerializedPropertyType.LayerMask;
            if (type == typeof(Color)) return SerializedPropertyType.Color;

            if (type == typeof(Gradient)) return SerializedPropertyType.Gradient;
            if (type == typeof(AnimationCurve)) return SerializedPropertyType.AnimationCurve;

            return typeof(Object).IsAssignableFrom(type) ? SerializedPropertyType.ObjectReference : SerializedPropertyType.Generic;
        }

        public static Type GetSystemType(this SerializedProperty property)
        {
            var targetType = property.GetTargetObject().GetType();
            FieldInfo fieldInfo = null;
            while (targetType != null)
            {
                var path = property.propertyPath;
                var splitPath = path.Split('.').ToList();
                while (splitPath.Last().ArrayPropertyPathItem()) splitPath.RemoveAt(splitPath.Count - 1);
                path = string.Join(".", splitPath);

                fieldInfo = ReflectionUtility.GetField(targetType, path);

                if (fieldInfo != null) break;
                if (splitPath.Count == 1)
                {
                    targetType = targetType.BaseType;
                    continue;
                }

                for (var i = 0; i < splitPath.Count; i++)
                {
                    var typeField = ReflectionUtility.GetField(targetType, splitPath[i]);
                    if (typeField == null) break;
                    targetType = typeField.FieldType;
                    var newPath = new List<string>(splitPath);

                    if (typeof(IList).IsAssignableFrom(targetType))
                    {
                        if (targetType.IsArray)
                            targetType = targetType.GetElementType();
                        else if (targetType.IsGenericType)
                            targetType = targetType.GetGenericArguments()[0];

                        if (newPath.Count > i + 3) i += 2;
                        else return targetType;
                    }

                    newPath.RemoveRange(0, i + 1);
                    path = string.Join(".", newPath);

                    fieldInfo = ReflectionUtility.GetField(targetType, path);
                    if (fieldInfo != null) break;
                }

                break;
            }

            if (fieldInfo == null) return null;

            if (fieldInfo.FieldType.IsArray)
                return fieldInfo.FieldType.GetElementType();
            if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType) && fieldInfo.FieldType.IsGenericType)
                return fieldInfo.FieldType.GetGenericArguments()[0];
            return fieldInfo.FieldType;
        }

        public static GameObject GetGameObject(this SerializedProperty property) =>
            ((MonoBehaviour) property.GetTargetObject()).gameObject;

        #endregion

        #region FindObject

        public static T FindFirstObject<T>(bool includeInactive = true) where T : Component
        {
            T t = default;
            return GetRootGameObjects().Any(o => (t = o.GetComponentsInChildren<T>(includeInactive).FirstOrDefault()) != null)
                ? t
                : null;
        }

        public static T[] FindAllObjects<T>(bool includeInactive = true) where T : Component
        {
            var list = new List<T>();
            foreach (var o in GetRootGameObjects()) list.AddRange(o.GetComponentsInChildren<T>(includeInactive));
            return list.ToArray();
        }

        public static Object FindObject(this Type type, string str, bool includeInactive,
            Func<GameObject, Type, bool, string, bool> comparer)
        {
            return GetRootGameObjects().SelectMany(o => o.GetComponentsInChildren(type, includeInactive))
                .FirstOrDefault(c => comparer(c.gameObject, type, includeInactive, str));
        }

        public static Object[] FindObjects(this Type type, string str, bool includeInactive,
            Func<GameObject, Type, bool, string, bool> comparer)
        {
            var list = new List<Object>();
            foreach (var o in GetRootGameObjects())
                list.AddRange(o.GetComponentsInChildren(type, includeInactive)
                    .Where(c => comparer(c.gameObject, type, includeInactive, str)));
            return list.ToArray();
        }

        public static Object FindObject(this SerializedProperty property, string str, bool includeInactive,
            Func<GameObject, Type, bool, string, bool> comparer) =>
            property.GetSystemType().FindObject(str, includeInactive, comparer);

        public static Object[] FindObjects(this SerializedProperty property, string str, bool includeInactive,
            Func<GameObject, Type, bool, string, bool> comparer) =>
            property.GetSystemType().FindObjects(str, includeInactive, comparer);

        public static Object FindObjectWithTag(this SerializedProperty property, string tag = null, bool includeInactive = false) =>
            property.FindObject(tag, includeInactive, CompareTypeAndTag);

        public static Object[] FindObjectsWithTag(this SerializedProperty property, string tag = null, bool includeInactive = false) =>
            property.FindObjects(tag, includeInactive, CompareTypeAndTag).ToArray();

        public static Object FindObjectWithName(this SerializedProperty property, string name = null, bool includeInactive = false) =>
            property.FindObject(name, includeInactive, CompareTypeAndName);

        public static Object[] FindObjectsWithName(this SerializedProperty property, string name = null, bool includeInactive = false) =>
            property.FindObjects(name, includeInactive, CompareTypeAndName).ToArray();

        public static Object FindObjectWithLayer(this SerializedProperty property, string layer = null, bool includeInactive = false) =>
            property.FindObject(layer, includeInactive, CompareTypeAndLayer);

        public static Object[] FindObjectsWithLayer(this SerializedProperty property, string layer = null, bool includeInactive = false) =>
            property.FindObjects(layer, includeInactive, CompareTypeAndLayer).ToArray();

        #endregion

        #region FindAsset

        public static Object FindAssetOfType(this Type type, string name = null, string[] folders = null)
        {
            return FindAssetsOfType(type, name, folders).FirstOrDefault();
        }

        public static Object[] FindAssetsOfType(this Type type, string name = null, string[] folders = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (folders != null)
                {
                    return AssetDatabase.FindAssets($"{name} t:{type}".Replace("UnityEngine.", ""), folders)
                        .Select(AssetDatabase.GUIDToAssetPath).Select((s, i) => AssetDatabase.LoadAssetAtPath(s, type))
                        .Where(asset => asset != null).ToArray();
                }

                return AssetDatabase.FindAssets($"{name} t:{type}".Replace("UnityEngine.", ""))
                    .Select(AssetDatabase.GUIDToAssetPath).Select((s, i) => AssetDatabase.LoadAssetAtPath(s, type))
                    .Where(asset => asset != null).ToArray();
            }

            if (folders != null)
            {
                return AssetDatabase.FindAssets($"t:{type}".Replace("UnityEngine.", ""), folders)
                    .Select(AssetDatabase.GUIDToAssetPath).Select((s, i) => AssetDatabase.LoadAssetAtPath(s, type))
                    .Where(asset => asset != null).ToArray();
            }

            return AssetDatabase.FindAssets($"t:{type}".Replace("UnityEngine.", "")).Select(AssetDatabase.GUIDToAssetPath)
                .Select((s, i) => AssetDatabase.LoadAssetAtPath(s, type)).Where(asset => asset != null).ToArray();
        }

        public static Object FindAssetWithName(this SerializedProperty property, string name) =>
            property.GetSystemType().FindAssetOfType(name);

        public static Object[] FindAssetsWithName(this SerializedProperty property, string name) =>
            property.GetSystemType().FindAssetsOfType(name);

        public static Object FindAssetInFolders(this SerializedProperty property, string name, string[] folders) =>
            property.GetSystemType().FindAssetOfType(name, folders);

        public static Object[] FindAssetsInFolders(this SerializedProperty property, string name, string[] folders) =>
            property.GetSystemType().FindAssetsOfType(name, folders);

        #endregion

        #region GetInChildren

        public static Component GetComponentInChildren(this GameObject gameObject, Type type, string str, bool includeInactive,
            Func<GameObject, string, bool> comparer) =>
            string.IsNullOrEmpty(str)
                ? gameObject.GetComponentInChildren(type, includeInactive)
                : gameObject.GetComponentsInChildren(type, includeInactive).FirstOrDefault(item => comparer(item.gameObject, str));

        public static IEnumerable<Component> GetComponentsInChildren(this GameObject gameObject, Type type, string str,
            bool includeInactive,
            Func<GameObject, string, bool> comparer) =>
            string.IsNullOrEmpty(str)
                ? gameObject.GetComponentsInChildren(type, includeInactive)
                : gameObject.GetComponentsInChildren(type, includeInactive).Where(item => comparer(item.gameObject, str));

        public static Component GetComponentInChildren(this SerializedProperty property, Type type, string str, bool includeInactive,
            Func<GameObject, string, bool> comparer) =>
            property.GetGameObject().GetComponentInChildren(property.GetSystemType(), str, includeInactive, comparer);

        public static IEnumerable<Component> GetComponentsInChildren(this SerializedProperty property, Type type, string str,
            bool includeInactive, Func<GameObject, string, bool> comparer) =>
            property.GetGameObject().GetComponentsInChildren(property.GetSystemType(), str, includeInactive, comparer);

        public static Component GetComponentInChildrenWithTag(this SerializedProperty property, string tag = null,
            bool includeInactive = false) =>
            property.GetComponentInChildren(property.GetSystemType(), tag, includeInactive, TagComparer);

        public static IEnumerable<Component> GetComponentsInChildrenWithTag(this SerializedProperty property, string tag = null,
            bool includeInactive = false) =>
            property.GetComponentsInChildren(property.GetSystemType(), tag, includeInactive, TagComparer);

        public static Component GetComponentInChildrenWithName(this SerializedProperty property, string name = null,
            bool includeInactive = false) =>
            property.GetComponentInChildren(property.GetSystemType(), name, includeInactive, NameComparer);

        public static IEnumerable<Component> GetComponentsInChildrenWithName(this SerializedProperty property, string layer = null,
            bool includeInactive = false) =>
            property.GetComponentsInChildren(property.GetSystemType(), layer, includeInactive, NameComparer);

        public static Component GetComponentInChildrenWithLayer(this SerializedProperty property, string name = null,
            bool includeInactive = false) =>
            property.GetComponentInChildren(property.GetSystemType(), name, includeInactive, LayerComparer);

        public static IEnumerable<Component> GetComponentsInChildrenWithLayer(this SerializedProperty property, string layer = null,
            bool includeInactive = false) =>
            property.GetComponentsInChildren(property.GetSystemType(), layer, includeInactive, LayerComparer);

        #endregion

        #region Comparer

        public static bool CompareArrays(this SerializedProperty property, Object[] array)
        {
            if (!property.isArray) return false;
            if (property.arraySize != array.Length) return false;

            var propertyArray = new Object[property.arraySize];
            for (var i = 0; i < propertyArray.Length; i++)
                propertyArray[i] = property.GetArrayElementAtIndex(i).objectReferenceValue;

            return new HashSet<Object>(propertyArray).SetEquals(array);
        }

        public static bool CompareArrays(this SerializedProperty property, object[] array, object target)
        {
            if (!property.isArray) return false;
            if (property.arraySize != array.Length) return false;
            if (property.arraySize < 1) return true;
            var propertyType = property.GetArrayElementAtIndex(0).propertyType;
            var propertyArray = SetArrayGenericValue(property, propertyType, target);

            if (propertyArray == null) return false;
            switch (propertyType)
            {
                case SerializedPropertyType.Enum:
                {
                    var comparer = new EnumComparer<object>();
                    return new HashSet<object>(propertyArray, comparer).SetEquals(new HashSet<object>(array, comparer));
                }
                case SerializedPropertyType.Generic:
                {
                    var comparer = new GenericComparer<object>();
                    return new HashSet<object>(propertyArray, comparer).SetEquals(new HashSet<object>(array, comparer));
                }
                default:
                    return new HashSet<object>(propertyArray).SetEquals(array);
            }
        }

        public static bool CompareArrays(this SerializedProperty property, SerializedProperty other, object target)
        {
            if (!property.isArray || !other.isArray) return false;
            if (property.arraySize != other.arraySize) return false;
            if (property.propertyType != other.propertyType) return false;
            if (property.arraySize < 1) return true;

            var propertyType = property.GetArrayElementAtIndex(0).propertyType;
            var propertyArray = SetArrayGenericValue(property, propertyType, target);
            if (propertyArray == null) return false;
            var otherArray = SetArrayGenericValue(other, propertyType, target);
            if (otherArray == null) return false;

            return propertyType == SerializedPropertyType.Generic
                ? new HashSet<object>(propertyArray, new GenericComparer<object>()).SetEquals(
                    new HashSet<object>(propertyArray, new GenericComparer<object>()))
                : new HashSet<object>(propertyArray).SetEquals(otherArray);
        }

        private class GenericComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) => x == null && y == null || x != null && y != null && x.Equals(y);

            public int GetHashCode(T obj) => obj.GetHashCode();
        }

        private class EnumComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) => Convert.ToInt32(x) == Convert.ToInt32(y);

            public int GetHashCode(T obj) => obj.GetHashCode();
        }

        public static bool CompareTypeAndTag(this GameObject go, Type type, bool includeInactive, string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return go.GetComponentInChildren(type, includeInactive) != null;
            var t = go.GetComponentInChildren(type, includeInactive);
            return t != null && t.gameObject.TagComparer(tag);
        }

        public static bool CompareTypeAndName(this GameObject go, Type type, bool includeInactive, string name)
        {
            if (string.IsNullOrEmpty(name))
                return go.GetComponentInChildren(type, includeInactive) != null;
            var t = go.GetComponentInChildren(type, includeInactive);
            return t != null && t.gameObject.NameComparer(name);
        }

        public static bool CompareTypeAndLayer(this GameObject go, Type type, bool includeInactive, string layer)
        {
            if (string.IsNullOrEmpty(layer))
                return go.GetComponentInChildren(type, includeInactive) != null;
            var t = go.GetComponentInChildren(type, includeInactive);
            return t != null && t.gameObject.LayerComparer(layer);
        }

        public static bool TagComparer(this GameObject gameObject, string tag) => gameObject.CompareTag(tag);

        public static bool NameComparer(this GameObject gameObject, string name) => gameObject.name == name;

        public static bool LayerComparer(this GameObject gameObject, string layer) => gameObject.layer == LayerMask.NameToLayer(layer);

        #endregion

        #region FieldInfo

        public delegate bool GetValuePredicate<T>(string memberName, out T value);

        public static bool GetCallbackName(this object target, string memberName, out string callbackName)
        {
            var fieldInfo = ReflectionUtility.GetField(target.GetType(), memberName);
            if (fieldInfo.IsCallbackName())
            {
                callbackName = (string) fieldInfo.GetValue(target);
                return true;
            }

            var methodInfo = ReflectionUtility.GetMethod(target.GetType(), memberName);
            if (methodInfo.IsCallbackName())
            {
                callbackName = (string) methodInfo.Invoke(target, null);
                return true;
            }

            var propertyInfo = ReflectionUtility.GetProperty(target.GetType(), memberName);
            if (propertyInfo.IsCallbackName())
            {
                callbackName = (string) propertyInfo.GetValue(target);
                return true;
            }

            callbackName = null;
            return false;
        }

        public static object GetCallbackTarget(this object target, string callbackName, CallbackSignature callbackSignature,
            out MemberInfo outMember)
        {
            outMember = null;
            if (string.IsNullOrWhiteSpace(callbackName)) return null;
            if (target.GetType().GetMemberInfo(callbackName, callbackSignature, out outMember)) return target;

            if (target is MightyWrapperAttribute && GetCallbackName(target, callbackName, out var callbackNameValue))
                return target.GetWrappedObject(target.GetType()).GetCallbackTarget(callbackNameValue, callbackSignature, out outMember);

            return null;
        }

        public static object GetCallbackTarget(this SerializedProperty property, object target, string callbackName,
            CallbackSignature callbackSignature, out MemberInfo outMember)
        {
            outMember = null;
            if (string.IsNullOrWhiteSpace(callbackName)) return null;
            if (target.GetType().GetMemberInfo(callbackName, callbackSignature, out outMember)) return target;

            if (target is MightyWrapperAttribute && GetCallbackName(target, callbackName, out var callbackNameValue))
                return property.GetCallbackTarget(property.GetWrappedObject(target.GetType()), callbackNameValue, callbackSignature,
                    out outMember);

            return null;
        }

        public static bool GetBoolValue(this object target, string memberName, out bool value) =>
            target.GetValueFromMember(memberName, out value, bool.TryParse);

        public static bool GetBoolValue(this SerializedProperty property, object target, string memberName, out bool value) =>
            property.GetValueFromMember(target, memberName, out value, bool.TryParse);

        public static bool GetBoolInfo(this SerializedProperty property, object target, string memberName, out MightyInfo<bool> mightyInfo)
            => property.GetInfoFromMember(target, memberName, out mightyInfo, (string s, out bool b) => bool.TryParse(s, out b));

        public static bool GetBoolInfo(this object target, string memberName, out MightyInfo<bool> mightyInfo) =>
            target.GetInfoFromMember(memberName, out mightyInfo, (string s, out bool b) => bool.TryParse(s, out b));

        public static bool GetValueFromMember<T>(this object target, string memberName, out T outValue,
            GetValuePredicate<T> predicate = null)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                outValue = default;
                return false;
            }

            if (predicate != null && predicate(memberName, out outValue)) return true;

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return target.GetWrappedObject(target.GetType()).GetValueFromMember(callbackName, out outValue, predicate);

            return InternalGetValueFromMember(target, memberName, true, out outValue);
        }

        public static bool GetInfoFromMember<T>(this object target, string memberName, out MightyInfo<T> mightyInfo,
            GetValuePredicate<T> predicate = null)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                mightyInfo = null;
                return false;
            }

            if (predicate != null && predicate(memberName, out var outValue))
            {
                mightyInfo = new MightyInfo<T>(null, null, outValue);
                return true;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return target.GetWrappedObject(target.GetType()).GetInfoFromMember(callbackName, out mightyInfo, predicate);

            return InternalGetInfoFromMember(target, memberName, true, out mightyInfo);
        }

        public static bool GetValueFromMember<T>(this SerializedProperty property, object target, string memberName, out T outValue,
            GetValuePredicate<T> predicate = null)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                outValue = default;
                return false;
            }

            if (predicate != null && predicate(memberName, out outValue)) return true;

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return property.GetValueFromMember(property.GetWrappedObject(target.GetType()),
                    callbackName, out outValue, predicate);

            if (InternalGetValueFromMember(target, memberName, true, out outValue))
                return true;

            var newtarget = property.GetPropertyTargetReference();
            if (newtarget.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                target = newtarget;

            return InternalGetValueFromMember(target, memberName, true, out outValue);
        }

        public static bool GetInfoFromMember<T>(this SerializedProperty property, object target, string memberName,
            out MightyInfo<T> mightyInfo, GetValuePredicate<T> predicate = null)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                mightyInfo = null;
                return false;
            }

            if (predicate != null && predicate(memberName, out var outValue))
            {
                mightyInfo = new MightyInfo<T>(null, null, outValue);
                return true;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return property.GetInfoFromMember(property.GetWrappedObject(target.GetType()), callbackName, out mightyInfo, predicate);

            if (InternalGetInfoFromMember(target, memberName, true, out mightyInfo))
                return true;

            var newtarget = property.GetPropertyTargetReference();
            if (newtarget.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                target = newtarget;

            return InternalGetInfoFromMember(target, memberName, true, out mightyInfo);
        }

        public static bool GetMightyMethod<T>(this SerializedProperty property, object target, string memberName,
            CallbackSignature callbackSignature, out MightyMethod<T> mightyMethod)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                mightyMethod = null;
                return false;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return property.GetMightyMethod(property.GetWrappedObject(target.GetType()), callbackName, callbackSignature,
                    out mightyMethod);

            if (InternalGetMightyMethod(target, memberName, callbackSignature, out mightyMethod))
                return true;

            var newtarget = property.GetPropertyTargetReference();
            if (newtarget.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                target = newtarget;

            return InternalGetMightyMethod(target, memberName, callbackSignature, out mightyMethod);
        }

        public static bool GetMightyMethod<T>(this object target, string memberName, CallbackSignature callbackSignature,
            out MightyMethod<T> mightyMethod)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                mightyMethod = null;
                return false;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return target.GetWrappedObject(target.GetType()).GetMightyMethod(callbackName, callbackSignature,
                    out mightyMethod);

            return InternalGetMightyMethod(target, memberName, callbackSignature, out mightyMethod);
        }

        public static bool GetMightyVoidMethod(this SerializedProperty property, object target, string memberName,
            CallbackSignature callbackSignature, out MightyVoidMethod mightyMethod)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                mightyMethod = null;
                return false;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return property.GetMightyVoidMethod(property.GetWrappedObject(target.GetType()), callbackName, callbackSignature,
                    out mightyMethod);

            if (InternalGetMightyVoidMethod(target, memberName, callbackSignature, out mightyMethod))
                return true;

            var newtarget = property.GetPropertyTargetReference();
            if (newtarget.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                target = newtarget;

            return InternalGetMightyVoidMethod(target, memberName, callbackSignature, out mightyMethod);
        }

        public static bool GetValueFromMember<T>(this SerializedProperty property, object target, string memberName,
            out string callbackName, out T outValue, GetValuePredicate<T> predicate = null)
        {
            callbackName = memberName;
            if (string.IsNullOrWhiteSpace(memberName))
            {
                outValue = default;
                return false;
            }

            if (predicate != null && predicate(memberName, out outValue)) return true;

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out callbackName))
                return GetValueFromMember(property, property.GetWrappedObject(target.GetType()), callbackName, out callbackName,
                    out outValue);

            if (InternalGetValueFromMember(target, memberName, true, out outValue))
                return true;

            var newtarget = property.GetPropertyTargetReference();
            if (newtarget.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                target = newtarget;

            return InternalGetValueFromMember(target, memberName, true, out outValue);
        }

        private static bool InternalGetValueFromMember<T>(object target, string memberName, bool checkElementType, out T outValue)
        {
            var callbackSignature = new CallbackSignature(typeof(T), checkElementType);
            var fieldInfo = ReflectionUtility.GetField(target.GetType(), memberName);
            if (fieldInfo.IsFieldInfoValid(callbackSignature))
            {
                outValue = (T) fieldInfo.GetValue(target);
                return true;
            }

            var methodInfo = ReflectionUtility.GetMethod(target.GetType(), memberName);
            if (methodInfo.IsMethodInfoValid(callbackSignature))
            {
                outValue = (T) methodInfo.Invoke(target, null);
                return true;
            }

            var propertyInfo = ReflectionUtility.GetProperty(target.GetType(), memberName);
            if (propertyInfo.IsPropertyInfoValid(callbackSignature))
            {
                outValue = (T) propertyInfo.GetValue(target, null);
                return true;
            }

            outValue = default;
            return false;
        }

        private static bool InternalGetInfoFromMember<T>(object target, string memberName, bool checkElementType,
            out MightyInfo<T> mightyInfo)
        {
            var callbackSignature = new CallbackSignature(typeof(T), checkElementType);
            var fieldInfo = ReflectionUtility.GetField(target.GetType(), memberName);
            if (fieldInfo.IsFieldInfoValid(callbackSignature))
            {
                mightyInfo = new MightyInfo<T>(target, fieldInfo, (T) fieldInfo.GetValue(target));
                return true;
            }

            var methodInfo = ReflectionUtility.GetMethod(target.GetType(), memberName);
            if (methodInfo.IsMethodInfoValid(callbackSignature))
            {
                mightyInfo = new MightyInfo<T>(target, methodInfo, (T) methodInfo.Invoke(target, null));
                return true;
            }

            var propertyInfo = ReflectionUtility.GetProperty(target.GetType(), memberName);
            if (propertyInfo.IsPropertyInfoValid(callbackSignature))
            {
                mightyInfo = new MightyInfo<T>(target, propertyInfo, (T) propertyInfo.GetValue(target));
                return true;
            }

            mightyInfo = null;
            return false;
        }

        private static bool InternalGetMightyMethod<T>(object target, string memberName, CallbackSignature callbackSignature,
            out MightyMethod<T> mightyMethod)
        {
            var fieldInfo = ReflectionUtility.GetField(target.GetType(), memberName);
            if (fieldInfo.IsFieldInfoValid(callbackSignature))
            {
                mightyMethod = new MightyMethod<T>(target, fieldInfo);
                return true;
            }

            var methodInfo = ReflectionUtility.GetMethod(target.GetType(), memberName);
            if (methodInfo.IsMethodInfoValid(callbackSignature))
            {
                mightyMethod = new MightyMethod<T>(target, methodInfo);
                return true;
            }

            var propertyInfo = ReflectionUtility.GetProperty(target.GetType(), memberName);
            if (propertyInfo.IsPropertyInfoValid(callbackSignature))
            {
                mightyMethod = new MightyMethod<T>(target, propertyInfo);
                return true;
            }

            mightyMethod = null;
            return false;
        }

        private static bool InternalGetMightyVoidMethod(object target, string memberName, CallbackSignature callbackSignature,
            out MightyVoidMethod mightyMethod)
        {
            var fieldInfo = ReflectionUtility.GetField(target.GetType(), memberName);
            if (fieldInfo.IsFieldInfoValid(callbackSignature))
            {
                mightyMethod = new MightyVoidMethod(target, fieldInfo);
                return true;
            }

            var methodInfo = ReflectionUtility.GetMethod(target.GetType(), memberName);
            if (methodInfo.IsMethodInfoValid(callbackSignature))
            {
                mightyMethod = new MightyVoidMethod(target, methodInfo);
                return true;
            }

            var propertyInfo = ReflectionUtility.GetProperty(target.GetType(), memberName);
            if (propertyInfo.IsPropertyInfoValid(callbackSignature))
            {
                mightyMethod = new MightyVoidMethod(target, propertyInfo);
                return true;
            }

            mightyMethod = null;
            return false;
        }

        public static bool GetArrayValueFromMember(this object target, string memberName, out object[] outValue)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                outValue = null;
                return false;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return GetArrayValueFromMember(target.GetWrappedObject(target.GetType()),
                    callbackName, out outValue);

            return InternalGetArrayValueFromMember(target, memberName, out outValue);
        }

        public static bool GetArrayValueFromMember(this SerializedProperty property, object target, string memberName,
            out object[] outValue)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                outValue = null;
                return false;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return GetArrayValueFromMember(property, property.GetWrappedObject(target.GetType()),
                    callbackName, out outValue);

            if (InternalGetArrayValueFromMember(target, memberName, out outValue))
                return true;

            var newtarget = property.GetPropertyTargetReference();
            if (newtarget.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                target = newtarget;

            return InternalGetArrayValueFromMember(target, memberName, out outValue);
        }

        public static bool GetArrayInfoFromMember(this SerializedProperty property, object target, string memberName,
            out MightyInfo<object[]> mightyInfo)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                mightyInfo = null;
                return false;
            }

            if (target is MightyWrapperAttribute && GetCallbackName(target, memberName, out var callbackName))
                return GetArrayInfoFromMember(property, property.GetWrappedObject(target.GetType()),
                    callbackName, out mightyInfo);

            if (InternalGetArrayInfoFromMember(target, memberName, out mightyInfo))
                return true;

            var newtarget = property.GetPropertyTargetReference();
            if (newtarget.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                target = newtarget;

            return InternalGetArrayInfoFromMember(target, memberName, out mightyInfo);
        }

        private static bool InternalGetArrayValueFromMember(object target, string memberName, out object[] outValue)
        {
            var fieldInfo = ReflectionUtility.GetField(target.GetType(), memberName);
            if (fieldInfo != null)
            {
                if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                {
                    var list = (IList) fieldInfo.GetValue(target);
                    outValue = new object[list.Count];
                    for (var i = 0; i < list.Count; i++)
                        outValue[i] = list[i];
                    return true;
                }

                outValue = null;
                return false;
            }

            var methodInfo = ReflectionUtility.GetMethod(target.GetType(), memberName);
            if (methodInfo != null)
            {
                if (typeof(IList).IsAssignableFrom(methodInfo.ReturnType))
                {
                    var list = (IList) methodInfo.Invoke(target, null);
                    outValue = new object[list.Count];
                    for (var i = 0; i < list.Count; i++)
                        outValue[i] = list[i];
                    return true;
                }

                outValue = null;
                return false;
            }

            var propertyInfo = ReflectionUtility.GetProperty(target.GetType(), memberName);
            if (propertyInfo != null)
            {
                if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var list = (IList) propertyInfo.GetValue(target);
                    outValue = new object[list.Count];
                    for (var i = 0; i < list.Count; i++)
                        outValue[i] = list[i];
                    return true;
                }

                outValue = null;
                return false;
            }

            outValue = null;
            return false;
        }

        private static bool InternalGetArrayInfoFromMember(object target, string memberName, out MightyInfo<object[]> mightyInfo)
        {
            var fieldInfo = ReflectionUtility.GetField(target.GetType(), memberName);
            if (fieldInfo != null)
            {
                if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                {
                    var list = (IList) fieldInfo.GetValue(target);
                    mightyInfo = new MightyInfo<object[]>(target, fieldInfo, new object[list.Count]);

                    for (var i = 0; i < list.Count; i++)
                        mightyInfo.Value[i] = list[i];
                    return true;
                }

                mightyInfo = null;
                return false;
            }

            var methodInfo = ReflectionUtility.GetMethod(target.GetType(), memberName);
            if (methodInfo != null)
            {
                if (typeof(IList).IsAssignableFrom(methodInfo.ReturnType))
                {
                    var list = (IList) methodInfo.Invoke(target, null);
                    mightyInfo = new MightyInfo<object[]>(target, methodInfo, new object[list.Count]);

                    for (var i = 0; i < list.Count; i++)
                        mightyInfo.Value[i] = list[i];
                    return true;
                }

                mightyInfo = null;
                return false;
            }

            var propertyInfo = ReflectionUtility.GetProperty(target.GetType(), memberName);
            if (propertyInfo != null)
            {
                if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var list = (IList) propertyInfo.GetValue(target);
                    mightyInfo = new MightyInfo<object[]>(target, propertyInfo, new object[list.Count]);

                    for (var i = 0; i < list.Count; i++)
                        mightyInfo.Value[i] = list[i];
                    return true;
                }

                mightyInfo = null;
                return false;
            }

            mightyInfo = null;
            return false;
        }

        public static bool SetGenericValue(object target, SerializedProperty property, string valueName, SerializedPropertyType type)
        {
            switch (type)
            {
                case SerializedPropertyType.Generic:
                    var propertyType = property.GetSystemType();
                    if (!InternalGetValueFromMember(target, valueName, true, out object objectValue)) return false;

                    var propertyField = target.GetType().GetField(property.name);
                    var genericTarget = propertyField.GetValue(target);

                    var fields = propertyType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        if (field.IsPrivate && field.GetCustomAttributes(typeof(SerializeField), false).Length <= 0) continue;
                        var instanceField = objectValue.GetType().GetField(field.Name,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (instanceField == null) continue;
                        var value = instanceField.GetValue(objectValue);

                        field.SetValue(genericTarget, value);
                    }

                    if (!propertyType.IsClass)
                        propertyField.SetValue(target, genericTarget);

                    return true;

                case SerializedPropertyType.Enum:
                    if (!InternalGetValueFromMember(target, valueName, true, out Enum enumValue)) return false;

                    property.intValue = Convert.ToInt32(enumValue);
                    return true;
                case SerializedPropertyType.Integer:
                    if (!InternalGetValueFromMember(target, valueName, true, out int intValue)) return false;

                    property.intValue = intValue;
                    return true;

                case SerializedPropertyType.Boolean:
                    if (!InternalGetValueFromMember(target, valueName, true, out bool boolValue)) return false;

                    property.boolValue = boolValue;
                    return true;

                case SerializedPropertyType.Float:
                    if (!InternalGetValueFromMember(target, valueName, true, out float floatValue)) return false;

                    property.floatValue = floatValue;
                    return true;

                case SerializedPropertyType.String:
                    if (!InternalGetValueFromMember(target, valueName, true, out string stringValue)) return false;

                    property.stringValue = stringValue;
                    return true;

                case SerializedPropertyType.Color:
                    if (!InternalGetValueFromMember(target, valueName, true, out Color colorValue)) return false;

                    property.colorValue = colorValue;
                    return true;

                case SerializedPropertyType.ObjectReference:
                    if (!InternalGetValueFromMember(target, valueName, true, out Object unityObjectValue)) return false;

                    property.objectReferenceValue = unityObjectValue;
                    return true;

                case SerializedPropertyType.LayerMask:
                    if (!InternalGetValueFromMember(target, valueName, true, out LayerMask layerMaskValue)) return false;

                    property.intValue = layerMaskValue;
                    return true;

                case SerializedPropertyType.Vector2:
                    if (!InternalGetValueFromMember(target, valueName, true, out Vector2 vector2Value)) return false;

                    property.vector2Value = vector2Value;
                    return true;

                case SerializedPropertyType.Vector3:
                    if (!InternalGetValueFromMember(target, valueName, true, out Vector3 vector3Value)) return false;

                    property.vector3Value = vector3Value;
                    return true;

                case SerializedPropertyType.Vector4:
                    if (!InternalGetValueFromMember(target, valueName, true, out Vector4 vector4Value)) return false;

                    property.vector4Value = vector4Value;
                    return true;

                case SerializedPropertyType.Rect:
                    if (!InternalGetValueFromMember(target, valueName, true, out Rect rectValue)) return false;

                    property.rectValue = rectValue;
                    return true;

                case SerializedPropertyType.AnimationCurve:
                    if (!InternalGetValueFromMember(target, valueName, true, out AnimationCurve animationCurveValue)) return false;

                    property.animationCurveValue = animationCurveValue;
                    return true;

                case SerializedPropertyType.Bounds:
                    if (!InternalGetValueFromMember(target, valueName, true, out Bounds boundsValue)) return false;

                    property.boundsValue = boundsValue;
                    return true;

                case SerializedPropertyType.Quaternion:
                    if (!InternalGetValueFromMember(target, valueName, true, out Quaternion quaternionValue)) return false;

                    property.quaternionValue = quaternionValue;
                    return true;

                case SerializedPropertyType.Vector2Int:
                    if (!InternalGetValueFromMember(target, valueName, true, out Vector2Int vector2IntValue)) return false;

                    property.vector2IntValue = vector2IntValue;
                    return true;

                case SerializedPropertyType.Vector3Int:
                    if (!InternalGetValueFromMember(target, valueName, true, out Vector3Int vector3IntValue)) return false;

                    property.vector3IntValue = vector3IntValue;
                    return true;

                case SerializedPropertyType.RectInt:
                    if (!InternalGetValueFromMember(target, valueName, true, out RectInt rectIntValue)) return false;

                    property.rectIntValue = rectIntValue;
                    return true;

                case SerializedPropertyType.BoundsInt:
                    if (!InternalGetValueFromMember(target, valueName, true, out BoundsInt boundsIntValue)) return false;

                    property.boundsIntValue = boundsIntValue;
                    return true;

                default:
                    return false;
            }
        }

        public static bool SetArrayElementGenericValue(object target, SerializedProperty element, string valueName,
            SerializedPropertyType type, int index)
        {
            switch (type)
            {
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Integer:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var intValue)) return false;

                    element.intValue = Convert.ToInt32(intValue[index]);
                    return true;

                case SerializedPropertyType.Boolean:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var boolValue)) return false;

                    element.boolValue = bool.Parse(boolValue[index].ToString());
                    return true;

                case SerializedPropertyType.Float:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var floatValue)) return false;

                    element.floatValue = float.Parse(floatValue[index].ToString());
                    return true;

                case SerializedPropertyType.String:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var stringValue)) return false;

                    element.stringValue = (string) stringValue[index];
                    return true;

                case SerializedPropertyType.Color:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var colorValue)) return false;

                    element.colorValue = (Color) colorValue[index];
                    return true;

                case SerializedPropertyType.ObjectReference:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var unityObjectValue)) return false;

                    element.objectReferenceValue = (Object) unityObjectValue[index];
                    return true;

                case SerializedPropertyType.LayerMask:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var layerMaskValue)) return false;

                    element.intValue = (LayerMask) layerMaskValue[index];
                    return true;

                case SerializedPropertyType.Vector2:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var vector2Value)) return false;

                    element.vector2Value = (Vector2) vector2Value[index];
                    return true;

                case SerializedPropertyType.Vector3:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var vector3Value)) return false;

                    element.vector3Value = (Vector3) vector3Value[index];
                    return true;

                case SerializedPropertyType.Vector4:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var vector4Value)) return false;

                    element.vector4Value = (Vector4) vector4Value[index];
                    return true;

                case SerializedPropertyType.Rect:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var rectValue)) return false;

                    element.rectValue = (Rect) rectValue[index];
                    return true;

                case SerializedPropertyType.AnimationCurve:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var animationCurveValue)) return false;

                    element.animationCurveValue = (AnimationCurve) animationCurveValue[index];
                    return true;

                case SerializedPropertyType.Bounds:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var boundsValue)) return false;

                    element.boundsValue = (Bounds) boundsValue[index];
                    return true;

                case SerializedPropertyType.Quaternion:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var quaternionValue)) return false;

                    element.quaternionValue = (Quaternion) quaternionValue[index];
                    return true;

                case SerializedPropertyType.Vector2Int:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var vector2IntValue)) return false;

                    element.vector2IntValue = (Vector2Int) vector2IntValue[index];
                    return true;

                case SerializedPropertyType.Vector3Int:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var vector3IntValue)) return false;

                    element.vector3IntValue = (Vector3Int) vector3IntValue[index];
                    return true;

                case SerializedPropertyType.RectInt:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var rectIntValue)) return false;

                    element.rectIntValue = (RectInt) rectIntValue[index];
                    return true;

                case SerializedPropertyType.BoundsInt:
                    if (!InternalGetArrayValueFromMember(target, valueName, out var boundsIntValue)) return false;

                    element.boundsIntValue = (BoundsInt) boundsIntValue[index];
                    return true;

                default:
                    return false;
            }
        }

        public static object[] SetArrayGenericValue(SerializedProperty property, SerializedPropertyType type, object target)
        {
            var value = new object[property.arraySize];
            for (var i = 0; i < property.arraySize; i++)
                switch (type)
                {
                    case SerializedPropertyType.Generic:
                        var propertyField = target.GetField(property.name);
                        var fieldValue = propertyField.GetValue(target);
                        var arrayValue = (IList) fieldValue;
                        value[i] = arrayValue[i];
                        break;

                    case SerializedPropertyType.Enum:
                    case SerializedPropertyType.Integer:
                    case SerializedPropertyType.LayerMask:
                        value[i] = property.GetArrayElementAtIndex(i).intValue;
                        break;

                    case SerializedPropertyType.Boolean:
                        value[i] = property.GetArrayElementAtIndex(i).boolValue;
                        break;

                    case SerializedPropertyType.Float:
                        value[i] = property.GetArrayElementAtIndex(i).floatValue;
                        break;

                    case SerializedPropertyType.String:
                        value[i] = property.GetArrayElementAtIndex(i).stringValue;
                        break;

                    case SerializedPropertyType.Color:
                        value[i] = property.GetArrayElementAtIndex(i).colorValue;
                        break;

                    case SerializedPropertyType.ObjectReference:
                        value[i] = property.GetArrayElementAtIndex(i).objectReferenceValue;
                        break;

                    case SerializedPropertyType.Vector2:
                        value[i] = property.GetArrayElementAtIndex(i).vector2Value;
                        break;

                    case SerializedPropertyType.Vector3:
                        value[i] = property.GetArrayElementAtIndex(i).vector3Value;
                        break;

                    case SerializedPropertyType.Vector4:
                        value[i] = property.GetArrayElementAtIndex(i).vector4Value;
                        break;

                    case SerializedPropertyType.Rect:
                        value[i] = property.GetArrayElementAtIndex(i).rectValue;
                        break;

                    case SerializedPropertyType.AnimationCurve:
                        value[i] = property.GetArrayElementAtIndex(i).animationCurveValue;
                        break;

                    case SerializedPropertyType.Bounds:
                        value[i] = property.GetArrayElementAtIndex(i).boundsValue;
                        break;

                    case SerializedPropertyType.Quaternion:
                        value[i] = property.GetArrayElementAtIndex(i).quaternionValue;
                        break;

                    case SerializedPropertyType.Vector2Int:
                        value[i] = property.GetArrayElementAtIndex(i).vector2IntValue;
                        break;

                    case SerializedPropertyType.Vector3Int:
                        value[i] = property.GetArrayElementAtIndex(i).vector3IntValue;
                        break;

                    case SerializedPropertyType.RectInt:
                        value[i] = property.GetArrayElementAtIndex(i).rectIntValue;
                        break;

                    case SerializedPropertyType.BoundsInt:
                        value[i] = property.GetArrayElementAtIndex(i).boundsIntValue;
                        break;

                    default:
                        return null;
                }

            return value;
        }
    }

    #endregion /FieldInfo
}
#endif