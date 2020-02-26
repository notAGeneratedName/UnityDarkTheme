#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(MarginsAttribute))]
    public class MarginsPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        private static readonly float[] MarginsWidths = {35, 27, 40, 45};

        private static readonly GUIContent[] MarginsContent =
        {
            EditorGUIUtility.TrTextContent("Left"),
            EditorGUIUtility.TrTextContent("Top"),
            EditorGUIUtility.TrTextContent("Right"),
            EditorGUIUtility.TrTextContent("Bottom")
        };

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawMargins(property, baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMargins(mightyMember.GetElement(index), baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index,
            BaseDrawerAttribute baseAttribute) => DrawMargins(mightyMember.GetElement(index), baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMargins(rect, mightyMember.GetElement(index), baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 32;

        private void DrawMargins(SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Vector4)
            {
                EditorDrawUtility.DrawHelpBox($"{property.name} should be of type UnityEngine.Vector4");
                return;
            }

            property.vector4Value = DrawMargins(EditorGUILayout.GetControlRect(true, 32),
                label ?? EditorGUIUtility.TrTextContent(property.displayName), property.vector4Value, attribute);
        }

        private void DrawMargins(Rect rect, SerializedProperty property, BaseDrawerAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Vector4)
            {
                EditorDrawUtility.DrawHelpBox(rect, $"{property.name} should be of type UnityEngine.Vector4");
                return;
            }

            property.vector4Value =
                DrawMargins(rect, EditorGUIUtility.TrTextContent(property.displayName), property.vector4Value, attribute);
        }

        private Vector4 DrawMargins(Rect position, GUIContent label, Vector4 margins, BaseDrawerAttribute attribute)
        {
            var marginsArray = margins.Vector4ToArray();

            DrawLabel(ref position, null, attribute, label);

            EditorGUI.BeginChangeCheck();
            EditorDrawUtility.MultiFloatField(position, MarginsContent, marginsArray, MarginsWidths, Orientation.Vertical);
            if (EditorGUI.EndChangeCheck())
                margins = marginsArray.ArrayToVector4();
            return margins;
        }

        protected override bool DrawLabel(ref Rect rect, SerializedProperty property, BaseDrawerAttribute baseAttribute, GUIContent label)
        {
            if (baseAttribute.Option.Contains(FieldOption.HideLabel)) return false;

            rect = EditorDrawUtility.MultiFieldPrefixLabel(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName),
                MarginsContent.Length,
                baseAttribute.Option.Contains(FieldOption.BoldLabel) ? EditorStyles.whiteBoldLabel : EditorStyles.whiteLabel);

            return true;
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