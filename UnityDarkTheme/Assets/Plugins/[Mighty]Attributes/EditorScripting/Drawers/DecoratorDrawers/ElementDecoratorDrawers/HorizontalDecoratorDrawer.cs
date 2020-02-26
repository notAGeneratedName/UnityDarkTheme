#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(BeginHorizontalAttribute), typeof(EndHorizontalAttribute), typeof(HorizontalAttribute))]
    public class HorizontalDecoratorDrawer : BaseElementDecoratorDrawer, IDrawAnywhereDecorator
    {
        public void BeginDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute)
        {
            if (attribute is BeginHorizontalAttribute || attribute is HorizontalAttribute)
                GUILayout.BeginHorizontal();
        }

        public void EndDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute)
        {
            if (attribute is EndHorizontalAttribute || attribute is HorizontalAttribute)
                GUILayout.EndHorizontal();
        }

        public override void BeginDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            var property = mightyMember.Property;
            if (!property.isArray || property.propertyType == SerializedPropertyType.String)
            {
                if (baseAttribute is BeginHorizontalAttribute || baseAttribute is HorizontalAttribute)
                    GUILayout.BeginHorizontal();

                propertyDrawCallback?.Invoke(mightyMember, property, drawerAttribute);
                return;
            }

            if (!EditorDrawUtility.DrawFoldout(property))
            {
                EndDrawHeader(mightyMember, baseAttribute);
                EndDrawArray(mightyMember, baseAttribute);
                return;
            }

            EditorGUI.indentLevel++;
            EditorDrawUtility.DrawArraySizeField(property);

            EditorDrawUtility.DrawArrayBody(property, index =>
            {
                BeginDrawElement(mightyMember, index, baseAttribute);
                propertyDrawCallback?.Invoke(mightyMember, property.GetArrayElementAtIndex(index), drawerAttribute);
                EndDrawElement(mightyMember, index, baseAttribute);
            });

            EditorGUI.indentLevel--;
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            if (!mightyMember.Property.isArray && baseAttribute is EndHorizontalAttribute || baseAttribute is HorizontalAttribute)
                GUILayout.EndHorizontal();
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
            if (attribute is BeginHorizontalAttribute || attribute is HorizontalAttribute)
                GUILayout.BeginHorizontal();
        }

        public override void EndDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            if (attribute is EndHorizontalAttribute || attribute is HorizontalAttribute)
                GUILayout.EndHorizontal();
        }

        public override Rect BeginDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            if (attribute is BeginHorizontalAttribute || attribute is HorizontalAttribute)
                GUILayout.BeginHorizontal();
            return rect;
        }

        public override Rect EndDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            if (attribute is EndHorizontalAttribute || attribute is HorizontalAttribute)
                GUILayout.EndHorizontal();
            return rect;
        }

        public override float GetElementHeight(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute) => 0;

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif