#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(AssetOnlyAttribute))]
    public class AssetOnlyPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute) =>
            DrawAssetField(property, baseAttribute);

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAssetField(mightyMember.GetElement(index), baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAssetField(mightyMember.GetElement(index), baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAssetField(rect, mightyMember.GetElement(index), baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 64;

        private void DrawAssetField(SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorDrawUtility.DrawHelpBox($"{property.name} should be of type UnityEngine.Object");
                return;
            }

            property.objectReferenceValue = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUILayout.ObjectField(property.objectReferenceValue, property.GetSystemType(), false)
                : EditorGUILayout.ObjectField(label ?? EditorGUIUtility.TrTextContent(property.displayName),
                    property.objectReferenceValue, property.GetSystemType(), false);
        }

        private void DrawAssetField(Rect rect, SerializedProperty property, BaseDrawerAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorDrawUtility.DrawHelpBox($"{property.name} should be of type UnityEngine.Object");
                return;
            }


            property.objectReferenceValue = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUI.ObjectField(rect, property.objectReferenceValue, property.GetSystemType(), false)
                : EditorGUI.ObjectField(rect, property.displayName, property.objectReferenceValue, property.GetSystemType(), false);
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