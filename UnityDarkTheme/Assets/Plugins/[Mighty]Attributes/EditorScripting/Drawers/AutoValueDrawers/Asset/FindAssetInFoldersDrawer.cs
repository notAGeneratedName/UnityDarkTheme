#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindAssetInFoldersAttribute))]
    public class FindAssetInFoldersDrawer : BaseSearchDrawer
    {
        protected override void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindAssetInFoldersAttribute) baseAttribute;
            property.objectReferenceValue = property.FindAssetInFolders(attribute.Name, attribute.Folders);
        }
    }
}
#endif