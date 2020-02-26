#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(HideIfEmptyAttribute))]
    public class HideIfEmptyDrawer : BasePropertyDrawCondition
    {
        public override bool CanDrawProperty(BaseMightyMember mightyMember, BaseDrawConditionAttribute baseAttribute)
        {
            var property = mightyMember.Property;

            return !property.isArray || property.arraySize != 0;
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