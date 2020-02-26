#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(LabelAttribute))]
    public class LabelDecoratorDrawer : BaseDecoratorDrawer, IDrawAnywhereDecorator, IRefreshDrawer
    {
        private readonly MightyCache<(MightyInfo<string>, MightyInfo<string>, MightyInfo<GUIStyle>)> m_labelCache =
            new MightyCache<(MightyInfo<string>, MightyInfo<string>, MightyInfo<GUIStyle>)>();

        public void BeginDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute)
        {
            var labelAttribute = (LabelAttribute) attribute;
            if (labelAttribute.Position.Contains(FieldPosition.Anywhere))
                BeginDraw(mightyMember, labelAttribute);
        }

        public void EndDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute)
        {
            var labelAttribute = (LabelAttribute) attribute;
            if (labelAttribute.Position.Contains(FieldPosition.Anywhere))
                EndDraw(mightyMember, labelAttribute);
        }

        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute attribute)
        {
            var labelAttribute = (LabelAttribute) attribute;

            if (labelAttribute.Position.Contains(FieldPosition.Horizontal)) GUILayout.BeginHorizontal();

            if (labelAttribute.Position.Contains(FieldPosition.Before)) DrawLabel(mightyMember, labelAttribute);
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute attribute)
        {
            var labelAttribute = (LabelAttribute) attribute;

            if (labelAttribute.Position.Contains(FieldPosition.After)) DrawLabel(mightyMember, labelAttribute);

            if (labelAttribute.Position.Contains(FieldPosition.Horizontal)) GUILayout.EndHorizontal();
        }

        public void DrawLabel(BaseMightyMember mightyMember, LabelAttribute attribute)
        {
            if (!m_labelCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (labelInfo, prefixInfo, styleInfo) = m_labelCache[mightyMember];

            var label = labelInfo.Value;
            var prefix = prefixInfo.Value;
            var style = styleInfo.Value;

            if (string.IsNullOrEmpty(prefix))
            {
                if (style == null)
                    EditorGUILayout.LabelField(label, GUILayout.Width(EditorDrawUtility.TextWidth(label)));
                else
                    EditorGUILayout.LabelField(label, style, GUILayout.Width(EditorDrawUtility.TextWidth(label)));
            }
            else
            {
                if (style == null)
                    EditorGUILayout.LabelField(prefix, label);
                else
                    EditorGUILayout.LabelField(prefix, label, style);
            }
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (LabelAttribute) mightyAttribute;
            var property = mightyMember.Property;
            if (property != null)
            {
                var target = mightyMember.InitAttributeTarget<LabelAttribute>();

                if (!property.GetInfoFromMember<string>(target, attribute.CallbackName, out var labelInfo))
                    labelInfo = new MightyInfo<string>(attribute.CallbackName);

                if (!attribute.PrefixAsCallback || !property.GetInfoFromMember<string>(target, attribute.Prefix, out var prefixInfo))
                    prefixInfo = new MightyInfo<string>(attribute.Prefix);

                var styleInfo = EditorDrawUtility.GetStyleInfo(property, target, attribute.StyleName, attribute.EditorStyle);

                m_labelCache[mightyMember] = (labelInfo, prefixInfo, styleInfo);
            }
            else
            {
                var target = mightyMember.Target;

                if (!target.GetInfoFromMember<string>(attribute.CallbackName, out var labelInfo))
                    labelInfo = new MightyInfo<string>(attribute.CallbackName);

                if (!attribute.PrefixAsCallback || !target.GetInfoFromMember<string>(attribute.Prefix, out var prefixInfo))
                    prefixInfo = new MightyInfo<string>(attribute.Prefix);

                var styleInfo = EditorDrawUtility.GetStyleInfo(target, attribute.StyleName, attribute.EditorStyle);

                m_labelCache[mightyMember] = (labelInfo, prefixInfo, styleInfo);
            }
        }

        public override void ClearCache() => m_labelCache.ClearCache();
        
        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_labelCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }
            var (labelInfo, prefixInfo, styleInfo) = m_labelCache[mightyMember];
            labelInfo.RefreshValue();
            prefixInfo.RefreshValue();
            styleInfo.RefreshValue();
        }
    }
}
#endif