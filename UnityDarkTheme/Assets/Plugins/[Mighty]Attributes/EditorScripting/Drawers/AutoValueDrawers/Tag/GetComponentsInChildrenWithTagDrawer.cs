#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentsInChildrenWithTagAttribute))]
    public class GetComponentsInChildrenWithTagDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (GetComponentsInChildrenWithTagAttribute) baseAttribute;
            return (Object[]) property.GetComponentsInChildrenWithTag(attribute.Tag, attribute.IncludeInactive);
        }
    }
}
#endif