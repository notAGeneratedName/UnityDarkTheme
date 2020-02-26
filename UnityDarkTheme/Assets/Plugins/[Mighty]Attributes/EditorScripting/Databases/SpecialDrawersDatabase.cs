#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MightyAttributes.Editor
{
    public static class SpecialDrawersDatabase
    {
        private static readonly Dictionary<Type, BaseSpecialDrawer> DrawersByAttributeType;

        static SpecialDrawersDatabase()
        {
            DrawersByAttributeType = new Dictionary<Type, BaseSpecialDrawer>();

            foreach (var type in Assembly.GetAssembly(typeof(BaseSpecialDrawer)).GetTypes())
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(BaseSpecialDrawer))) continue;
                if (!(type.GetCustomAttributes(typeof(DrawerTargetAttribute), true) is DrawerTargetAttribute[] targetAttributes)) continue;

                foreach (var targetAttribute in targetAttributes)
                foreach (var targetType in targetAttribute.TargetAttributeTypes)
                    DrawersByAttributeType.Add(targetType, (BaseSpecialDrawer) Activator.CreateInstance(type));
            }
        }

        public static T GetDrawerForAttribute<T>(BaseSpecialAttribute attribute) where T : BaseSpecialDrawer =>
            GetDrawerForAttribute<T>(attribute.GetType());

        public static T GetDrawerForAttribute<T>(Type attributeType) where T : BaseSpecialDrawer
        {
            DrawersByAttributeType.TryGetValue(attributeType, out var drawer);
            return drawer as T;
        }

        public static IEnumerable<T> GetChildrenDrawers<T>() where T : BaseSpecialDrawer
        {
            foreach (var pair in DrawersByAttributeType)
                if (pair.Value is T drawer)
                    yield return drawer;
        }
    }
}
#endif