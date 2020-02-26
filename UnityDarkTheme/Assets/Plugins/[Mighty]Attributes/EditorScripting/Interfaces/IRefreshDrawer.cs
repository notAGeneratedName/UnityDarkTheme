#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    public interface IRefreshDrawer
    {
        void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute);
    }
}
#endif