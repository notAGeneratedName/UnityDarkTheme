#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MightyAttributes.Editor
{
    public static class DrawersDatabase
    {
        private static readonly Dictionary<Type, BaseMightyDrawer> DrawersByAttributeType;

        static DrawersDatabase()
        {
            DrawersByAttributeType = new Dictionary<Type, BaseMightyDrawer>();

            foreach (var type in Assembly.GetAssembly(typeof(BaseMightyDrawer)).GetTypes())
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(BaseMightyDrawer))) continue;
                if (!(type.GetCustomAttributes(typeof(DrawerTargetAttribute), true) is DrawerTargetAttribute[] targetAttributes)) continue;

                foreach (var targetAttribute in targetAttributes)
                foreach (var targetType in targetAttribute.TargetAttributeTypes)
                    DrawersByAttributeType.Add(targetType, (BaseMightyDrawer) Activator.CreateInstance(type));
            }
        }

        public static T GetDrawerForAttribute<T>(BaseMightyAttribute attribute) where T : BaseMightyDrawer =>
            GetDrawerForAttribute<T>(attribute.GetType());

        public static T GetDrawerForAttribute<T>(Type attributeType) where T : BaseMightyDrawer
        {
            DrawersByAttributeType.TryGetValue(attributeType, out var drawer);
            return drawer as T;
        }

        public static void ClearCache()
        {
            foreach (var mightyDrawer in DrawersByAttributeType.Values) mightyDrawer.ClearCache();
        }
    }
}
#endif