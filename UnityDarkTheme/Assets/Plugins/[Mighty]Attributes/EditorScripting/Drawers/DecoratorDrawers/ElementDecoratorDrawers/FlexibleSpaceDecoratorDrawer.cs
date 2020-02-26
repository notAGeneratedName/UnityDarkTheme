#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(FlexibleSpaceAttribute))]
    public class FlexibleSpaceDecoratorDrawer : BaseElementDecoratorDrawer, IRefreshDrawer
    {
        private readonly MightyCache<MightyInfo<DecoratorPosition>> m_flexibleSpaceCache = new MightyCache<MightyInfo<DecoratorPosition>>();

        public override void BeginDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                if (PositionByMember(mightyMember, baseAttribute).Contains(DecoratorPosition.Before))
                    DrawSpace();

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
                if (PositionByMember(mightyMember, baseAttribute).Contains(DecoratorPosition.After))
                    DrawSpace();
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
                DrawSpace();
        }

        public override void EndDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.After))
                DrawSpace();
        }

        public override void BeginDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.BeforeHeader))
                DrawSpace();
        }

        public override void EndDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.AfterHeader))
                DrawSpace();
        }

        public override Rect BeginDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute) => rect;

        public override Rect EndDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute) => rect;

        public override void BeginDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var position = PositionByMember(mightyMember, attribute);

            if (position.Contains(DecoratorPosition.BeforeElements))
                DrawSpace();

            if (index != 0 && position.Contains(DecoratorPosition.BetweenElements))
                DrawSpace();
        }

        public override void EndDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            if (PositionByMember(mightyMember, attribute).Contains(DecoratorPosition.AfterElements))
                DrawSpace();
        }

        public override Rect BeginDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
            => rect;

        public override Rect EndDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
            => rect;

        public override float GetElementHeight(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute) => 0;

        public void DrawSpace() => GUILayout.FlexibleSpace();

        private DecoratorPosition PositionByMember(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute)
        {
            if (!m_flexibleSpaceCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            return m_flexibleSpaceCache[mightyMember].Value;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var property = mightyMember.Property;
            var target = mightyMember.InitAttributeTarget<FlexibleSpaceAttribute>();
            var attribute = (FlexibleSpaceAttribute) mightyAttribute;

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