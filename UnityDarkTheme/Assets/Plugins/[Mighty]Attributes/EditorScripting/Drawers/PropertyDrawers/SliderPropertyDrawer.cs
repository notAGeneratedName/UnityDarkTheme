#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(SliderAttribute))]
    public class SliderPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(bool, MightyInfo<float>, MightyInfo<float>)> m_sliderCache =
            new MightyCache<(bool, MightyInfo<float>, MightyInfo<float>)>();

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawSlider(mightyMember, property, (SliderAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(mightyMember, mightyMember.GetElement(index), (SliderAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index,
            BaseDrawerAttribute baseAttribute) =>
            DrawSlider(mightyMember, mightyMember.GetElement(index), (SliderAttribute) baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawSlider(rect, mightyMember, mightyMember.GetElement(index), (SliderAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            var propertyType = mightyMember.GetElement(index).propertyType;
            return propertyType != SerializedPropertyType.Integer && propertyType != SerializedPropertyType.Float ? 64 : 20;
        }

        public void DrawSlider(BaseMightyMember mightyMember, SerializedProperty property, SliderAttribute attribute,
            GUIContent label = null)
        {
            if (!m_sliderCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, min, max) = m_sliderCache[mightyMember];

            if (!valid)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"{typeof(SliderAttribute).Name} can be used only on int or float fields");
                return;
            }
            
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = attribute.Option.Contains(FieldOption.HideLabel)
                        ? EditorGUILayout.IntSlider(property.intValue, (int) min.Value, (int) max.Value)
                        : EditorGUILayout.IntSlider(label ?? EditorGUIUtility.TrTextContent(property.displayName), property.intValue,
                            (int) min.Value, (int) max.Value);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = attribute.Option.Contains(FieldOption.HideLabel)
                        ? EditorGUILayout.Slider(property.floatValue, min.Value, max.Value)
                        : EditorGUILayout.Slider(label ?? EditorGUIUtility.TrTextContent(property.displayName), property.floatValue,
                            min.Value, max.Value);
                    break;
            }
        }

        public void DrawSlider(Rect rect, BaseMightyMember mightyMember, SerializedProperty property, SliderAttribute attribute,
            GUIContent label = null)
        {
            if (!m_sliderCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, min, max) = m_sliderCache[mightyMember];
            
            if (!valid)
            {
                EditorDrawUtility.DrawPropertyField(rect, property);
                rect.y += 18;
                rect.height -= 24;
                EditorDrawUtility.DrawHelpBox(rect, $"{typeof(SliderAttribute).Name} can be used only on int or float fields");
                return;
            }
            
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = attribute.Option.Contains(FieldOption.HideLabel)
                        ? EditorGUI.IntSlider(rect, property.intValue, (int) min.Value, (int) max.Value)
                        : EditorGUI.IntSlider(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName), property.intValue,
                            (int) min.Value, (int) max.Value);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = attribute.Option.Contains(FieldOption.HideLabel)
                        ? EditorGUI.Slider(rect, property.floatValue, min.Value, max.Value)
                        : EditorGUI.Slider(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName), property.floatValue,
                            min.Value, max.Value);
                    break;
            }
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var property = mightyMember.Property;
            var attribute = (SliderAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<SliderAttribute>();
            
            switch (property.GetPropertyType())
            {
                case SerializedPropertyType.Integer:
                {
                    var minValueInfo = new MightyInfo<float>(null, null, attribute.MinValue);
                    if (property.GetInfoFromMember<int>(target, attribute.MinValueCallback, out var minInfo))
                        minValueInfo = new MightyInfo<float>(minInfo, minInfo.Value);


                    var maxValueInfo = new MightyInfo<float>(null, null, attribute.MaxValue);
                    if (property.GetInfoFromMember<int>(target, attribute.MaxValueCallback, out var maxInfo))
                        maxValueInfo = new MightyInfo<float>(maxInfo, maxInfo.Value);

                    m_sliderCache[mightyMember] = (true, minValueInfo, maxValueInfo);
                    break;
                }
                case SerializedPropertyType.Float:
                {
                    var minValueInfo = new MightyInfo<float>(null, null, attribute.MinValue);
                    if (property.GetInfoFromMember<float>(target, attribute.MinValueCallback, out var minInfo))
                        minValueInfo = new MightyInfo<float>(minInfo, minInfo.Value);


                    var maxValueInfo = new MightyInfo<float>(null, null, attribute.MaxValue);
                    if (property.GetInfoFromMember<float>(target, attribute.MaxValueCallback, out var maxInfo))
                        maxValueInfo = new MightyInfo<float>(maxInfo, maxInfo.Value);

                    m_sliderCache[mightyMember] = (true, minValueInfo, maxValueInfo);
                    break;
                }
                default:
                    m_sliderCache[mightyMember] = default;
                    break;
            }
        }

        public override void ClearCache() => m_sliderCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_sliderCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, min, max) = m_sliderCache[mightyMember];
            if (!valid) return;
            
            min.RefreshValue();
            max.RefreshValue();
        }
    }
}
#endif