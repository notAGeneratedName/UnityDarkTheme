namespace MightyAttributes
{
    [HideIf("HideCondition"), Width("LabelWidth", "FieldWidth")]
    public class HideWidthAttribute : MightyWrapperAttribute
    {
        [CallbackName] public string HideCondition { get; }
        
        public float LabelWidth { get; }
        public float? FieldWidth { get; }

        public HideWidthAttribute(string hideCondition, float labelWidth, float fieldWidth)
        {
            HideCondition = hideCondition;
            LabelWidth = labelWidth;
            FieldWidth = fieldWidth;
        }

        public HideWidthAttribute(string hideCondition, float labelWidth)
        {
            HideCondition = hideCondition;
            LabelWidth = labelWidth;
            FieldWidth = null;
        }
    }
}