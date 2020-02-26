namespace MightyAttributes
{
    public class DisableIfAttribute : BaseDecoratorAttribute
    {
        public string[] ConditionNames { get; }

        public DisableIfAttribute(params string[] conditionNames) => ConditionNames = conditionNames;
    }
}
