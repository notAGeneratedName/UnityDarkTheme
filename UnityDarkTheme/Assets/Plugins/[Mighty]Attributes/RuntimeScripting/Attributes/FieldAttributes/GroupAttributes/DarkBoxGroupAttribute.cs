namespace MightyAttributes
{
    public class DarkBoxAttribute : DarkBoxGroupAttribute
    {
        public DarkBoxAttribute(string name = "", bool drawName = true, bool drawLine = true, bool nameAsCallback = false,
            string backgroundColorName = null, string contentColorName = null) : base(name, drawName, drawLine, nameAsCallback,
            backgroundColorName, contentColorName)
        {
        }

        public DarkBoxAttribute(string name, bool drawName, bool drawLine, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor = ColorValue.Default) : base(name, drawName, drawLine, nameAsCallback, backgroundColor, contentColor)
        {
        }
        public DarkBoxAttribute(string name, ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(name, backgroundColor, contentColor)
        {
        }

        public DarkBoxAttribute(ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(backgroundColor, contentColor)
        {
        }
    }

    public class DarkBoxGroupAttribute : BaseGroupAttribute
    {
        public override ColorValue BackgroundColor { get; } = ColorValue.Brightest;
        
        public DarkBoxGroupAttribute(string name = "", bool drawName = true, bool drawLine = true, bool nameAsCallback = false,
            string backgroundColorName = null, string contentColorName = null) : base(name, drawName, drawLine, nameAsCallback,
            backgroundColorName, contentColorName)
        {
        }

        public DarkBoxGroupAttribute(string name, bool drawName, bool drawLine, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor = ColorValue.Default) : base(name, drawName, drawLine, nameAsCallback, backgroundColor,
            contentColor)
        {
            BackgroundColor = backgroundColor;
        }

        public DarkBoxGroupAttribute(string name, ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(name, true, true, false, backgroundColor, contentColor)
        {
            BackgroundColor = backgroundColor;
        }

        public DarkBoxGroupAttribute(ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base("", false, false, false, backgroundColor, contentColor)
        {
            BackgroundColor = backgroundColor;
        }
    }
}