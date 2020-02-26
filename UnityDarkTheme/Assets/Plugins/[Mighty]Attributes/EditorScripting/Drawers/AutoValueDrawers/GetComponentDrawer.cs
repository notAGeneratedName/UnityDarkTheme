#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentAttribute))]
    public class GetComponentDrawer : BaseAutoValueDrawer
    {
        protected override InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            if (!typeof(Object).IsAssignableFrom(property.GetSystemType()))
                return new InitState(false, "\"" + property.displayName + "\" should inherit from UnityEngine.Object");

            property.objectReferenceValue = property.GetGameObject().GetComponent(property.GetSystemType());
            return new InitState(true);
        }
    }
}
#endif