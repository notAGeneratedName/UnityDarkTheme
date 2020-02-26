#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindObjectAttribute))]
    public class FindObjectDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindObjectAttribute) baseAttribute;
            property.objectReferenceValue = property.FindObjectWithName(attribute.Name, attribute.IncludeInactive);
        }
    }
}
#endif