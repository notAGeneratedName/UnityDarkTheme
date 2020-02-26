namespace MightyAttributes
{
    public class FoldAttribute : BaseConditionalGroupAttribute
    {
        public override ColorValue BackgroundColor { get; } = ColorValue.Brighter;
        
        public FoldAttribute(string name = "", bool drawName = true, bool nameAsCallback = false, string backgroundColorName = null,
            string contentColorName = null) : base(name, drawName, nameAsCallback, backgroundColorName, contentColorName)
        {
        }

        public FoldAttribute(string name, bool drawName, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor = ColorValue.Default) : base(name, drawName, nameAsCallback, backgroundColor, contentColor)
        {
        }

        public FoldAttribute(string name, ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(name, true, false, backgroundColor, contentColor)
        {
        }

        public FoldAttribute(ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base("", false, false, backgroundColor, contentColor)
        {
        }
    }

    public class FoldGroupAttribute : BaseConditionalGroupAttribute
    {
        public override ColorValue BackgroundColor { get; } = ColorValue.Brighter;

        public FoldGroupAttribute(string name = "", bool drawName = true, bool nameAsCallback = false, string backgroundColorName = null,
            string contentColorName = null) : base(name, drawName, nameAsCallback, backgroundColorName, contentColorName)
        {
        }

        public FoldGroupAttribute(string name, bool drawName, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor = ColorValue.Default) : base(name, drawName, nameAsCallback, backgroundColor, contentColor)
        {
        }

        public FoldGroupAttribute(string name, ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(name, true, false, backgroundColor, contentColor)
        {
        }

        public FoldGroupAttribute(ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base("", false, false, backgroundColor, contentColor)
        {
        }
    }
}