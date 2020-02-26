#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(LayerFieldAttribute))]
    public class LayerFieldPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawLayerField(property, baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawLayerField(mightyMember.GetElement(index), baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawLayerField(mightyMember.GetElement(index), baseAttribute, label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawLayerField(rect, mightyMember.GetElement(index), baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            mightyMember.GetElement(index).propertyType != SerializedPropertyType.Integer ? 64 : 20;

        public void DrawLayerField(SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" should be of type int");
                return;
            }

            int layer;
            if (attribute.Option.Contains(FieldOption.HideLabel))
                layer = EditorGUILayout.LayerField(property.intValue);
            else if (attribute.Option.Contains(FieldOption.BoldLabel))
                layer = EditorGUILayout.LayerField(label ?? EditorGUIUtility.TrTextContent(property.displayName), property.intValue,
                    EditorStyles.boldLabel);
            else
                layer = EditorGUILayout.LayerField(label ?? EditorGUIUtility.TrTextContent(property.displayName), property.intValue);

            property.intValue = layer;
        }

        public void DrawLayerField(Rect rect, SerializedProperty property, BaseDrawerAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorDrawUtility.DrawPropertyField(rect, property);
                rect.y += 18;
                rect.height -= 24;
                EditorDrawUtility.DrawHelpBox(rect, $"\"{property.displayName}\" should be of type int");
                return;
            }

            int layer;
            if (attribute.Option.Contains(FieldOption.HideLabel))
                layer = EditorGUI.LayerField(rect, property.intValue);
            else if (attribute.Option.Contains(FieldOption.BoldLabel))
                layer = EditorGUI.LayerField(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName), property.intValue,
                    EditorStyles.boldLabel);
            else
                layer = EditorGUI.LayerField(rect, label ?? EditorGUIUtility.TrTextContent(property.displayName), property.intValue);

            property.intValue = layer;
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