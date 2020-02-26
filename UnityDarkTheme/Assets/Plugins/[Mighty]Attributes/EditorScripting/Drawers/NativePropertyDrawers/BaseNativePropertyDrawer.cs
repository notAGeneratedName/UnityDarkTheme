#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BaseNativePropertyDrawer : BaseMightyDrawer
    {
        public abstract void DrawNativeProperty(MightyMember<PropertyInfo> mightyMember, BasePropertyAttribute baseAttribute);
    }
}
#endif