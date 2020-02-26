#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    public abstract class BasePropertyValidator : BaseMightyDrawer
    {
        public abstract void ValidateProperty(BaseMightyMember mightyMember, BaseValidatorAttribute baseAttribute);
    }
}
#endif