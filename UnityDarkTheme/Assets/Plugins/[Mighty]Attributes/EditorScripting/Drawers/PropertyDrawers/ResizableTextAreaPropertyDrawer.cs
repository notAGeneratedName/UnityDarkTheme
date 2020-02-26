#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ResizableTextAreaAttribute))]
    public class ResizableTextAreaPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        private float Height(SerializedProperty property)
        {
            var linesCount = property.stringValue.Split('\n').Length;
            return (linesCount > 3 ? linesCount : 3) * 13 + 3;
        }

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawTextArea(property, (ResizableTextAreaAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawTextArea(mightyMember.GetElement(index), (ResizableTextAreaAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawTextArea(mightyMember.GetElement(index), (ResizableTextAreaAttribute) baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawTextArea(rect, mightyMember.GetElement(index), (ResizableTextAreaAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            var element = mightyMember.GetElement(index);
            return element.propertyType != SerializedPropertyType.String ? 64 : Height(element) + 24;
        }

        public void DrawTextArea(SerializedProperty property, ResizableTextAreaAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                DrawLabel(property, attribute, label);

                EditorGUI.BeginChangeCheck();

                var textAreaValue = EditorGUILayout.TextArea(property.stringValue, GUILayout.MinHeight(Height(property)));

                if (EditorGUI.EndChangeCheck()) property.stringValue = textAreaValue;
            }
            else
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"{typeof(ResizableTextAreaAttribute).Name} can only be used on string fields");
            }
        }

        public void DrawTextArea(Rect rect, SerializedProperty property, ResizableTextAreaAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                DrawLabel(ref rect, property, attribute, label);

                EditorGUI.BeginChangeCheck();

                var textAreaValue = EditorGUI.TextArea(new Rect(rect.x, rect.y + 18, rect.width, Height(property)), property.stringValue);

                if (EditorGUI.EndChangeCheck()) property.stringValue = textAreaValue;
            }
            else
            {
                EditorDrawUtility.DrawPropertyField(rect, property);
                rect.y += 18;
                rect.height -= 24;
                EditorDrawUtility.DrawHelpBox(rect, $"{typeof(ResizableTextAreaAttribute).Name} can only be used on string fields");
            }
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif