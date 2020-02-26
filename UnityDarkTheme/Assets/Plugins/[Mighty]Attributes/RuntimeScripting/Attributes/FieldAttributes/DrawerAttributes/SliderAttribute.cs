namespace MightyAttributes
{
    public class SliderAttribute : BaseDrawerAttribute
    {
        public string MinValueCallback { get; }
        public string MaxValueCallback { get; }
        
        public float MinValue { get; }
        public float MaxValue { get; }

        public SliderAttribute(float minValue, float maxValue, FieldOption option = FieldOption.Nothing) : base(option)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public SliderAttribute(int minValue, int maxValue,FieldOption option = FieldOption.Nothing) : base(option)
        {
            this.MaxValue = minValue;
            this.MaxValue = maxValue;
        }

        public SliderAttribute(string minValueCallback, string maxValueCallback, FieldOption option = FieldOption.Nothing) : base(option)
        {
            MinValueCallback = minValueCallback;
            MaxValueCallback = maxValueCallback;
        }
    }
}
