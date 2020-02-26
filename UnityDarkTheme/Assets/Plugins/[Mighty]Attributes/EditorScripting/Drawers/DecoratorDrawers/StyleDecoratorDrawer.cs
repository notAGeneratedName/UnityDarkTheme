#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(StyleAttribute))]
    public class StyleDecoratorDrawer : BaseDecoratorDrawer, IRefreshDrawer
    {
        private readonly MightyCache<MightyInfo<GUIStyle>> m_styleCache = new MightyCache<MightyInfo<GUIStyle>>();
        
        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            if (!m_styleCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var style = m_styleCache[mightyMember].Value;
            
            if (style == null) return;
            
            var attribute = (StyleAttribute) baseAttribute;

            GUILayout.BeginVertical(style);
            if (attribute.Indent) EditorGUI.indentLevel++;
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            if (!m_styleCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var style = m_styleCache[mightyMember].Value;
            if (style == null) return;
            
            var attribute = (StyleAttribute) baseAttribute;

            if (attribute.Indent) EditorGUI.indentLevel--;
            GUILayout.EndVertical();
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (StyleAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<StyleAttribute>();
            
            var styleInfo = EditorDrawUtility.GetStyleInfo(mightyMember.Property, target, attribute.StyleName,
                attribute.EditorStyle);

            m_styleCache[mightyMember] = styleInfo;
        }

        public override void ClearCache() => m_styleCache.ClearCache();
        
        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_styleCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            m_styleCache[mightyMember].RefreshValue();
        }
    }
}
#endif