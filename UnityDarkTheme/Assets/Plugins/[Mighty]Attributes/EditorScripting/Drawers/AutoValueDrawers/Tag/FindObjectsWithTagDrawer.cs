#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindObjectsWithTagAttribute))]
    public class FindObjectsWithTagDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindObjectsWithTagAttribute) baseAttribute;
            return property.FindObjectsWithTag(attribute.Tag, attribute.IncludeInactive);
        }
    }
}
#endif