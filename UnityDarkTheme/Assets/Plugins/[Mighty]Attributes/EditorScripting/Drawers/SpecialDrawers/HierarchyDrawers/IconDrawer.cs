#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(IconAttribute))]
    public class IconDrawer : BaseHierarchyDrawer
    {
        private readonly MightyCache<(bool, Texture2D)> m_iconsCache = new MightyCache<(bool, Texture2D)>();

        protected override void UpdateImpl(MonoBehaviour monoBehaviour, BaseHierarchyAttribute baseAttribute)
        {
            var instanceID = monoBehaviour.gameObject.GetInstanceID();
            var hasChildren = monoBehaviour.transform.childCount != 0;

            if (GetTexture((IconAttribute) baseAttribute) is Texture2D texture)
                m_iconsCache[instanceID] = (hasChildren, texture);
        }

        public override void OnGUI(int instanceID, Rect selectionRect, BaseHierarchyAttribute baseAttribute)
        {
            if (!m_iconsCache.Contains(instanceID)) return;
            
            var (hasChildren, texture2D) = m_iconsCache[instanceID];
            selectionRect.x -= hasChildren ? 30 : 16;
            GUI.Label(selectionRect, texture2D);
        }

        private Texture2D GetTexture(IconAttribute attribute) => EditorDrawUtility.GetTexture(attribute.IconPath);
    }
}
#endif