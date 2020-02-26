#if UNITY_EDITOR
using System.Globalization;
using UnityEngine;
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ProgressBarAttribute))]
    public class ProgressBarPropertyDrawer : BasePropertyDrawer, IRefreshDrawer
    {
        private readonly MightyCache<MightyInfo<Color?>> m_progressBarCache = new MightyCache<MightyInfo<Color?>>();

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
                EditorDrawUtility.DrawHelpBox($"Field {property.name} is not a number");
                return;
            }

            var value = property.propertyType == SerializedPropertyType.Integer ? property.intValue : property.floatValue;
            var valueFormatted = property.propertyType == SerializedPropertyType.Integer
                ? value.ToString(CultureInfo.InvariantCulture)
                : $"{value:0.00}";

            var progressBarAttribute = (ProgressBarAttribute) baseAttribute;
            var position = EditorGUILayout.GetControlRect();
            var maxValue = progressBarAttribute.MaxValue;
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var barPosition = new Rect(position.position.x, position.position.y, position.size.x, lineHeight);

            var fillPercentage = value / maxValue;
            var barLabel =
                $"{(!string.IsNullOrEmpty(progressBarAttribute.Name) ? $"[{progressBarAttribute.Name}] " : "")}{valueFormatted}/{maxValue}";

            if (!m_progressBarCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var color = m_progressBarCache[mightyMember].Value ?? Color.white;
            var color2 = Color.white;
            DrawBar(barPosition, Mathf.Clamp01(fillPercentage), barLabel, color, color2, (ProgressBarAttribute) baseAttribute);
        }

        private void DrawBar(Rect position, float fillPercent, string label, Color barColor, Color labelColor,
            ProgressBarAttribute attribute)
        {
            if (Event.current.type != EventType.Repaint) return;

            var fillRect = new Rect(position.x, position.y, position.width * fillPercent, position.height);

            EditorGUI.DrawRect(position, new Color(0.13f, 0.13f, 0.13f));
            EditorGUI.DrawRect(fillRect, barColor);

            if (attribute.Option.Contains(FieldOption.HideLabel)) return;

            // set alignment and cache the default
            var align = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.UpperCenter;

            // set the color and cache the default
            var c = GUI.contentColor;
            GUI.contentColor = labelColor;

            // calculate the position
            var labelRect = new Rect(position.x, position.y - 2, position.width, position.height);

            // draw~
            if (attribute.Option.Contains(FieldOption.BoldLabel))
                EditorGUI.DropShadowLabel(labelRect, label, EditorStyles.boldLabel);
            else
                EditorGUI.DropShadowLabel(labelRect, label);

            // reset color and alignment
            GUI.contentColor = c;
            GUI.skin.label.alignment = align;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (ProgressBarAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<ProgressBarAttribute>();

            var colorInfo = EditorDrawUtility.GetColorInfo(mightyMember.Property, target, attribute.ColorName, attribute.Color);

            m_progressBarCache[mightyMember] = colorInfo;
        }

        public override void ClearCache() => m_progressBarCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_progressBarCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            m_progressBarCache[mightyMember].RefreshValue();
        }
    }
}
#endif