#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindAssetAttribute))]
    public class FindAssetDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute) => 
            property.objectReferenceValue = property.FindAssetWithName(((FindAssetAttribute) baseAttribute).Name);
    }
}
#endif