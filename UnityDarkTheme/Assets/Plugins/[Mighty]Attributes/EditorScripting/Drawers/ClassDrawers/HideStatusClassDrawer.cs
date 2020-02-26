#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(HideClassAttribute), typeof(HideScriptFieldAttribute))]
    public class HideStatusClassDrawer : BaseClassDrawer
    {
        public override void OnEnableClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer)
        {
            var hideStatusAttribute = (HideStatusClassAttribute) attribute;
            drawer.hideStatus |= hideStatusAttribute.HideStatus;
        }

        public override void OnDisableClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer)
        {
        }

        public override void BeginDrawClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer)
        {
            var hideStatusAttribute = (HideStatusClassAttribute) attribute;
            drawer.hideStatus |= hideStatusAttribute.HideStatus;
        }

        public override void EndDrawClass(SerializedProperty script, Object context, BaseClassAttribute attribute, MightyDrawer drawer)
        {
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif