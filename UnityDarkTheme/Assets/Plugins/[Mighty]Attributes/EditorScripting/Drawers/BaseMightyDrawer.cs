#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    public abstract class BaseMightyDrawer
    {
        public abstract void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute);

        public abstract void ClearCache();
    }
}
#endif