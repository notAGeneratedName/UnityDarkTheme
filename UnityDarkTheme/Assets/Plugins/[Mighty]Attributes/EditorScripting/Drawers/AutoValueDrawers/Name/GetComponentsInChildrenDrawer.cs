#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentsInChildrenAttribute))]
    public class GetComponentsInChildrenDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (GetComponentsInChildrenAttribute) baseAttribute;
            return (Object[]) property.GetComponentsInChildrenWithName(attribute.Name, attribute.IncludeInactive);
        }
    }
}
#endif