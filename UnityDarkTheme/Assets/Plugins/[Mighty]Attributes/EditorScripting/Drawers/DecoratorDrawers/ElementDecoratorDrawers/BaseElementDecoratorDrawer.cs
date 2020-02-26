#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BaseElementDecoratorDrawer : BaseGlobalDecoratorDrawer
    {
        public abstract void BeginDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback, BaseDrawerAttribute drawerAttribute = null);

        public abstract void EndDraw(BaseMightyMember mightyMember, BaseElementDecoratorAttribute baseAttribute,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> propertyDrawCallback, BaseDrawerAttribute drawerAttribute = null);

        public abstract void BeginDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute);
        public abstract void EndDrawArray(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute);

        public abstract void BeginDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute);
        public abstract void EndDrawHeader(BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute);

        public abstract Rect BeginDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute);
        public abstract Rect EndDrawHeader(Rect rect, BaseMightyMember mightyMember, BaseElementDecoratorAttribute attribute);

        public abstract void BeginDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute);

        public abstract void EndDrawElement(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute);

        public abstract Rect BeginDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute);

        public abstract Rect EndDrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute);

        public abstract float GetElementHeight(BaseMightyMember mightyMember, int index, BaseElementDecoratorAttribute attribute);
    }
}
#endif