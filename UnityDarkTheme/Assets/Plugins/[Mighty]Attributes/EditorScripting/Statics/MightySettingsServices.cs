#if UNITY_EDITOR
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;

namespace MightyAttributes.Editor
{
    public class MightyAutoValues : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!MightySettingsServices.AutoValuesOnBuild) return;
            ApplyAutoValues();
        }

        [DidReloadScripts]
        private static void OnScriptLoaded() => EditorApplication.playModeStateChanged += ApplyAutoValuesOnExitEditMode;

        private static void ApplyAutoValuesOnExitEditMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && MightySettingsServices.AutoValuesOnPlay) ApplyAutoValues();
        }

        [MenuItem("Window/[Mighty]Attributes/Apply Auto Values", false, 2050)]
        public static async void ApplyAutoValuesAsync()
        {
            AutoValuesWindowUtility.Open();
            await Task.Delay(50);

            var mightyEditors = MightyEditorUtility.GetMightyEditors().ToArray();
            AutoValuesWindowUtility.DisplayCount(mightyEditors.Length);

            for (var i = 0; i < mightyEditors.Length; i++)
            {
                AutoValuesWindowUtility.SetIndex(i);
                await Task.Yield();
                mightyEditors[i].ApplyAutoValues();
            }

            AutoValuesWindowUtility.Close();

            EditorDrawUtility.MightyDebug("Auto Values Applied");
        }

        public static void ApplyAutoValues()
        {
            foreach (var script in SerializedPropertyUtility.FindAllObjects<MonoBehaviour>())
                if (script.CreateEditor(out var mightyEditor))
                    mightyEditor.ApplyAutoValues();

            EditorDrawUtility.MightyDebug("Auto Values Applied");
        }
    }

    public static class MightySettingsServices
    {
        public const string DEFAULT_MAIN_ASSEMBLY_NAME = "Assembly-CSharp";
        public const string DEFAULT_PLUGINS_ASSEMBLY_NAME = "Assembly-CSharp-firstpass";

        #region Params

        private struct MightySetting<T>
        {
            private readonly string m_paramName;
            private readonly T m_defaultValue;

            public T Value
            {
                get => PlayerPrefsUtilities.GetPlayerPref(m_paramName, m_defaultValue);
                set => PlayerPrefsUtilities.SetPlayerPref(m_paramName, value);
            }

            public MightySetting(string name, T defaultValue)
            {
                m_paramName = name;
                m_defaultValue = defaultValue;
            }
        }

        // @formatter:off
        private static MightySetting<bool> m_activated = new MightySetting<bool>("Activated", true);
        private static MightySetting<bool> m_autoValuesOnPlay = new MightySetting<bool>("AutoValuesOnPlay", true);
        private static MightySetting<bool> m_autoValuesOnBuild = new MightySetting<bool>("AutoValuesOnBuild", true);

        private static MightySetting<string> m_mainAssemblyName = new MightySetting<string>("MainAssembly", DEFAULT_MAIN_ASSEMBLY_NAME);
        private static MightySetting<string> m_pluginsAssemblyName = new MightySetting<string>("PluginsAssembly", DEFAULT_PLUGINS_ASSEMBLY_NAME);
        // @formatter:on

        #endregion /Params

        #region Properties

        public static bool Activated
        {
            get => m_activated.Value;
            set => m_activated.Value = value;
        }

        public static bool AutoValuesOnPlay
        {
            get => m_autoValuesOnPlay.Value;
            set => m_autoValuesOnPlay.Value = value;
        }

        public static bool AutoValuesOnBuild
        {
            get => m_autoValuesOnBuild.Value;
            set => m_autoValuesOnBuild.Value = value;
        }

        public static string MainAssemblyName
        {
            get => m_mainAssemblyName.Value;
            set => m_mainAssemblyName.Value = value;
        }

        public static string PluginsAssemblyName
        {
            get => m_pluginsAssemblyName.Value;
            set => m_pluginsAssemblyName.Value = value;
        }

        #endregion /Properties
    }
}
#endif