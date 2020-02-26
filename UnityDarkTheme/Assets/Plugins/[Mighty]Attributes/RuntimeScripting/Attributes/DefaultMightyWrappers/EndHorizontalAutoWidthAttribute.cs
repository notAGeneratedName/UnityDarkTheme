namespace MightyAttributes
{
    [EndHorizontal, AutoWidth("FieldWidth")]
    public class EndHorizontalAutoWidthAttribute : MightyWrapperAttribute
    {
        public float? FieldWidth { get; }

        public EndHorizontalAutoWidthAttribute() => FieldWidth = null;

        public EndHorizontalAutoWidthAttribute(float fieldWidth) => FieldWidth = fieldWidth;
    }
}