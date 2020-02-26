#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowScript : MonoBehaviour, IDragHandler
{
    private Vector2 m_deltaValue;

    public void OnClickCloseButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickMinimizeButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        BorderlessWindow.MinimizeWindow();
    }

    public void OnDrag(PointerEventData data)
    {
        if (BorderlessWindow.framed)
            return;

        m_deltaValue += data.delta;
        if (data.dragging) BorderlessWindow.MoveWindowPos(m_deltaValue, Screen.width, Screen.height);
    }
}