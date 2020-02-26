#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    public abstract class BaseDecoratorDrawer : BaseGlobalDecoratorDrawer
    {
        public abstract void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute);
        
        public abstract void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute);
    }
}
#endif