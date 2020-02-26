#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindObjectsAttribute))]
    public class FindObjectsDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindObjectsAttribute) baseAttribute;
            return property.FindObjectsWithName(attribute.Name, attribute.IncludeInactive);
        }
    }
}
#endif