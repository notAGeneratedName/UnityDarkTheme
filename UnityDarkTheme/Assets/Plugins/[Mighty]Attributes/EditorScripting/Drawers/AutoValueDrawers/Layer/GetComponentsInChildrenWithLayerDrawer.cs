#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentsInChildrenWithLayerAttribute))]
    public class GetComponentsInChildrenWithLayerDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (GetComponentsInChildrenWithLayerAttribute) baseAttribute;
            return (Object[]) property.GetComponentsInChildrenWithLayer(attribute.Layer, attribute.IncludeInactive);
        }
    }
}
#endif