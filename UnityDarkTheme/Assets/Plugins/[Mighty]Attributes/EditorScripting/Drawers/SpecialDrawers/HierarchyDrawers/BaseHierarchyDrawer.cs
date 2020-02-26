#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BaseHierarchyDrawer : BaseSpecialDrawer
    {
        private readonly MightyCache<int> m_priorityCache = new MightyCache<int>();

        public void Update(MonoBehaviour monoBehaviour, BaseHierarchyAttribute baseAttribute)
        {
            var instanceID = monoBehaviour.gameObject.GetInstanceID();
            if (m_priorityCache.TryGetValue(instanceID, out var priority) && priority <= baseAttribute.Priority) return;

            m_priorityCache[instanceID] = baseAttribute.Priority;
            UpdateImpl(monoBehaviour, baseAttribute);
        }
        
        protected abstract void UpdateImpl(MonoBehaviour monoBehaviour, BaseHierarchyAttribute baseAttribute);

        public abstract void OnGUI(int instanceID, Rect selectionRect, BaseHierarchyAttribute baseAttribute);
    }
}
#endif