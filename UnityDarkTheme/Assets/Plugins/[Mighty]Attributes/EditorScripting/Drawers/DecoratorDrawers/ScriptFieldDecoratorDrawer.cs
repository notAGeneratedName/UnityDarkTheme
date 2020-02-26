#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ScriptFieldAttribute))]
    public class ScriptFieldDecoratorDrawer : BaseDecoratorDrawer, IDrawAnywhereDecorator
    {
        public void BeginDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute) =>
            BeginDraw(mightyMember, (BaseDecoratorAttribute) attribute);

        public void EndDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute) =>
            EndDraw(mightyMember, (BaseDecoratorAttribute) attribute);

        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute attribute)
        {
            if (((ScriptFieldAttribute) attribute).Position != FieldPosition.Before) return;

            var enabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(mightyMember.Property.serializedObject.FindProperty("m_Script"));
            GUI.enabled = enabled;
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute attribute)
        {
            if (((ScriptFieldAttribute) attribute).Position != FieldPosition.After) return;

            var enabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(mightyMember.Property.serializedObject.FindProperty("m_Script"));
            GUI.enabled = enabled;
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