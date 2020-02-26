namespace MightyAttributes
{
    [Nest("NestOption"), Line("Position")]
    public class LineNestAttribute : MightyWrapperAttribute
    {
        [CallbackName] public string Position { get; }

        public NestOption NestOption { get; }

        public LineNestAttribute(string linePosition = null, NestOption option = NestOption.Nothing)
        {
            Position = linePosition;
            NestOption = option;
        }
    }
}