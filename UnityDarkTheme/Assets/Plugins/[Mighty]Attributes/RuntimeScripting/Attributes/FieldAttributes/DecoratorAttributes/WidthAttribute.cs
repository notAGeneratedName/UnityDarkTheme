namespace MightyAttributes
{
    public class WidthAttribute : BaseDecoratorAttribute
    {
        public float LabelWidth { get; }
        public float? FieldWidth { get; }
        
        public string LabelWidthCallback { get; }
        public string FieldWidthCallback { get; }
        
        public WidthAttribute(float labelWidth, float fieldWidth)
        {
            LabelWidth = labelWidth;
            FieldWidth = fieldWidth;
        }    
        
        public WidthAttribute(float labelWidth)
        {
            LabelWidth = labelWidth;
            FieldWidth = null;
        }

        public WidthAttribute(string labelWidthCallback, string fieldWidthCallback = null)
        {
            LabelWidthCallback = labelWidthCallback;
            FieldWidthCallback = fieldWidthCallback;
        }
    }
}