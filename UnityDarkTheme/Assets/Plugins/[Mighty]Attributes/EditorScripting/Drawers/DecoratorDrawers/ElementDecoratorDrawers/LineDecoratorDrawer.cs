#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(LineAttribute))]
    public class LineDecoratorDrawer : BaseElementDecoratorDrawer, IDrawAnywhereDecorator, IRefreshDrawer
    {
        private readonly MightyCache<(MightyInfo<DecoratorPosition>, MightyInfo<Color?>)> m_lineCache =
            new MightyCache<(MightyInfo<DecoratorPosition>, MightyInfo<Color?>)>();

        private Color m_previousColor;

        public void BeginDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.ContainsExact(DecoratorPosition.Anywhere | DecoratorPosition.Before))
                DrawLine(color);
        }

        public void EndDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.ContainsExact(DecoratorPosition.Anywhere | DecoratorPosition.After))
                DrawLine(color);
        }

        public override void BeginDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback,
            BaseDrawerAttribute drawerAttribute = null)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                m_previousColor = GUI.color;
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
                m_previousColor = GUI.color;
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
            m_previousColor = GUI.color;
            BeginDraw(mightyMember, attribute);
        }

        public override void EndDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute) =>
            EndDraw(mightyMember, attribute);

        public override void BeginDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.BeforeHeader))
                DrawLine(color);
        }

        public override void EndDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.AfterHeader))
                DrawLine(color);
        }

        public override Rect BeginDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.BeforeHeader))
                rect = DrawLine(rect, color);

            return rect;
        }

        public override Rect EndDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.AfterHeader))
                rect = DrawLine(rect, color);

            return rect;
        }

        public override void BeginDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.BeforeElements))
                DrawLine(color);

            if (index != 0 && position.Contains(DecoratorPosition.BetweenElements))
                DrawLine(color);
        }

        public override void EndDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.AfterElements))
                DrawLine(color);
        }

        public override Rect BeginDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.BeforeElements))
                rect = DrawLine(rect, color);

            if (index != 0 && position.Contains(DecoratorPosition.BetweenElements))
                rect = DrawLine(rect, color);

            return rect;
        }

        public override Rect EndDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.AfterElements))
                rect = DrawLine(rect, color);

            return rect;
        }

        private void BeginDraw(BaseMightyMember mightyMember, BaseMightyAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.Before))
                DrawLine(color);
        }

        private void EndDraw(BaseMightyMember mightyMember, BaseMightyAttribute attribute)
        {
            var (position, color) = PositionAndColorForMember(mightyMember, attribute);
            if (position.Contains(DecoratorPosition.After))
                DrawLine(color);
        }

        public override float GetElementHeight(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute)
        {
            var (position, _) = PositionAndColorForMember(mightyMember, attribute);
            return position.Contains(DecoratorPosition.BeforeElements | DecoratorPosition.AfterElements) ? 10 : 0;
        }

        public void DrawLine(Color? color = null)
        {
            GUI.color = color ?? m_previousColor;
            GUILayout.Box(GUIContent.none, GUIStyleUtility.HorizontalLine(true));
            GUI.color = m_previousColor;
        }

        public Rect DrawLine(Rect rect, Color? color = null)
        {
            GUI.color = color ?? m_previousColor;

            var lineStyle = GUIStyleUtility.HorizontalLine(true);
            GUI.Box(rect, GUIContent.none, lineStyle);
            rect.y += lineStyle.margin.top + lineStyle.margin.bottom;

            GUI.color = m_previousColor;
            return rect;
        }

        private (DecoratorPosition, Color?) PositionAndColorForMember(BaseMightyMember mightyMember, BaseMightyAttribute attribute)
        {
            if (!m_lineCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (positionInfo, colorInfo) = m_lineCache[mightyMember];
            return (positionInfo.Value, colorInfo.Value);
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (LineAttribute) mightyAttribute;

            var property = mightyMember.Property;
            if (property != null)
            {
                var target = mightyMember.InitAttributeTarget<LineAttribute>();

                if (!property.GetInfoFromMember<DecoratorPosition>(target, attribute.PositionName, out var positionInfo, Enum.TryParse))
                    positionInfo = new MightyInfo<DecoratorPosition>(attribute.Position);

                var colorInfo = EditorDrawUtility.GetColorInfo(property, target, attribute.ColorName, attribute.Color);

                m_lineCache[mightyMember] = (positionInfo, colorInfo);
            }
            else
            {
                var target = mightyMember.Target;
                if (!target.GetInfoFromMember<DecoratorPosition>(attribute.PositionName, out var positionInfo, Enum.TryParse))
                    positionInfo = new MightyInfo<DecoratorPosition>(attribute.Position);

                var colorInfo = EditorDrawUtility.GetColorInfo(target, attribute.ColorName, attribute.Color);

                m_lineCache[mightyMember] = (positionInfo, colorInfo);
            }
        }

        public override void ClearCache() => m_lineCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_lineCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (positionInfo, colorInfo) = m_lineCache[mightyMember];

            positionInfo.RefreshValue();
            colorInfo.RefreshValue();
        }
    }
}
#endif