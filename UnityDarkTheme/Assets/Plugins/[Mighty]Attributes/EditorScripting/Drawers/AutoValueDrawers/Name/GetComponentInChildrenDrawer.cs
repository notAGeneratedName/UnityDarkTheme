#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(GetComponentInChildrenAttribute))]
    public class GetComponentInChildrenDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (GetComponentInChildrenAttribute) baseAttribute;
            property.objectReferenceValue = property.GetComponentInChildrenWithName(attribute.Name, attribute.IncludeInactive);
        }
    }
}
#endif