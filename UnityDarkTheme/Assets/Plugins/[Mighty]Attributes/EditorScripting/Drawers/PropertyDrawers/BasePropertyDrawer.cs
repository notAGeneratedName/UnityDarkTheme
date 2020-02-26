#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BasePropertyDrawer : BaseMightyDrawer
    {
        public abstract void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute);

        protected virtual bool DrawLabel(SerializedProperty property, BaseDrawerAttribute baseAttribute, GUIContent label)
        {
            if (baseAttribute.Option.Contains(FieldOption.HideLabel)) return false;

            if (baseAttribute.Option.Contains(FieldOption.BoldLabel))
                EditorGUILayout.LabelField(label ?? EditorGUIUtility.TrTextContent(property.displayName), EditorStyles.boldLabel);
            else
                EditorGUILayout.LabelField(label ?? EditorGUIUtility.TrTextContent(property.displayName));

            return true;
        }

        protected virtual bool DrawLabel(ref Rect rect, SerializedProperty property, BaseDrawerAttribute baseAttribute, GUIContent label)
        {
            if (baseAttribute.Option.Contains(FieldOption.HideLabel)) return false;

            if (baseAttribute.Option.Contains(FieldOption.BoldLabel))
                EditorGUI.LabelField(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName), EditorStyles.boldLabel);
            else
                EditorGUI.LabelField(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName));

            return true;
        }
    }
}
#endif