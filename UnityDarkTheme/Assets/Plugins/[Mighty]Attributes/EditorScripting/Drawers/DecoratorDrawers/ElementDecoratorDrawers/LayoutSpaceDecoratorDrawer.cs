#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(LayoutSpaceAttribute))]
    public class LayoutSpaceDecoratorDrawer : BaseElementDecoratorDrawer, IRefreshDrawer
    {
        private readonly MightyCache<MightyInfo<DecoratorPosition>> m_flexibleSpaceCache = new MightyCache<MightyInfo<DecoratorPosition>>();

        public override void BeginDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                BeginDraw(mightyMember, baseAttribute);

                propertyDrawCallback?.Invoke(mightyMember, property, drawerAttribute);
                return;
            }

            BeginDrawArray(mightyMember, baseAttribute);
            BeginDrawHeader(mightyMember, baseAttribute);

            if (!EditorDrawUtility.DrawFoldout(property))
            {
                EndDrawHeader(mightyMember, baseAttribute);
                EndDrawArray(mightyMember, baseAttribute);
                return;
            }

            EditorGUI.indentLevel++;
            EditorDrawUtility.DrawArraySizeField(property);

            EndDrawHeader(mightyMember, baseAttribute);
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                EndDraw(mightyMember, baseAttribute);
                return;
            }

            if (!property.isExpanded) return;

            EditorDrawUtility.DrawArrayBody(property, index =>
            {
                BeginDrawElement(mightyMember, index, baseAttribute);
                propertyDrawCallback?.Invoke(mightyMember, property.GetArrayElementAtIndex(index), drawerAttribute);
                EndDrawElement(mightyMember, index, baseAttribute);
            });

            EditorGUI.indentLevel--;

            EndDrawArray(mightyMember, baseAttribute);
        }

        public override void BeginDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.Before))
                DrawSpace(((LayoutSpaceAttribute) attribute).Size);
        }

        public override void EndDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.After))
                DrawSpace(((LayoutSpaceAttribute) attribute).Size);
        }

        public override void BeginDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.BeforeHeader))
                DrawSpace(((LayoutSpaceAttribute) attribute).Size);
        }

        public override void EndDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.AfterHeader))
                DrawSpace(((LayoutSpaceAttribute) attribute).Size);
        }

        public override Rect BeginDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            var position = PositionByMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.BeforeHeader))
                rect = DrawSpace(rect, ((LayoutSpaceAttribute) attribute).Size);

            return rect;
        }

        public override Rect EndDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.AfterHeader))
                rect = DrawSpace(rect, ((LayoutSpaceAttribute) attribute).Size);

            return rect;
        }

        public override void BeginDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var position = PositionByMember(mightyMember, attribute);
            var size = ((LayoutSpaceAttribute) attribute).Size;
            
            if (position.Contains(DecoratorPosition.BeforeElements))
                DrawSpace(size);

            if (index != 0 && position.Contains(DecoratorPosition.BetweenElements))
                DrawSpace(size);
        }

        public override void EndDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var position = PositionByMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.AfterElements))
                DrawSpace(((LayoutSpaceAttribute) attribute).Size);
        }

        public override Rect BeginDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var position = PositionByMember(mightyMember, attribute);
            var size = ((LayoutSpaceAttribute) attribute).Size;
            
            if (position.Contains(DecoratorPosition.BeforeElements))
                rect = DrawSpace(rect, size);

            if (index != 0 && position.Contains(DecoratorPosition.BetweenElements))
                rect = DrawSpace(rect, size);

            return rect;
        }

        public override Rect EndDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.AfterElements))
                rect = DrawSpace(rect, ((LayoutSpaceAttribute) attribute).Size);

            return rect;
        }

        private void BeginDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.Before))
                DrawSpace(((LayoutSpaceAttribute) attribute).Size);
        }

        private void EndDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.After))
                DrawSpace(((LayoutSpaceAttribute) attribute).Size);
        }

        public override float GetElementHeight(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute) => 
            ((LayoutSpaceAttribute) attribute).Size;

        public static void DrawSpace(float size) => GUILayout.Space(size);

        public static Rect DrawSpace(Rect rect, float size) => new Rect(rect.x, rect.y + size, rect.width, rect.height);

        public DecoratorPosition PositionByMember(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (!m_flexibleSpaceCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            return m_flexibleSpaceCache[mightyMember].Value;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (LayoutSpaceAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<LayoutSpaceAttribute>();
            var property = mightyMember.Property;

            if (!property.GetInfoFromMember<DecoratorPosition>(target, attribute.PositionName, out var positionInfo, Enum.TryParse))
                positionInfo = new MightyInfo<DecoratorPosition>(attribute.Position);

            m_flexibleSpaceCache[mightyMember] = positionInfo;
        }

        public override void ClearCache() => m_flexibleSpaceCache.ClearCache();
        
        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_flexibleSpaceCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            m_flexibleSpaceCache[mightyMember].RefreshValue();
        }
    }
}
#endif