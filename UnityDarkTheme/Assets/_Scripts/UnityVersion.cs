using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnityVersion : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private Image _logoImage, _outlineImage, _backgroundImage;
    [SerializeField] private Color32 _unpatchedColor, _patchedColor, _selectedUnpatchedColor, _selectedPatchedColor;

    private bool m_patched;
    private Version m_version;
    private string m_path;
    private UnityDarkTheme m_unityDarkTheme;

    public void Init(bool patched, Version version, string versionName, string path, ToggleGroup group, UnityDarkTheme unityDarkTheme)
    {
        SetPatched(patched);
        m_version = version;
        _label.text = versionName;
        m_path = path;
        _toggle.group = group;
        m_unityDarkTheme = unityDarkTheme;
    }

    public void Select(bool selected)
    {
        SelectGUI(selected);
        if (m_unityDarkTheme == null) return;
        if (selected)
            m_unityDarkTheme.SelectVersion(m_patched, m_version, m_path, this);
        else
            m_unityDarkTheme.SetButtonsInteractables(false);
    }

    public void SetPatched(bool patched)
    {
        m_patched = patched;
        _outlineImage.color = patched ? _unpatchedColor : _patchedColor;
        _backgroundImage.color = patched ? _patchedColor : _unpatchedColor;
        _logoImage.color = patched ? _unpatchedColor : _patchedColor;
        _label.color = patched ? _unpatchedColor : _patchedColor;
        _toggle.isOn = false;
    }

    public void SelectGUI(bool selected)
    {
        _outlineImage.color = m_patched ? selected ? _selectedUnpatchedColor : _unpatchedColor :
            selected ? _selectedPatchedColor : _patchedColor;
        _backgroundImage.color = m_patched ? selected ? _selectedPatchedColor : _patchedColor :
            selected ? _selectedUnpatchedColor : _unpatchedColor;
    }
}