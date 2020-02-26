namespace MightyAttributes
{
    public class ColorAttribute : BaseDecoratorAttribute
    {
        public string BackgroundColorName { get; private set; }
        
        public string ContentColorName { get; private set; }
        
        public ColorValue BackgroundColor { get; private set; }
        
        public ColorValue ContentColor { get; private set; }
        
        public ColorAttribute(string backgroundColorName = null, string contentColorName = null)
        {
            BackgroundColorName = backgroundColorName;
            ContentColorName = contentColorName;
            BackgroundColor = ColorValue.Default;
            ContentColor = ColorValue.Default;
        }
        
        public ColorAttribute(ColorValue backgroundColor, ColorValue contentColor = ColorValue.Default)
        {
            BackgroundColor = backgroundColor;
            ContentColor = contentColor;
        }
    }
}