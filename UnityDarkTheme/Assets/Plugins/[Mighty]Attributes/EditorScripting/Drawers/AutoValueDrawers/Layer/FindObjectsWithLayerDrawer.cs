#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindObjectsWithLayerAttribute))]
    public class FindObjectsWithLayerDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindObjectsWithLayerAttribute) baseAttribute;
            return property.FindObjectsWithLayer(attribute.Layer, attribute.IncludeInactive);
        }
    }
}
#endif