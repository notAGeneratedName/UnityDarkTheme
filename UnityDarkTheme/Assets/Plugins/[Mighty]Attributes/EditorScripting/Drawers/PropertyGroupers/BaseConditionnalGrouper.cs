#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BaseConditionalGrouper : BaseGlobalPropertyGrouper
    {
        public bool CanDraw(BaseMightyMember mightyMember, BaseConditionalGroupAttribute baseAttribute)
        {
            _previousBackgroundColor = GUI.backgroundColor;
            _previousContentColor = GUI.contentColor;

            if (!m_colorsCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (background, content) = m_colorsCache[mightyMember];

            GUI.backgroundColor = background.Value ?? _previousBackgroundColor;
            GUI.contentColor = content.Value ?? _previousContentColor;
            
            var canDraw = CanDrawImpl(mightyMember, baseAttribute.DrawName, IndentLevelForMember(mightyMember));

            return canDraw;
        }

        protected abstract bool CanDrawImpl(BaseMightyMember mightyMember, bool drawName, int indentLevel);

        public void BeginGroup(BaseMightyMember mightyMember)
        {
            BeginGroupImpl(IndentLevelForMember(mightyMember));
            GUI.backgroundColor = _previousBackgroundColor;
            GUI.contentColor = _previousContentColor;
        }

        public void EndGroup(BaseMightyMember mightyMember, bool canDraw)
        {
            if (canDraw) EndGroupImpl(IndentLevelForMember(mightyMember));
            GUI.backgroundColor = _previousBackgroundColor;
            GUI.contentColor = _previousContentColor;
        }

        protected abstract void BeginGroupImpl(int indentLevel);

        protected abstract void EndGroupImpl(int indentLevel);
    }
}
#endif