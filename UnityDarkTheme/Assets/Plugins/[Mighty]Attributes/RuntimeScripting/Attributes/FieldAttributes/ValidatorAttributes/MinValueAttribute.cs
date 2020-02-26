namespace MightyAttributes
{
    public class MinValueAttribute : BaseValidatorAttribute
    {
        public float MinValue { get; }

        public MinValueAttribute(float minValue) => MinValue = minValue;

        public MinValueAttribute(int minValue) => MinValue = minValue;
    }
}
