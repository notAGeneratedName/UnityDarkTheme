namespace MightyAttributes
{
    public class MaxValueAttribute : BaseValidatorAttribute
    {
        public float MaxValue { get; }

        public MaxValueAttribute(float maxValue) => MaxValue = maxValue;

        public MaxValueAttribute(int maxValue) => MaxValue = maxValue;
    }
}
