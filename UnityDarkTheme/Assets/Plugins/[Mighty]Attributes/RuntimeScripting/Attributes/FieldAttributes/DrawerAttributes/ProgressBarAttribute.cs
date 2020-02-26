namespace MightyAttributes
{
    public class ProgressBarAttribute : BaseDrawerAttribute
    {
        public string Name { get; }
        public float MaxValue { get; }
        public ColorValue Color { get; }
        public string ColorName { get; }

        public ProgressBarAttribute(string name = "", float maxValue = 100, ColorValue color = ColorValue.Blue,
            FieldOption option = FieldOption.Nothing) : base(option)
        {
            Name = name;
            MaxValue = maxValue;
            Color = color;
        }

        public ProgressBarAttribute(string name = "", float maxValue = 100, string colorName = null,
            FieldOption option = FieldOption.Nothing) : base(option)
        {
            Name = name;
            MaxValue = maxValue;
            ColorName = colorName;
        }
    }
}