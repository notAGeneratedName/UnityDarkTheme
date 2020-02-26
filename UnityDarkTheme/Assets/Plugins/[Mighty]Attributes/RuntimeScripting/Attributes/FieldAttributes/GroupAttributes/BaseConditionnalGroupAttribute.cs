namespace MightyAttributes
{
    public abstract class BaseConditionalGroupAttribute : BaseGlobalGroupAttribute
    {
        protected BaseConditionalGroupAttribute(string name, bool drawName, bool nameAsCallback, string backgroundColorName,
            string contentColorName) : base(name, drawName, nameAsCallback, backgroundColorName, contentColorName)
        {
        }

        protected BaseConditionalGroupAttribute(string name, bool drawName, bool nameAsCallback, ColorValue backgroundColor,
            ColorValue contentColor) : base(name, drawName, nameAsCallback, backgroundColor, contentColor)
        {
        }
    }
}