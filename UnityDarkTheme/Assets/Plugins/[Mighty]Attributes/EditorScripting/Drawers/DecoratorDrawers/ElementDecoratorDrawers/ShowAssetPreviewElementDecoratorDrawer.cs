#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ShowAssetPreviewAttribute), typeof(AssetPreviewAttribute))]
    public class ShowAssetPreviewElementDecoratorDrawer : BaseElementDecoratorDrawer
    {
        private Color m_previousBackgroundColor, m_previousContentColor;

        public override void BeginDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                propertyDrawCallback?.Invoke(mightyMember, property, drawerAttribute);
                return;
            }

            if (!EditorDrawUtility.DrawFoldout(property)) return;

            EditorGUI.indentLevel++;
            EditorDrawUtility.DrawArraySizeField(property);
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                DrawPreview(property, (ShowAssetPreviewAttribute) baseAttribute);
                return;
            }

            if (!property.isExpanded) return;

            EditorDrawUtility.DrawArrayBody(property, index =>
            {
                propertyDrawCallback?.Invoke(mightyMember, property.GetArrayElementAtIndex(index), drawerAttribute);
                EndDrawElement(mightyMember, index, baseAttribute);
            });

            EditorGUI.indentLevel--;
        }

        public override void BeginDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
        }

        public override void EndDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
        }

        public override void BeginDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
        }

        public override void EndDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
        }

        public override Rect BeginDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute) => rect;

        public override Rect EndDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute) => rect;

        public override void BeginDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
        }

        public override void EndDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute) => 
            DrawPreview(mightyMember.GetElement(index), (ShowAssetPreviewAttribute) attribute);

        public override Rect BeginDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
            => rect;

        public override Rect EndDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute) => 
            DrawPreview(rect, mightyMember.GetElement(index), (ShowAssetPreviewAttribute) attribute);

        public override float GetElementHeight(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var element = mightyMember.GetElement(index);
            if (element.propertyType != SerializedPropertyType.ObjectReference || element.objectReferenceValue == null) return 50;

            var previewTexture = AssetPreview.GetAssetPreview(element.objectReferenceValue);

            if (previewTexture == null) return 50;

            var showAssetAttribute = (ShowAssetPreviewAttribute) attribute;

            var textureRatio = (float) previewTexture.width / previewTexture.height;
            return (int) Mathf.Clamp(showAssetAttribute.Size / textureRatio, 0, previewTexture.height) + 15;
        }

        private void DrawPreview(SerializedProperty property, ShowAssetPreviewAttribute attribute)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                if (previewTexture != null)
                {
                    var width = Mathf.Clamp(attribute.Size, 0, previewTexture.width);
                    var height = Mathf.Clamp(attribute.Size, 0, previewTexture.height);

                    GUILayout.BeginVertical();
                    EditorDrawUtility.DrawWithAlign(attribute.Align, () =>
                    {
                        if (previewTexture != null)
                            GUILayout.Label(previewTexture, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height));
                    });
                    GUILayout.EndVertical();
                }
                else
                    EditorDrawUtility.DrawHelpBox($"{property.name} doesn't have an asset preview");
            }
            else
                EditorDrawUtility.DrawHelpBox($"{property.name} doesn't have an asset preview");
        }  
        
        private Rect DrawPreview(Rect rect, SerializedProperty property, ShowAssetPreviewAttribute attribute)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                if (previewTexture != null)
                {
                    var textureRatio = (float) previewTexture.width / previewTexture.height;

                    var width = Mathf.Clamp(attribute.Size, 0, previewTexture.width);
                    var height = Mathf.Clamp((int) (attribute.Size / textureRatio), 0, previewTexture.height);

                    switch (attribute.Align)
                    {
                        case Align.Left:
                            rect = new Rect(rect.x + 5, rect.y + 5, width, height);
                            break;
                        case Align.Center:
                            rect = new Rect(Screen.width / 2f - width / 2f, rect.y + 5, width, height);
                            break;
                        case Align.Right:
                            rect = new Rect(Screen.width - width - 29, rect.y + 5, width, height);
                            break;
                    }

                    GUI.Label(new Rect(rect.x, rect.y, rect.width, rect.height), previewTexture);
                }
                else
                    EditorDrawUtility.DrawHelpBox(new Rect(rect.x, rect.y, rect.width, 40),
                        $"{property.name} doesn't have an asset preview");
            }
            else
                EditorDrawUtility.DrawHelpBox(new Rect(rect.x, rect.y, rect.width, 40), $"{property.name} doesn't have an asset preview");

            return rect;
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