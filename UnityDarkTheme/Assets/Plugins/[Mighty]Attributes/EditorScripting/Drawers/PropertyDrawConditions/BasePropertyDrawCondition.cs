#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    public abstract class BasePropertyDrawCondition : BaseMightyDrawer
    {
        public abstract bool CanDrawProperty(BaseMightyMember mightyMember, BaseDrawConditionAttribute baseAttribute);
    }
}
#endif