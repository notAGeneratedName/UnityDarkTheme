#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindObjectWithTagAttribute))]
    public class FindObjectWithTagDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindObjectWithTagAttribute) baseAttribute;
            property.objectReferenceValue = property.FindObjectWithTag(attribute.Tag, attribute.IncludeInactive);
        }
    }
}
#endif