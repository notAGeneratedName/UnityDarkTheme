#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }
            
            DrawSlider(property, (MinMaxSliderAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(mightyMember.GetElement(index), (MinMaxSliderAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(mightyMember.GetElement(index), (MinMaxSliderAttribute) baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(rect, mightyMember.GetElement(index), (MinMaxSliderAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            mightyMember.GetElement(index).propertyType != SerializedPropertyType.Vector2 ? 66 : 22;

        private void DrawSlider(SerializedProperty property, MinMaxSliderAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
                Draw(EditorGUILayout.GetControlRect(), property, attribute, label);
            else
            {
                EditorDrawUtility.DrawPropertyField(property, label);
                EditorDrawUtility.DrawHelpBox($"{typeof(MinMaxSliderAttribute).Name} can be used only on Vector2 fields");
            }
        }

        private void DrawSlider(Rect position, SerializedProperty property, MinMaxSliderAttribute attribute)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
                Draw(position, property, attribute);
            else
            {
                EditorDrawUtility.DrawPropertyField(position, property);
                position.y += 18;
                position.height -= 26;
                EditorDrawUtility.DrawHelpBox(position, $"{typeof(MinMaxSliderAttribute).Name} can be used only on Vector2 fields");
            }
        }

        private void Draw(Rect rect, SerializedProperty property, MinMaxSliderAttribute attribute, GUIContent label = null)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            var floatFieldWidth = EditorGUIUtility.fieldWidth;
            var sliderWidth = rect.width - labelWidth - 2f * floatFieldWidth;
            var sliderPadding = 5f;

            var labelRect = new Rect(
                rect.x,
                rect.y,
                labelWidth,
                rect.height);

            var sliderRect = new Rect(
                rect.x + labelWidth + floatFieldWidth + sliderPadding,
                rect.y,
                sliderWidth - 2f * sliderPadding,
                rect.height);

            var minFloatFieldRect = new Rect(
                rect.x + labelWidth,
                rect.y,
                floatFieldWidth,
                rect.height);

            var maxFloatFieldRect = new Rect(
                rect.x + labelWidth + floatFieldWidth + sliderWidth,
                rect.y,
                floatFieldWidth,
                rect.height);

            // Draw the label
            DrawLabel(ref labelRect, property, attribute, label);

            // Draw the slider
            EditorGUI.BeginChangeCheck();

            var sliderValue = property.vector2Value;
            EditorGUI.MinMaxSlider(sliderRect, ref sliderValue.x, ref sliderValue.y, attribute.MinValue,
                attribute.MaxValue);

            sliderValue.x = EditorGUI.FloatField(minFloatFieldRect, sliderValue.x);
            sliderValue.x = Mathf.Clamp(sliderValue.x, attribute.MinValue,
                Mathf.Min(attribute.MaxValue, sliderValue.y));

            sliderValue.y = EditorGUI.FloatField(maxFloatFieldRect, sliderValue.y);
            sliderValue.y = Mathf.Clamp(sliderValue.y, Mathf.Max(attribute.MinValue, sliderValue.x),
                attribute.MaxValue);

            if (EditorGUI.EndChangeCheck()) property.vector2Value = sliderValue;
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