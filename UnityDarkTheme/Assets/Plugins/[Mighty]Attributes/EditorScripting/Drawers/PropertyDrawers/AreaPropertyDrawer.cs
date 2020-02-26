#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(AreaAttribute))]
    public class AreaPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        private static readonly float[] AreaWidths = {35, 40, 45, 27};

        private static readonly GUIContent[] AreaContent =
        {
            EditorGUIUtility.TrTextContent("Left"),
            EditorGUIUtility.TrTextContent("Right"),
            EditorGUIUtility.TrTextContent("Bottom"),
            EditorGUIUtility.TrTextContent("Top")
        };

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawArea(property, baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawArea(mightyMember.GetElement(index), baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 
            DrawArea(mightyMember.GetElement(index), baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawArea(rect, mightyMember.GetElement(index), baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 32;

        private void DrawArea(SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Vector4)
            {
                EditorDrawUtility.DrawHelpBox($"{property.name} should be of type UnityEngine.Vector4");
                return;
            }

            property.vector4Value = DrawArea(EditorGUILayout.GetControlRect(true, 32),
                label ?? EditorGUIUtility.TrTextContent(property.displayName), property.vector4Value, attribute);
        }

        private void DrawArea(Rect rect, SerializedProperty property, BaseDrawerAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Vector4)
            {
                EditorDrawUtility.DrawHelpBox(rect, $"{property.name} should be of type UnityEngine.Vector4");
                return;
            }

            property.vector4Value = DrawArea(rect, EditorGUIUtility.TrTextContent(property.displayName), property.vector4Value, attribute);
        }

        private Vector4 DrawArea(Rect position, GUIContent label, Vector4 area, BaseDrawerAttribute attribute)
        {
            var areaArray = area.Vector4ToArray();

            DrawLabel(ref position, null, attribute, label);

            EditorGUI.BeginChangeCheck();
            EditorDrawUtility.MultiFloatField(position, AreaContent, areaArray, AreaWidths, Orientation.Vertical);
            if (EditorGUI.EndChangeCheck())
                area = areaArray.ArrayToVector4();
            
            return area;
        }

        protected override bool DrawLabel(ref Rect rect, SerializedProperty property, BaseDrawerAttribute baseAttribute, GUIContent label)
        {
            if (baseAttribute.Option.Contains(FieldOption.HideLabel)) return false;

            rect = EditorDrawUtility.MultiFieldPrefixLabel(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName),
                AreaContent.Length,
                baseAttribute.Option.Contains(FieldOption.BoldLabel) ? EditorStyles.whiteBoldLabel : EditorStyles.whiteLabel);

            return true;
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