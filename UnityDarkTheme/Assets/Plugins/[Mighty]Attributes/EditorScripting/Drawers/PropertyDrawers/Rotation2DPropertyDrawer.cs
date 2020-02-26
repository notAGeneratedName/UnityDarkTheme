#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(Rotation2DAttribute))]
    public class Rotation2DPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }
            
            DrawRotation2D(property, baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawRotation2D(mightyMember.GetElement(index), baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawRotation2D(mightyMember.GetElement(index), baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawRotation2D(rect, mightyMember.GetElement(index), baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 16;

        private void DrawRotation2D(SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Quaternion)
            {
                EditorDrawUtility.DrawHelpBox($"{property.name} should be of type Quaternion");
                return;
            }

            var quaternion = property.quaternionValue;
            var angles = quaternion.eulerAngles;

            angles.z = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUILayout.Slider(angles.z, 0, 359.9f)
                : EditorGUILayout.Slider(label ?? EditorGUIUtility.TrTextContent(property.displayName), angles.z, 0, 359.9f);

            quaternion.eulerAngles = angles;
            property.quaternionValue = quaternion;
        }

        private void DrawRotation2D(Rect rect, SerializedProperty property, BaseDrawerAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Quaternion)
            {
                EditorDrawUtility.DrawHelpBox(rect, $"{property.name} should be of type Quaternion");
                return;
            }

            var quaternion = property.quaternionValue;
            var angles = quaternion.eulerAngles;

            angles.z = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUI.Slider(rect, angles.z, 0, 359.9f)
                : EditorGUI.Slider(rect, property.displayName, angles.z, 0, 359.9f);

            quaternion.eulerAngles = angles;
            property.quaternionValue = quaternion;
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