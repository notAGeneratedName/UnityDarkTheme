#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(PercentSliderAttribute))]
    public class PercentSliderPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawSlider(property, (PercentSliderAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(mightyMember.GetElement(index), (PercentSliderAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(mightyMember.GetElement(index), (PercentSliderAttribute) baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(rect, mightyMember.GetElement(index), (PercentSliderAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            var propertyType = mightyMember.GetElement(index).propertyType;
            return propertyType != SerializedPropertyType.Integer && propertyType != SerializedPropertyType.Float ? 64 : 20;
        }

        public void DrawSlider(SerializedProperty property, PercentSliderAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                GUILayout.BeginHorizontal();
                property.floatValue = attribute.Option.Contains(FieldOption.HideLabel)
                    ? EditorGUILayout.Slider(property.floatValue * 100, 0, 100) / 100
                    : EditorGUILayout.Slider(property.displayName, property.floatValue * 100, 0, 100) / 100;
                GUILayout.Label("%", GUILayout.Width(15));
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"{typeof(PercentSliderAttribute).Name} can be used only on float fields");
            }
        }

        public void DrawSlider(Rect rect, SerializedProperty property, PercentSliderAttribute attribute)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = attribute.Option.Contains(FieldOption.HideLabel)
                    ? EditorGUI.Slider(rect, property.floatValue * 100, 0, 100) / 100
                    : EditorGUI.Slider(rect, property.displayName, property.floatValue * 100, 0, 100) / 100;
                GUI.Label(rect, "%");
            }
            else
            {
                EditorDrawUtility.DrawPropertyField(rect, property);
                rect.y += 18;
                rect.height -= 24;
                EditorDrawUtility.DrawHelpBox(rect, $"{typeof(PercentSliderAttribute).Name} can be used only on float fields");
            }
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