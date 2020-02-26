using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;
using SFB;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public enum PatchMode : byte
{
    OverwriteFile,
    CreateNewFile
}

public enum Version : byte
{
    Lower_2018_3,
    Equal_2018_3,
    Equal_2019_1,
    Equal_2019_2_3
}

public class UnityDarkTheme : MonoBehaviour
{
    [SerializeField] private CanvasScaler _canvasScaler;
    [SerializeField] private float _sizeFactor;
    [SerializeField] private TMP_Dropdown _versionDropdown;
    [SerializeField] private TMP_InputField _filePathInput;
    [SerializeField] private TMP_Dropdown _patchModeDropdown;
    [SerializeField] private GameObject _outcomePanel;
    [SerializeField] private TextMeshProUGUI _outcomeLabel;
    [SerializeField] private string _patchSuccessMessage, _patchFailureMessage;
    [SerializeField] private string _unpatchSuccessMessage, _unpatchFailureMessage;
    [SerializeField] private string _emptyPathMessage;
    [SerializeField] private float _outComeTimer;

    [SerializeField] private Button _patchButton, _unpatchButton;

    [SerializeField] private UnityVersion _unityVersionPrefab;
    [SerializeField] private Transform _scrollViewContent;
    [SerializeField] private ToggleGroup _group;
    [SerializeField] private AnimatorParameterBehaviour _animatorParameterBehaviour;

#if UNITY_EDITOR
    [Header("Edit Mode")] public bool simulateSuccess;
#endif

    private Version m_version;
    private PatchMode m_patchMode;

    private WaitForSeconds m_waitForOutcome;

    private readonly string UnityFolderPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\Unity\Hub\Editor";
    private const string RegistryKey = @"Software\Unity Technologies\Unity Editor 5.x\UserSkin_h307680651";

    private int m_year;
    private byte m_majorVersion, m_minorVersion, m_patch;
    private UnityVersion m_unityVersion;

    #region Patterns

    private static readonly byte[][] PatchPatterns =
    {
        // < 2018.3:
        // 84 C0 75 08 33 C0 48 83 C4 20 5B C3 8B 03 48 83 C4 20 5B C3
        new byte[]
        {
            0x84, 0xC0, 0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3
        },
        // 2018.3:
        // 84 C0 75 08 33 C0 48 83 C4 30 5B C3 8B 03 48 83 C4 30 5B C3
        new byte[]
        {
            0x84, 0xC0, 0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3
        },
        // 2019.1:
        // 84 DB 74 04 33 C0 EB 02 8B 07 4C 8D 5C 24 70
        new byte[]
        {
            0x84, 0xDB, 0x74, 0x04, 0x33, 0xC0, 0xEB, 0x02, 0x8B, 0x07, 0x4C, 0x8D, 0x5C, 0x24, 0x70
        },
        // > 2019.1:
        // 75 15 33 C0 EB 13 90
        new byte[]
        {
            0x75, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90
        },
    };

    private static readonly byte[][] UnpatchPatterns =
    {
        // < 2018.3:
        // 84 C0 74 08 33 C0 48 83 C4 20 5B C3 8B 03 48 83 C4 20 5B C3
        new byte[]
        {
            0x84, 0xC0, 0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3
        },
        // 2018.3:
        // 84 C0 74 08 33 C0 48 83 C4 30 5B C3 8B 03 48 83 C4 30 5B C3
        new byte[]
        {
            0x84, 0xC0, 0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3
        },
        // 2019.1:
        // 84 DB 75 04 33 C0 EB 02 8B 07 4C 8D 5C 24 70
        new byte[]
        {
            0x84, 0xDB, 0x75, 0x04, 0x33, 0xC0, 0xEB, 0x02, 0x8B, 0x07, 0x4C, 0x8D, 0x5C, 0x24, 0x70
        },
        // > 2019.1:
        // 74 15 33 C0 EB 13 90
        new byte[]
        {
            0x74, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90
        },
    };

    private static readonly byte[] PatchIndexes =
    {
        // < 2018.3
        2,
        // 2018.3
        2,
        // 2019.1:
        2,
        // > 2019.1:
        0,
    };

    private static readonly byte[] UnpatchIndexes =
    {
        // < 2018.3
        2,
        // 2018.3
        2,
        // 2019.1:
        2,
        // > 2019.1:
        0,
    };

    private static readonly byte[] PatchValues =
    {
        // < 2018.3
        0x74,
        // 2018.3
        0x74,
        // 2019.1:
        0x75,
        // > 2019.1:
        0x74,
    };

    private static readonly byte[] UnpatchValues =
    {
        // < 2018.3
        0x75,
        // 2018.3
        0x75,
        // 2019.1:
        0x74,
        // > 2019.1:
        0x75,
    };

    #endregion /Patterns

    private void Awake()
    {
        _outcomePanel.SetActive(false);
        m_waitForOutcome = new WaitForSeconds(_outComeTimer);
        var resolutionVector = _canvasScaler.referenceResolution * _sizeFactor;
        Screen.SetResolution((int) resolutionVector.x, (int) resolutionVector.y, false);
#if !UNITY_EDITOR
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, (int) resolutionVector.x, (int) resolutionVector.y);
#endif

        var versionsFound = ListAllVersions();
        SetButtonsInteractables(!versionsFound);
        _animatorParameterBehaviour.SetBool(0, versionsFound);
        _animatorParameterBehaviour.SetTrigger(1);
    }

    public bool ListAllVersions()
    {
        try
        {
            var count = _scrollViewContent.childCount;
            for (var i = count - 1; i >= 0; i--)
                Destroy(_scrollViewContent.GetChild(i).gameObject);
            if (count > 0)
                _scrollViewContent.DetachChildren();

            var dirInfo = new DirectoryInfo(UnityFolderPath);
            var versionsDirectories = dirInfo.GetDirectories();
            foreach (var directory in versionsDirectories)
            {
                var versionName = directory.Name;
                var nameArray = versionName.Split('.', 'a', 'b', 'f');
                var year = int.Parse(nameArray[0]);
                var major = byte.Parse(nameArray[1]);
                var editorDir = directory.GetDirectories("Editor").FirstOrDefault(t => t.Name == "Editor");
                var unityExe = editorDir?.GetFiles("Unity.exe").FirstOrDefault(f => f.Name == "Unity.exe");
                if (unityExe == null) continue;

                Version version;
                switch (year)
                {
                    case 2019 when major > 1:
                        version = Version.Equal_2019_2_3;
                        break;
                    case 2019 when major == 1:
                        version = Version.Equal_2019_1;
                        break;
                    case 2018 when major == 3:
                        version = Version.Equal_2018_3;
                        break;
                    default:
                    {
                        if (year <= 2018)
                            version = Version.Lower_2018_3;
                        else continue;
                        break;
                    }
                }

                var path = unityExe.FullName;

                bool patched;

//#if !UNITY_EDITOR
                if (IsUnpatchable(path, version)) patched = true;
                else if (IsPatchable(path, version)) patched = false;
                else continue;
//#else
//                patched = false;
//#endif

                var versionSelector = Instantiate(_unityVersionPrefab, _scrollViewContent);
                versionSelector.Init(patched, version, versionName, unityExe.FullName, _group, this);
            }

            return versionsDirectories.Length > 0;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
    }

    public void UnSelectVersion()
    {
        if (m_unityVersion != null)
            m_unityVersion.SelectGUI(false);
    }

    public void SelectVersion(bool patched, Version version, string path, UnityVersion unityVersion)
    {
        _patchButton.interactable = !patched;
        _unpatchButton.interactable = patched;
        m_version = version;
        _versionDropdown.value = (int) version;
        SetInputFieldValue(new[] {path});
        m_unityVersion = unityVersion;
    }

    public void SetButtonsInteractables(bool interactable) => _patchButton.interactable = _unpatchButton.interactable = interactable;

    public void OnClickRefreshList() => ListAllVersions();

    public void OnClickLastVersionButton()
    {
        try
        {
            var dirInfo = new DirectoryInfo(UnityFolderPath);
            DirectoryInfo lastVersionDir = null;
            var versionsDirectories = dirInfo.GetDirectories();
            for (var i = 0; i < versionsDirectories.Length; i++)
            {
                var directory = versionsDirectories[i];
                var nameArray = directory.Name.Split('.', 'a', 'b', 'f');
                var year = int.Parse(nameArray[0]);
                var major = byte.Parse(nameArray[1]);
                var minor = byte.Parse(nameArray[2]);
                var patch = byte.Parse(nameArray[3]);

                if (i != 0 && year <= m_year && major <= m_majorVersion && minor <= m_minorVersion && patch < m_patch) continue;

                lastVersionDir = directory;
                m_year = year;
                m_majorVersion = major;
                m_minorVersion = minor;
                m_patch = patch;
            }

            var editorDir = lastVersionDir?.GetDirectories("Editor").FirstOrDefault(t => t.Name == "Editor");
            var unityExe = editorDir?.GetFiles("Unity.exe").FirstOrDefault(f => f.Name == "Unity.exe");
            if (unityExe == null) return;
            switch (m_year)
            {
                case 2019 when m_majorVersion >= 2:
                    _versionDropdown.value = (int) Version.Equal_2019_2_3;
                    break;
                case 2019 when m_majorVersion == 1:
                    _versionDropdown.value = (int) Version.Equal_2019_1;
                    break;
                case 2018 when m_majorVersion == 3:
                    _versionDropdown.value = (int) Version.Equal_2018_3;
                    break;
                default:
                {
                    if (m_year <= 2018)
                        _versionDropdown.value = (int) Version.Lower_2018_3;
                    break;
                }
            }

            SetInputFieldValue(new[] {unityExe.FullName});
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            throw;
        }
    }

    public void OnClickBrowseButton()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Unity Editor exe", UnityFolderPath, "exe", true);

        if (paths != null && paths.Length != 0)
            SetInputFieldValue(paths);
    }

    public void OnClickPatchButton(bool patch)
    {
        if (string.IsNullOrEmpty(_filePathInput.text))
        {
            StartCoroutine(DisplayEmptyPath());
            return;
        }

        m_version = (Version) _versionDropdown.value;
        m_patchMode = (PatchMode) _patchModeDropdown.value;

#if UNITY_EDITOR
        StartCoroutine(DisplayOutcome(simulateSuccess, patch));
        return;
#endif

        var success = PatchRegistry();

        foreach (var path in GetInputFieldValue())
            success = (patch ? PatchFile(path) : UnpatchFile(path)) && success;

        StartCoroutine(DisplayOutcome(success, patch));
    }

    private bool PatchRegistry()
    {
        try
        {
            using (var key = Registry.CurrentUser)
                key.DeleteSubKey(RegistryKey, false);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
    }

    private void SetInputFieldValue(string[] paths)
    {
        var value = paths.Aggregate("", (current, path) => current + ($"\"{path}\","));
        value = value.Remove(value.Length - 1);
        _filePathInput.text = value;
    }

    private string[] GetInputFieldValue() => _filePathInput.text.Split(new[] {',', '"'}, StringSplitOptions.RemoveEmptyEntries);

    private bool IsPatchable(string path, Version version) =>
        ReadFile(path, out var data, out var length) && SearchPattern(data, PatchPatterns[(int) version], length) > 0;

    private bool IsUnpatchable(string path, Version version) =>
        ReadFile(path, out var data, out var length) && SearchPattern(data, UnpatchPatterns[(int) version], length) > 0;

    private bool PatchFile(string path) =>
        ReadFile(path, out var data, out var length) &&
        UpdateFileData(ref data, PatchPatterns[(int) m_version], PatchValues[(int) m_version],
            PatchIndexes[(int) m_version], length) && WriteFile(path, data, length);

    private bool UnpatchFile(string path) =>
        ReadFile(path, out var data, out var length) &&
        UpdateFileData(ref data, UnpatchPatterns[(int) m_version], UnpatchValues[(int) m_version],
            UnpatchIndexes[(int) m_version], length) && WriteFile(path, data, length);

    private static bool ReadFile(string path, out byte[] data, out int length)
    {
        try
        {
            GrantAccess(path);
            using (var readStream = File.OpenRead(path))
            {
                length = (int) readStream.Length;
                data = new byte[length];
                readStream.Read(data, 0, length);
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            data = null;
            length = -1;
            return false;
        }
    }

    private bool UpdateFileData(ref byte[] data, byte[] pattern, byte value, byte indexOffset, int length)
    {
        var index = SearchPattern(data, pattern, length);

        if (index == -1) return false;

        data[index + indexOffset] = value;
        return true;
    }

    private static int SearchPattern(byte[] data, byte[] pattern, int length)
    {
        var index = -1;
        for (int i = 0, j = 0; i < length; i++)
        {
            if (j >= pattern.Length) return index;
            if (data[i] != pattern[j])
            {
                j = 0;
                index = -1;
                continue;
            }

            if (j == 0)
                index = i;
            j++;
        }

        return index;
    }

    private bool WriteFile(string path, byte[] data, int length)
    {
        try
        {
            switch (m_patchMode)
            {
                case PatchMode.OverwriteFile:
                    using (var writeStream = File.OpenWrite(path))
                        if (writeStream.CanWrite)
                            writeStream.Write(data, 0, length);
                    break;
                case PatchMode.CreateNewFile:
                    using (var file = File.Create(path.Insert(path.Length - 4, "_copy"), length))
                        file.Write(data, 0, length);
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return false;
        }
    }

    private IEnumerator DisplayOutcome(bool success, bool patch)
    {
        _outcomePanel.SetActive(true);
        _outcomeLabel.text = success ? patch ? _patchSuccessMessage : _unpatchSuccessMessage :
            patch ? _patchFailureMessage : _unpatchFailureMessage;
        if (m_unityVersion != null)
            m_unityVersion.SetPatched(success ? patch : !patch);
        yield return m_waitForOutcome;
        _outcomePanel.SetActive(false);
    }

    private IEnumerator DisplayEmptyPath()
    {
        _outcomePanel.SetActive(true);
        _outcomeLabel.text = _emptyPathMessage;
        yield return m_waitForOutcome;
        _outcomePanel.SetActive(false);
    }

    private static void GrantAccess(string file)
    {
        var dInfo = new DirectoryInfo(file);
        var dSecurity = dInfo.GetAccessControl();
        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
            PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
        dInfo.SetAccessControl(dSecurity);
    }
}