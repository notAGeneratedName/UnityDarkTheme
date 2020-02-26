#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(MinMaxAttribute))]
    public class MinMaxPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        private static readonly float[] MinMaxWidths = {25, 27};

        private static readonly GUIContent[] MinMaxContent = {EditorGUIUtility.TrTextContent("Min"), EditorGUIUtility.TrTextContent("Max")};

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawMinMax(property, baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMinMax(mightyMember.GetElement(index), baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMinMax(mightyMember.GetElement(index), baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMinMax(rect, mightyMember.GetElement(index), baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 16;

        private void DrawMinMax(SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorDrawUtility.DrawHelpBox($"{property.name} should be of type UnityEngine.Vector2");
                return;
            }

            property.vector2Value = DrawMinMax(EditorGUILayout.GetControlRect(true, 16),
                label ?? EditorGUIUtility.TrTextContent(property.displayName), property.vector2Value, attribute);
        }

        private void DrawMinMax(Rect rect, SerializedProperty property, BaseDrawerAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorDrawUtility.DrawHelpBox(rect, $"{property.name} should be of type UnityEngine.Vector2");
                return;
            }

            property.vector2Value =
                DrawMinMax(rect, EditorGUIUtility.TrTextContent(property.displayName), property.vector2Value, attribute);
        }

        private Vector2 DrawMinMax(Rect position, GUIContent label, Vector2 minMaxVector, BaseDrawerAttribute attribute)
        {
            var vector2Array = minMaxVector.Vector2ToArray();

            DrawLabel(ref position, null, attribute, label);

            EditorGUI.BeginChangeCheck();
            EditorDrawUtility.MultiFloatField(position, MinMaxContent, vector2Array, MinMaxWidths, Orientation.Horizontal);
            if (EditorGUI.EndChangeCheck())
                minMaxVector = vector2Array.ArrayToVector2();
            return minMaxVector;
        }

        protected override bool DrawLabel(ref Rect rect, SerializedProperty property, BaseDrawerAttribute baseAttribute, GUIContent label)
        {
            if (baseAttribute.Option.Contains(FieldOption.HideLabel)) return false;

            if (baseAttribute.Option.Contains(FieldOption.BoldLabel))
                rect = EditorDrawUtility.MultiFieldPrefixLabel(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName),
                    MinMaxContent.Length, EditorStyles.whiteBoldLabel);
            else
                rect = EditorDrawUtility.MultiFieldPrefixLabel(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName),
                    MinMaxContent.Length, EditorStyles.whiteLabel);

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