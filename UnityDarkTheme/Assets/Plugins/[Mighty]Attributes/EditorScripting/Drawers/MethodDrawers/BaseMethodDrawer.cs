#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public abstract class BaseMethodDrawer : BaseMightyDrawer
    {
        public abstract void DrawMethod(MightyMember<MethodInfo> mightyMember, BaseMethodAttribute baseAttribute);
    }
}
#endif