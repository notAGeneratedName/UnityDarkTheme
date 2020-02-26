#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    public abstract class BasePropertyMeta : BaseMightyDrawer
    {
        public abstract void ApplyPropertyMeta(BaseMightyMember mightyMember, BaseMetaAttribute metaAttribute);
    }
}
#endif