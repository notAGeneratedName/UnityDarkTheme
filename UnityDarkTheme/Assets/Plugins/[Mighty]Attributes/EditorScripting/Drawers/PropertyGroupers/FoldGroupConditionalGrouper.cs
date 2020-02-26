#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FoldGroupAttribute), typeof(FoldAttribute))]
    public class FoldGroupConditionalGrouper : BaseConditionalGrouper
    {
        private readonly MightyCache<string, bool> m_foldoutStateCache = new MightyCache<string, bool>();

        protected override bool CanDrawImpl(BaseMightyMember mightyMember, bool drawName, int indentLevel) =>
            m_foldoutStateCache[mightyMember, mightyMember.GroupID] =
                BeginFoldout(FoldoutStateByProperty(mightyMember), drawName ? mightyMember.GroupName : "", indentLevel);

        protected override void BeginGroupImpl(int indentLevel) => DrawBody();

        protected override void EndGroupImpl(int indentLevel) => EndFoldout(indentLevel);

        public bool BeginFoldout(bool foldout, string label, int indentLevel)
        {
            GUILayout.BeginVertical(GUIStyleUtility.FoldGroupHeader(indentLevel));
            EditorGUI.indentLevel = 1;

            GUILayout.BeginVertical(GUIStyleUtility.FoldGroupHeaderContent);
            foldout = EditorDrawUtility.DrawFoldout(foldout, label, GUIStyleUtility.BoldFoldout);
            GUILayout.EndVertical();

            if (foldout) return true;
            EditorGUI.indentLevel = indentLevel;
            GUILayout.EndVertical();
            return false;
        }

        public void DrawBody() => EditorGUILayout.BeginVertical(GUIStyleUtility.FoldGroupBody);

        public void EndFoldout(int indentLevel)
        {
            EditorGUI.indentLevel = indentLevel;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private bool FoldoutStateByProperty(BaseMightyMember mightyMember)
        {
            if (m_foldoutStateCache.TryGetValue(mightyMember, mightyMember.GroupID, out var value)) return value;

            m_foldoutStateCache[mightyMember, mightyMember.GroupID] = false;
            return false;
        }
    }
}
#endif