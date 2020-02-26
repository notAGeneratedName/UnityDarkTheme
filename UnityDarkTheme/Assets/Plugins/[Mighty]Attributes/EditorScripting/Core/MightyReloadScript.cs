#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

namespace MightyAttributes.Editor
{
    public static class MightyReloadScript
    {
        [DidReloadScripts]
        [MenuItem("Window/[Mighty]Attributes/Apply Script Reload", false, 2051)]
        public static void OnReloadScript()
        {
            foreach (var drawer in SpecialDrawersDatabase.GetChildrenDrawers<BaseReloadScriptDrawer>())
                drawer.OnReloadScript();

            EditorDrawUtility.MightyDebug("Apply Script Reload");
        }
    }
}
#endif