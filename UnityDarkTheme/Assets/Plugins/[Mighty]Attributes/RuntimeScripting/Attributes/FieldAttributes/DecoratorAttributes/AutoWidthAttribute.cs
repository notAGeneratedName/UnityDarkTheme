namespace MightyAttributes
{
    public class AutoWidthAttribute : BaseDecoratorAttribute
    {
        public float? FieldWidth { get; }
        
        public string FieldWidthCallback { get; }
        
        public AutoWidthAttribute() => FieldWidth = null;

        public AutoWidthAttribute(float fieldWidth) => FieldWidth = fieldWidth;

        public AutoWidthAttribute(string fieldWidthCallback = null) => FieldWidthCallback = fieldWidthCallback;
    }
}