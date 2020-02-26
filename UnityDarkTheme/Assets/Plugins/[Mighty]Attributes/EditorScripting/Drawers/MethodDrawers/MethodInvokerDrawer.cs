#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(OnInspectorGUIAttribute), typeof(OnEnableAttribute), typeof(OnModifiedPropertiesAttribute))]
    public class MethodInvokerDrawer : BaseMethodDrawer
    {
        public override void DrawMethod(MightyMember<MethodInfo> mightyMember, BaseMethodAttribute baseAttribute)
        {
            var methodInfo = mightyMember.MemberInfo;
            if (methodInfo.GetParameters().Length == 0)
            {
                if (baseAttribute.ExecuteInPlayMode || !EditorApplication.isPlaying)
                    methodInfo.Invoke(mightyMember.Target, null);
            }
            else
                EditorDrawUtility.DrawHelpBox($"{baseAttribute.GetType().Name} works only on methods with no parameters");
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif