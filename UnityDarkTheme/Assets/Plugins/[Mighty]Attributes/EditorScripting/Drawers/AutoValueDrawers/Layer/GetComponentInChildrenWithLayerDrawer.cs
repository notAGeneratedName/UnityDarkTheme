#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentInChildrenWithLayerAttribute))]
    public class GetComponentInChildrenWithLayerDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (GetComponentInChildrenWithLayerAttribute) baseAttribute;
            property.objectReferenceValue = property.GetComponentInChildrenWithLayer(attribute.Layer, attribute.IncludeInactive);
        }
    }
}
#endif