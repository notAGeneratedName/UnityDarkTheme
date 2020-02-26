#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ColorAttribute))]
    public class ColorDecoratorDrawer : BaseDecoratorDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(MightyInfo<Color?>, MightyInfo<Color?>)> m_colorCache =
            new MightyCache<(MightyInfo<Color?>, MightyInfo<Color?>)>();
        
        private Color _previousBackgroundColor, _previousContentColor;

        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            _previousBackgroundColor = GUI.backgroundColor;
            _previousContentColor = GUI.contentColor;

            if (!m_colorCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (background, content) = m_colorCache[mightyMember];

            GUI.backgroundColor = background.Value ?? _previousBackgroundColor;
            GUI.contentColor = content.Value ?? _previousContentColor;
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            GUI.backgroundColor = _previousBackgroundColor;
            GUI.contentColor = _previousContentColor;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (ColorAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<ColorAttribute>();

            var backgroundInfo = EditorDrawUtility.GetColorInfo(mightyMember.Property, target, attribute.BackgroundColorName,
                attribute.BackgroundColor);
            var contentInfo =
                EditorDrawUtility.GetColorInfo(mightyMember.Property, target, attribute.ContentColorName, attribute.ContentColor);

            m_colorCache[mightyMember] = (backgroundInfo, contentInfo);
        }

        public override void ClearCache() => m_colorCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_colorCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (background, content) = m_colorCache[mightyMember];
            background.RefreshValue();
            content.RefreshValue();
        }
    }
}
#endif