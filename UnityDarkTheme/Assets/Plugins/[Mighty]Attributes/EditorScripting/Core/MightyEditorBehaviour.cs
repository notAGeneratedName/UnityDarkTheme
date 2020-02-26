#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    [ExecuteInEditMode]
    public class MightyEditorBehaviour : MonoBehaviour
    {
        private static MightyEditorBehaviour m_instance;

        public static MightyEditorBehaviour Instance
        {
            get
            {
                if (m_instance == null) m_instance = SerializedPropertyUtility.FindFirstObject<MightyEditorBehaviour>();
                if (m_instance != null) return m_instance;

                var go = new GameObject("Mighty Editor Behaviour", typeof(MightyEditorBehaviour))
                {
                    hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy
                };
                m_instance = go.GetComponent<MightyEditorBehaviour>();

                return m_instance;
            }
        }

        private void Update() => hideFlags = HideFlags.None;
    }
}
#endif