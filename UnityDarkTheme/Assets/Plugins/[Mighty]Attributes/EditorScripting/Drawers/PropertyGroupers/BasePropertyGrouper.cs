#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BasePropertyGrouper : BaseGlobalPropertyGrouper
    {
        public void BeginGroup(BaseMightyMember mightyMember, BaseGroupAttribute baseAttribute)
        {
            _previousBackgroundColor = GUI.backgroundColor;
            _previousContentColor = GUI.contentColor;

            if (!m_colorsCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (background, content) = m_colorsCache[mightyMember];

            GUI.backgroundColor = background.Value ?? _previousBackgroundColor;
            GUI.contentColor = content.Value ?? _previousContentColor;

            BeginDrawGroup(mightyMember.GroupName, baseAttribute.DrawName, IndentLevelForMember(mightyMember));

            if (baseAttribute.DrawLine) DrawLine(baseAttribute.LineColor);
        }

        public static void DrawLine(ColorValue color = ColorValue.Grey)
        {
            var previousColor = GUI.color;
            GUI.color = EditorDrawUtility.GetColor(color);
            GUILayout.Box(GUIContent.none, GUIStyleUtility.HorizontalLine(false));
            GUI.color = previousColor;
        }

        public void EndGroup(BaseMightyMember mightyMember)
        {
            EndDrawGroup(IndentLevelForMember(mightyMember));
            GUI.backgroundColor = _previousBackgroundColor;
            GUI.contentColor = _previousContentColor;
        }

        public abstract void BeginDrawGroup(string label = null, bool drawName = false, int indentLevel = 0);

        public abstract void EndDrawGroup(int indentLevel = 0);
    }
}
#endif