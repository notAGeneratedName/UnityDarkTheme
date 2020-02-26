#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(HeaderAttribute))]
    public class HeaderDecoratorDrawer : BaseDecoratorDrawer
    {
        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            EditorGUILayout.LabelField(((HeaderAttribute) baseAttribute).Header, EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
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