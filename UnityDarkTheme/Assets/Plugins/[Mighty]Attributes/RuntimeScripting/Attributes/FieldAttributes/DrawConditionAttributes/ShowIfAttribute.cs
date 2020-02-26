namespace MightyAttributes
{
    public class ShowIfAttribute : BaseDrawConditionAttribute
    {
        public string[] ConditionNames { get; }
        
        public ShowIfAttribute(params string[] conditionNames) => ConditionNames = conditionNames;
    }
}
