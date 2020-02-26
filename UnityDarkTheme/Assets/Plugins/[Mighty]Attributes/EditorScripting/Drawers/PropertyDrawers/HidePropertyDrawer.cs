#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(HideAttribute))]
    public class HidePropertyDrawer : BasePropertyDrawer
    {
        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
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