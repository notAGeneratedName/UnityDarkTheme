#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(BoxGroupAttribute), typeof(BoxAttribute))]
    public class BoxGroupPropertyGrouper : BasePropertyGrouper
    {
        public override void BeginDrawGroup(string label = null, bool drawName = false, int indentLevel = 0)
        {
            GUILayout.BeginVertical(GUIStyleUtility.LightBox(indentLevel));
            EditorGUI.indentLevel = 1;
            if (drawName && !string.IsNullOrEmpty(label)) EditorGUILayout.LabelField(label, GUIStyleUtility.BoxGroupLabel);
        }

        public override void EndDrawGroup(int indentLevel = 0)
        {
            EditorGUI.indentLevel = indentLevel;
            GUILayout.EndVertical();
        }
    }

    [DrawerTarget(typeof(DarkBoxGroupAttribute), typeof(DarkBoxAttribute))]
    public class DarkBoxGroupPropertyGrouper : BasePropertyGrouper
    {
        public override void BeginDrawGroup(string label = null, bool drawName = false, int indentLevel = 0)
        {
            GUILayout.BeginVertical(GUIStyleUtility.DarkBox(indentLevel));
            EditorGUI.indentLevel = 1;
            if (drawName && !string.IsNullOrEmpty(label)) EditorGUILayout.LabelField(label, GUIStyleUtility.BoxGroupLabel);
        }

        public override void EndDrawGroup(int indentLevel = 0)
        {
            EditorGUI.indentLevel = indentLevel;
            GUILayout.EndVertical();
        }
    }
}
#endif