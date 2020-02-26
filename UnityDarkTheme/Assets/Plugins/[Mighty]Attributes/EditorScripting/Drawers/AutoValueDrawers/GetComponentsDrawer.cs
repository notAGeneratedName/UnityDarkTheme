#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentsAttribute))]
    public class GetComponentsDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            return property.GetGameObject().GetComponents(property.GetSystemType());
        }
    }
}
#endif