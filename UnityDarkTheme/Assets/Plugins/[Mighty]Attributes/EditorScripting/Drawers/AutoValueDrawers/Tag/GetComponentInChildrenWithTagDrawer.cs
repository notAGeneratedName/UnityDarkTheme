#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentInChildrenWithTagAttribute))]
    public class GetComponentInChildrenWithTagDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (GetComponentInChildrenWithTagAttribute) baseAttribute;
            property.objectReferenceValue = property.GetComponentInChildrenWithTag(attribute.Tag, attribute.IncludeInactive);
        }
    }
}
#endif