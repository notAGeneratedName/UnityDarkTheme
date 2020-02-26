namespace MightyAttributes
{
    [BeginHorizontal, AutoWidth("FieldWidth")]
    public class BeginHorizontalAutoWidthAttribute : MightyWrapperAttribute
    {
        public float? FieldWidth { get; }

        public BeginHorizontalAutoWidthAttribute() => FieldWidth = null;

        public BeginHorizontalAutoWidthAttribute(float fieldWidth) => FieldWidth = fieldWidth;
    }
}