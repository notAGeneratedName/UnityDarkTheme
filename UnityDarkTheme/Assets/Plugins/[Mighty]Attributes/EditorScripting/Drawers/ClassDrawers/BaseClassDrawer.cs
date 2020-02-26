#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BaseClassDrawer : BaseMightyDrawer
    {
        public abstract void OnEnableClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer);
        public abstract void OnDisableClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer);
        public abstract void BeginDrawClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer);
        public abstract void EndDrawClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer);
    }
}
#endif