#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FindAssetsAttribute))]
    public class FindAssetsDrawer : BaseAutoValueArrayDrawer
    {
        protected override Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute) => 
            property.FindAssetsWithName(((FindAssetsAttribute) baseAttribute).Name);
    }
}
#endif