#if UNITY_EDITOR
using UnityEditor;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class MightyEditor : UnityEditor.Editor
    {
        private MightyDrawer m_drawer;

        public void ApplyAutoValues()
        {
            if (m_drawer == null) Enable();
            m_drawer.ApplyAutoValues(false);
            Disable();
        }

        #region Unity Events

        private void OnEnable()
        {
            if (MightySettingsServices.Activated) Enable();
        }

        private void OnDisable()
        {
            if (MightySettingsServices.Activated) Disable();
        }

        public override void OnInspectorGUI()
        {
            if (MightySettingsServices.Activated) InspectorGUI();
            else base.OnInspectorGUI();
        }

        #endregion /Unity Events

        private void Enable()
        {
            m_drawer = new MightyDrawer();
            try
            {
                m_drawer.OnEnableMonoScript(target, serializedObject);
            }
            catch
            {
                // ignored
            }
        }

        private void Disable()
        {
            m_drawer?.OnDisable();
            m_drawer = null;
            DrawersDatabase.ClearCache();
        }

        private void InspectorGUI()
        {
            if (m_drawer == null) Enable();
            var context = target;
            var serialized = serializedObject;

            m_drawer.BeginOnGUI(context);
            if (!m_drawer.DrawSerializedFields(serialized, out var valueChanged))
            {
                EditorGUI.BeginChangeCheck();
                base.OnInspectorGUI();
                valueChanged = EditorGUI.EndChangeCheck();
            }

            m_drawer.DrawNonSerialized(valueChanged);
            if (valueChanged) m_drawer.ManageValueChanged(serialized);

            m_drawer.EndOnGUI(context);
        }
    }
}
#endif