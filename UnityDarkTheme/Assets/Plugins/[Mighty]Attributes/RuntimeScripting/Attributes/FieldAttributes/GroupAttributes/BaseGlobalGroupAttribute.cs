namespace MightyAttributes
{
    public abstract class BaseGlobalGroupAttribute : BaseFieldAttribute
    {
        public string Name { get; }

        public bool DrawName { get; }

        public bool NameAsCallback { get; }

        public string BackgroundColorName { get; }

        public string ContentColorName { get; }

        public virtual ColorValue BackgroundColor { get; }

        public ColorValue ContentColor { get; }

        protected BaseGlobalGroupAttribute(string name, bool drawName, bool nameAsCallback, string backgroundColorName,
            string contentColorName)
        {
            Name = name;
            DrawName = drawName;
            NameAsCallback = nameAsCallback;
            BackgroundColorName = backgroundColorName;
            ContentColorName = contentColorName;
            BackgroundColor = ColorValue.Default;
            ContentColor = ColorValue.Default;
        }

        protected BaseGlobalGroupAttribute(string name, bool drawName, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor)
        {
            Name = name;
            DrawName = drawName;
            NameAsCallback = nameAsCallback;
            BackgroundColor = backgroundColor;
            ContentColor = contentColor;
        }
    }
}