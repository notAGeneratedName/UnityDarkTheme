#if UNITY_EDITOR
using System;

namespace MightyAttributes.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawerTargetAttribute : Attribute
    {
        public Type[] TargetAttributeTypes { get; }

        public DrawerTargetAttribute(params Type[] targetAttributeTypes) => TargetAttributeTypes = targetAttributeTypes;
    }
}
#endif