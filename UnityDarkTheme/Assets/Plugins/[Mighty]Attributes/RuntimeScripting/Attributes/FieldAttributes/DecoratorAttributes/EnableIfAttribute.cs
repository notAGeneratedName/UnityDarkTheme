namespace MightyAttributes
{
    public class EnableIfAttribute : BaseDecoratorAttribute
    {
        public string[] ConditionNames { get; }

        public EnableIfAttribute(params string[] conditionNameses) => ConditionNames = conditionNameses;
    }
}
