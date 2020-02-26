#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BaseGlobalPropertyGrouper : BaseMightyDrawer, IRefreshDrawer
    {
        protected readonly MightyCache<(MightyInfo<Color?>, MightyInfo<Color?>)> m_colorsCache =
            new MightyCache<(MightyInfo<Color?>, MightyInfo<Color?>)>();

        private readonly MightyCache<int> m_indentLevelCache = new MightyCache<int>();

        protected Color _previousBackgroundColor, _previousContentColor;

        public string GetName(BaseMightyMember mightyMember, BaseGlobalGroupAttribute baseAttribute)
        {
            var target = mightyMember.InitAttributeTarget<BaseGlobalGroupAttribute>();

            var name = baseAttribute.Name;
            if (baseAttribute.NameAsCallback) mightyMember.Property.GetValueFromMember(target, name, out name);
            return name ?? "";
        }

        protected int IndentLevelForMember(BaseMightyMember mightyMember)
        {
            if (m_indentLevelCache.TryGetValue(mightyMember, out var value)) return value;
            m_indentLevelCache[mightyMember] = EditorGUI.indentLevel;
            return EditorGUI.indentLevel;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (BaseGlobalGroupAttribute) mightyAttribute;
            var attributeTarget = mightyMember.InitAttributeTarget<BaseGlobalGroupAttribute>();
            var property = mightyMember.Property;

            var background =
                EditorDrawUtility.GetColorInfo(property, attributeTarget, attribute.BackgroundColorName, attribute.BackgroundColor);
            var content = EditorDrawUtility.GetColorInfo(property, attributeTarget, attribute.ContentColorName, attribute.ContentColor);

            m_colorsCache[mightyMember] = (background, content);
        }

        public override void ClearCache()
        {
            m_indentLevelCache.ClearCache();
            m_colorsCache.ClearCache();
        }

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_colorsCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (background, content) = m_colorsCache[mightyMember];
            background.RefreshValue();
            content.RefreshValue();
        }
    }
}
#endif