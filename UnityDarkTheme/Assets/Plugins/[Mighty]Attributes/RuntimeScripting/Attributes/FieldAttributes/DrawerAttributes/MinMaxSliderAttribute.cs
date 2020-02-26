namespace MightyAttributes
{
    public class MinMaxSliderAttribute : BaseDrawerAttribute
    {
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public MinMaxSliderAttribute(float minValue, float maxValue, FieldOption option = FieldOption.Nothing) : base(option)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
    }
}