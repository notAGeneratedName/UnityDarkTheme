#if UNITY_EDITOR
using System.Reflection;

namespace MightyAttributes.Editor
{
    public abstract class BaseFieldDrawer : BaseMightyDrawer
    {
        public abstract void DrawField(MightyMember<FieldInfo> mightyMember, BaseDrawerAttribute baseAttribute);
    }
}
#endif