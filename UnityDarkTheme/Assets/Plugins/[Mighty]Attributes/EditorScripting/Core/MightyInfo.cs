#if UNITY_EDITOR
using System;
using System.Reflection;

namespace MightyAttributes.Editor
{
    public abstract class BaseMightyInfo
    {
        protected readonly object Target;
        protected readonly MemberInfo Member;

        protected BaseMightyInfo(BaseMightyInfo copy) : this(copy?.Target, copy?.Member)
        {
        }

        protected BaseMightyInfo(object target, MemberInfo member)
        {
            Target = target;
            Member = member;
        }

        public Type MemberType
        {
            get
            {
                switch (Member)
                {
                    case FieldInfo fieldInfo:
                        return fieldInfo.FieldType;
                    case MethodInfo methodInfo:
                        return methodInfo.ReturnType;
                    case PropertyInfo propertyInfo:
                        return propertyInfo.PropertyType;
                }

                return null;
            }
        }

        public Type ElementType => GetElementType(MemberType);

        private Type GetElementType(Type memberType) => memberType == null ? null :
            memberType.IsGenericType ? memberType.GetGenericArguments()[0] : memberType.GetElementType();
    }

    public class MightyInfo<T> : BaseMightyInfo
    {
        public T Value { get; set; }

        public MightyInfo(BaseMightyInfo copy, T value) : base(copy) => Value = value;

        public MightyInfo(object target, MemberInfo member, T value) : base(target, member) => Value = value;

        public MightyInfo(T value) : base(null, null) => Value = value;

        public T RefreshValue()
        {
            try
            {
                switch (Member)
                {
                    case FieldInfo fieldInfo:
                        return Value = (T) fieldInfo.GetValue(Target);
                    case MethodInfo methodInfo:
                        return Value = (T) methodInfo.Invoke(Target, null);
                    case PropertyInfo propertyInfo:
                        return Value = (T) propertyInfo.GetValue(Target);
                }
            }
            catch
            {
                return Value;
            }

            return Value;
        }
    }

    public class MightyMethod<T> : BaseMightyInfo
    {
        public MightyMethod(BaseMightyInfo copy) : base(copy)
        {
        }

        public MightyMethod(object target, MemberInfo member) : base(target, member)
        {
        }

        public T Invoke(params object[] parameters) => Member is MethodInfo method ? (T) method.Invoke(Target, parameters) : default;
    }

    public class MightyVoidMethod : BaseMightyInfo
    {
        public MightyVoidMethod(BaseMightyInfo copy) : base(copy)
        {
        }

        public MightyVoidMethod(object target, MemberInfo member) : base(target, member)
        {
        }

        public void Invoke(params object[] parameters)
        {
            if (Member is MethodInfo method) method.Invoke(Target, parameters);
        }
    }
}
#endif