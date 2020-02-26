#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(EulerAttribute))]
    public class EulerPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawEuler(property, baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawEuler(mightyMember.GetElement(index), baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawEuler(mightyMember.GetElement(index), baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawEuler(rect, mightyMember.GetElement(index), baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 16;

        private static void DrawEuler(SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Quaternion)
            {
                EditorDrawUtility.DrawHelpBox($"{property.name} should be of type Quaternion");
                return;
            }

            if (attribute.Option.Contains(FieldOption.HideLabel))
                EditorDrawUtility.DrawRotationEuler(GUIContent.none, property);
            else
                EditorDrawUtility.DrawRotationEuler(label ?? EditorGUIUtility.TrTextContent(property.displayName), property);
        }      
        
        private static void DrawEuler(Rect position, SerializedProperty property, BaseDrawerAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Quaternion)
            {
                EditorDrawUtility.DrawHelpBox(position, $"{property.name} should be of type Quaternion");
                return;
            }

            if (attribute.Option.Contains(FieldOption.HideLabel))
                EditorDrawUtility.DrawRotationEuler(position, GUIContent.none, property);
            else
                EditorDrawUtility.DrawRotationEuler(position, property);
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