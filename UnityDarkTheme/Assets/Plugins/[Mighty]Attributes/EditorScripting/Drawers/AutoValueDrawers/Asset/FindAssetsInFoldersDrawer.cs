#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindAssetsInFoldersAttribute))]
    public class FindAssetsInFoldersDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (FindAssetsInFoldersAttribute) baseAttribute;
            return property.FindAssetsInFolders(attribute.Name, attribute.Folders);
        }
    }
}
#endif