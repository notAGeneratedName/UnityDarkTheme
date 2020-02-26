#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ButtonAttribute))]
    public class ButtonMethodDrawer : BaseMethodDrawer, IRefreshDrawer
    {
        private readonly MightyCache<MightyInfo<bool>> m_buttonCache = new MightyCache<MightyInfo<bool>>();

        public override void DrawMethod(MightyMember<MethodInfo> mightyMember, BaseMethodAttribute baseAttribute)
        {
            var methodInfo = mightyMember.MemberInfo;

            if (methodInfo.GetParameters().Length == 0)
            {
                var buttonAttribute = (ButtonAttribute) baseAttribute;
                var buttonText = string.IsNullOrEmpty(buttonAttribute.Text) ? methodInfo.Name.DrawPrettyName() : buttonAttribute.Text;

                var enabled = GUI.enabled;
                if (!m_buttonCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
                GUI.enabled = m_buttonCache[mightyMember].Value && (buttonAttribute.ExecuteInPlayMode || !EditorApplication.isPlaying);

                if (GUILayout.Button(buttonText, GUILayout.Height(buttonAttribute.Height))) methodInfo.Invoke(mightyMember.Target, null);

                GUI.enabled = enabled;
            }
            else
                EditorDrawUtility.DrawHelpBox($"{typeof(ButtonAttribute).Name} works only on methods with no parameters");
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var buttonAttribute = (ButtonAttribute) mightyAttribute;

            if (mightyMember.Target.GetBoolInfo(buttonAttribute.EnabledCallback, out var enabledInfo))
                enabledInfo.Value = enabledInfo.Value;
            else
                enabledInfo = new MightyInfo<bool>(GUI.enabled && buttonAttribute.ExecuteInPlayMode || !EditorApplication.isPlaying);

            m_buttonCache[mightyMember] = enabledInfo;
        }

        public override void ClearCache() => m_buttonCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_buttonCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            if (((ButtonAttribute) mightyAttribute).ExecuteInPlayMode || !EditorApplication.isPlaying)
                m_buttonCache[mightyMember].RefreshValue();
        }
    }
}
#endif