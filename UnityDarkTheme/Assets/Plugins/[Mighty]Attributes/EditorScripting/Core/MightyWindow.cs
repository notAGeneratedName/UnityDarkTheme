#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    public abstract class MightyWindow : EditorWindow
    {
        private MightyDrawer m_drawer;
        
        #region Unity Events
        
        private void OnEnable()
        {
            if (MightySettingsServices.Activated) Enable();
            OnEnableWindow();
        }

        private void OnDisable()
        {
            if (MightySettingsServices.Activated) Disable();
            OnDisableWindow();
        }

        private void OnGUI()
        {
            if (MightySettingsServices.Activated) GUI();
            OnGUIWindow();
        }

        #endregion /Unity Events

        #region Virtuals

        protected virtual void OnEnableWindow()
        {
            
        }
        
        protected virtual void OnDisableWindow()
        {
            
        }       
        
        protected virtual void OnGUIWindow()
        {
            
        }

        #endregion /Virtuals

        private void Enable()
        {
            m_drawer = new MightyDrawer();
            try
            {
                m_drawer.OnEnableWindow(this);
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
        
        private void GUI()
        {
            if (m_drawer == null) Enable();

            m_drawer.BeginOnGUI(this);
            m_drawer.DrawNonSerialized(false);
            m_drawer.EndOnGUI(this);
        }
        
    }
}
#endif
