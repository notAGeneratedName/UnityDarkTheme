namespace MightyAttributes
{
    public class HideIfAttribute : BaseDrawConditionAttribute
    {
        public string[] ConditionNames { get; private set; }

        public HideIfAttribute(params string[] conditionNames)
        {
            this.ConditionNames = conditionNames;
        }
    }
}
