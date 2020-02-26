#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindObjectWithLayerAttribute))]
    public class FindObjectWithLayerDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindObjectWithLayerAttribute) baseAttribute;
            property.objectReferenceValue = property.FindObjectWithLayer(attribute.Layer, attribute.IncludeInactive);
        }
    }
}
#endif