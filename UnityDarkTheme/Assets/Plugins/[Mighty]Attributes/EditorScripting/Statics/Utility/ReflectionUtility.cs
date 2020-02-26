#if UNITY_EDITOR
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MightyAttributes.Editor
{
    public class CallbackSignature
    {
        public static readonly CallbackSignature VoidNoParams = new CallbackSignature(null);

        public Type ReturnType { get; }
        public bool CheckElementType { get; }
        public Type[] ParamTypes { get; private set; }

        public CallbackSignature(Type returnType, params Type[] paramTypes)
        {
            ReturnType = returnType;
            ParamTypes = paramTypes;
        }

        public CallbackSignature(Type returnType, bool checkElementType, params Type[] paramTypes)
        {
            ReturnType = returnType;
            CheckElementType = checkElementType;
            ParamTypes = paramTypes;
        }

        public void SetParamsType(params Type[] paramTypes) => ParamTypes = paramTypes;

        public bool IsMethodValid(MethodInfo callbackInfo)
        {
            if (callbackInfo == null) return false;
            var callbackType = CheckElementType && callbackInfo.ReturnType.IsArray
                ? callbackInfo.ReturnType.GetElementType()
                : callbackInfo.ReturnType;

            if (!CompareTypes(UnderlyingTypeIfNullable(ReturnType), UnderlyingTypeIfNullable(callbackType))) return false;

            var callbackParameters = callbackInfo.GetParameters();
            if ((ParamTypes == null || ParamTypes.Length == 0) && callbackParameters.Length == 0) return true;

            if (ParamTypes == null || ParamTypes.Length != callbackParameters.Length) return false;

            for (var i = 0; i < callbackParameters.Length; i++)
                if (!CompareTypes(ParamTypes[i], callbackParameters[i].ParameterType))
                    return false;

            return true;
        }

        public bool IsFieldValid(FieldInfo callbackInfo)
        {
            if (callbackInfo == null) return false;
            var callbackType = CheckElementType && callbackInfo.FieldType.IsArray
                ? callbackInfo.FieldType.GetElementType()
                : callbackInfo.FieldType;

            return CompareTypes(UnderlyingTypeIfNullable(ReturnType), UnderlyingTypeIfNullable(callbackType));
        }

        public bool IsPropertyValid(PropertyInfo callbackInfo)
        {
            if (callbackInfo == null) return false;
            var callbackType = CheckElementType && callbackInfo.PropertyType.IsArray
                ? callbackInfo.PropertyType.GetElementType()
                : callbackInfo.PropertyType;

            return CompareTypes(UnderlyingTypeIfNullable(ReturnType), UnderlyingTypeIfNullable(callbackType));
        }

        private static Type UnderlyingTypeIfNullable(Type type) =>
            type != null && Nullable.GetUnderlyingType(type) is Type underlyingType ? underlyingType : type;

        private static bool CompareTypes(Type first, Type second)
        {
            if (first == null && (second == null || second == typeof(void))) return true;
            if (first == null) return false;
            return second != null && (second == first || second.IsSubclassOf(first));
        }
    }

    public static class ReflectionUtility
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
                                                   BindingFlags.Public | BindingFlags.DeclaredOnly;

        public static IEnumerable<FieldInfo> GetAllFields(Type type, Func<FieldInfo, bool> predicate)
        {
            var types = new List<Type> {type};
            while ((type = type.BaseType) != null) types.Add(type);
            
            for (var i = types.Count - 1; i >= 0; i--)
                foreach (var fieldInfo in types[i].GetFields(BINDING_FLAGS).Where(predicate))
                    yield return fieldInfo;
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(Type type, Func<PropertyInfo, bool> predicate)
        {
            var types = new List<Type> {type};
            while ((type = type.BaseType) != null) types.Add(type);

            for (var i = types.Count - 1; i >= 0; i--)
                foreach (var propertyInfo in types[i].GetProperties(BINDING_FLAGS).Where(predicate))
                    yield return propertyInfo;
        }

        public static IEnumerable<MethodInfo> GetAllMethods(Type type, Func<MethodInfo, bool> predicate)
        {
            var types = new List<Type> {type};
            while ((type = type.BaseType) != null) types.Add(type);

            for (var i = types.Count - 1; i >= 0; i--)
                foreach (var methodInfo in types[i].GetMethods(BINDING_FLAGS).Where(predicate))
                    yield return methodInfo;
        }

        public static FieldInfo GetField(Type type, string fieldName) =>
            GetAllFields(type, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();

        public static PropertyInfo GetProperty(Type type, string propertyName) =>
            GetAllProperties(type, p => p.Name.Equals(propertyName, StringComparison.InvariantCulture)).FirstOrDefault();

        public static MethodInfo GetMethod(Type type, string methodName) =>
            GetAllMethods(type, m => m.Name.Equals(methodName, StringComparison.InvariantCulture)).FirstOrDefault();

        public static FieldInfo GetField(this object target, string fieldName) => GetField(target.GetType(), fieldName);

        public static PropertyInfo GetProperty(this object target, string propertyName) => GetProperty(target.GetType(), propertyName);

        public static MethodInfo GetMethod(this object target, string methodName) => GetMethod(target.GetType(), methodName);

        public static bool IsFieldInfoValid(this FieldInfo fieldInfo, CallbackSignature callbackSignature) =>
            callbackSignature.IsFieldValid(fieldInfo);

        public static bool IsMethodInfoValid(this MethodInfo methodInfo, CallbackSignature callbackSignature) =>
            callbackSignature.IsMethodValid(methodInfo);

        public static bool IsPropertyInfoValid(this PropertyInfo propertyInfo, CallbackSignature callbackSignature) =>
            callbackSignature.IsPropertyValid(propertyInfo);

        public static bool IsCallbackName(this FieldInfo fieldInfo) =>
            fieldInfo != null && fieldInfo.GetCustomAttribute<CallbackNameAttribute>() != null;

        public static bool IsCallbackName(this MethodInfo methodInfo) =>
            methodInfo != null && methodInfo.GetCustomAttribute<CallbackNameAttribute>() != null;

        public static bool IsCallbackName(this PropertyInfo propertyInfo) =>
            propertyInfo != null && propertyInfo.GetCustomAttribute<CallbackNameAttribute>() != null;


        public static bool InfoExist(this Type targetType, string memberName) =>
            GetField(targetType, memberName) != null ||
            GetMethod(targetType, memberName) != null ||
            GetProperty(targetType, memberName) != null;

        public static bool InfoValid(this Type targetType, string memberName, [NotNull] CallbackSignature callbackSignature) =>
            GetField(targetType, memberName).IsFieldInfoValid(callbackSignature) ||
            GetMethod(targetType, memberName).IsMethodInfoValid(callbackSignature) ||
            GetProperty(targetType, memberName).IsPropertyInfoValid(callbackSignature);

        public static bool GetMemberInfo(this Type targetType, string memberName, [NotNull] CallbackSignature callbackSignature,
            out MemberInfo memberInfo)
        {
            memberInfo = GetField(targetType, memberName);
            if (callbackSignature.IsFieldValid((FieldInfo) memberInfo)) return true;
            memberInfo = GetMethod(targetType, memberName);
            if (callbackSignature.IsMethodValid((MethodInfo) memberInfo)) return true;
            memberInfo = GetProperty(targetType, memberName);
            return callbackSignature.IsPropertyValid((PropertyInfo) memberInfo);
        }

        public static T GetValueFromMember<T>(this MemberInfo memberInfo, object target, params object[] parameters)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    return (T) fieldInfo.GetValue(target);
                case MethodInfo methodInfo:
                    return (T) methodInfo.Invoke(target, parameters);
                case PropertyInfo propertyInfo:
                    return (T) propertyInfo.GetValue(target);
                default:
                    return default;
            }
        }
    }
}
#endif