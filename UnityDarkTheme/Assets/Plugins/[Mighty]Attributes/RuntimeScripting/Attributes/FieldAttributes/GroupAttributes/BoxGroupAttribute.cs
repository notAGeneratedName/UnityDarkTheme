namespace MightyAttributes
{
    public class BoxAttribute : BoxGroupAttribute
    {
        public BoxAttribute(string name = "", bool drawName = true, bool drawLine = true, bool nameAsCallback = false,
            string backgroundColorName = null, string contentColorName = null) : base(name, drawName, drawLine, nameAsCallback,
            backgroundColorName, contentColorName)
        {
        }

        public BoxAttribute(string name, bool drawName, bool drawLine, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor = ColorValue.Default) : base(name, drawName, drawLine, nameAsCallback, backgroundColor, contentColor)
        {
        }

        public BoxAttribute(string name, ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(name, backgroundColor, contentColor)
        {
        }

        public BoxAttribute(ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(backgroundColor, contentColor)
        {
        }
    }

    public class BoxGroupAttribute : BaseGroupAttribute
    {
        public override ColorValue BackgroundColor { get; } = ColorValue.Brightest;

        public BoxGroupAttribute(string name = "", bool drawName = true, bool drawLine = true, bool nameAsCallback = false,
            string backgroundColorName = null, string contentColorName = null) : base(name, drawName, drawLine, nameAsCallback,
            backgroundColorName, contentColorName)
        {
        }

        public BoxGroupAttribute(string name, bool drawName, bool drawLine, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor = ColorValue.Default) : base(name, drawName, drawLine, nameAsCallback, backgroundColor,
            contentColor)
        {
            BackgroundColor = backgroundColor;
        }

        public BoxGroupAttribute(string name, ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base(name, true, true, false, backgroundColor, contentColor)
        {
            BackgroundColor = backgroundColor;
        }

        public BoxGroupAttribute(ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default) :
            base("", false, false, false, backgroundColor, contentColor)
        {
            BackgroundColor = backgroundColor;
        }
    }
}