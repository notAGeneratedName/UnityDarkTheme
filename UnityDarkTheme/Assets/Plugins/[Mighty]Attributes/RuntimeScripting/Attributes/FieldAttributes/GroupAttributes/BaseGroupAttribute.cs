namespace MightyAttributes
{
    public abstract class BaseGroupAttribute : BaseGlobalGroupAttribute
    {
        public bool DrawLine { get; }

        public ColorValue LineColor { get; set; } = ColorValue.Grey;

        protected BaseGroupAttribute(string name, bool drawName, bool drawLine, bool nameAsCallback, string backgroundColorName,
            string contentColorName) : base(name, drawName, nameAsCallback, backgroundColorName, contentColorName) =>
            DrawLine = drawLine;

        protected BaseGroupAttribute(string name, bool drawName, bool drawLine, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor) : base(name, drawName, nameAsCallback, backgroundColor, contentColor) => DrawLine = drawLine;
    }
}