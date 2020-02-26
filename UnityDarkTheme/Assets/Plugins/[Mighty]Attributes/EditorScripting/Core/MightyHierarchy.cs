#if UNITY_EDITOR
using UnityEditor.Callbacks;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public static class MightyHierarchy
    {
        private static readonly Dictionary<(int, BaseHierarchyAttribute), BaseHierarchyDrawer> DrawersByAttribute =
            new Dictionary<(int, BaseHierarchyAttribute), BaseHierarchyDrawer>();

        [DidReloadScripts]
        private static void Init()
        {
            HierarchyChanged();
            
            EditorApplication.hierarchyChanged += HierarchyChanged;
            EditorApplication.hierarchyWindowItemOnGUI += ItemOnGUI;
        }

        private static void HierarchyChanged()
        {
            foreach (var monoBehaviour in SerializedPropertyUtility.FindAllObjects<MonoBehaviour>())
            {
                if (monoBehaviour == null) continue;
                var attributes = monoBehaviour.GetType().GetCustomAttributes(typeof(BaseHierarchyAttribute), true);

                if (attributes.Length == 0) continue;

                var instanceID = monoBehaviour.gameObject.GetInstanceID();

                foreach (BaseHierarchyAttribute attribute in attributes)
                {
                    if (!DrawersByAttribute.ContainsKey((instanceID, attribute)))
                        DrawersByAttribute.Add((instanceID, attribute),
                            SpecialDrawersDatabase.GetDrawerForAttribute<BaseHierarchyDrawer>(attribute));

                    DrawersByAttribute[(instanceID, attribute)].Update(monoBehaviour, attribute);
                }
            }
        }

        private static void ItemOnGUI(int instanceID, Rect selectionRect)
        {
            var items = DrawersByAttribute.Where(x => x.Key.Item1 == instanceID).ToArray();
            if (items.Length == 0) return;

            foreach (var item in items)
                item.Value.OnGUI(instanceID, selectionRect, item.Key.Item2);
        }
    }
}
#endif