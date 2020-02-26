namespace MightyAttributes
{
    public class ScriptFieldAttribute : BaseDecoratorAttribute
    {
        public FieldPosition Position { get; }
        
        public ScriptFieldAttribute(FieldPosition position = FieldPosition.Before) => Position = position;
    }
}