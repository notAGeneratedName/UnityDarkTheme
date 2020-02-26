using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public class LineAttribute : BaseElementDecoratorAttribute
    {
        public DecoratorPosition Position { get; } = DecoratorPosition.Before;
        public string PositionName { get; }

        public ColorValue Color { get; } = ColorValue.Default;
        public string ColorName { get; }

        public LineAttribute(DecoratorPosition position = DecoratorPosition.Before, ColorValue color = ColorValue.Grey)
        {
            Position = position;
            Color = color;
        }
        
        public LineAttribute(string positionName, ColorValue color = ColorValue.Grey)
        {
            PositionName = positionName;
            Color = color;
        }

        public LineAttribute(string positionName, string colorName)
        {
            PositionName = positionName;
            ColorName = colorName;
        }

        public LineAttribute(DecoratorPosition position, string colorName)
        {
            Position = position;
            ColorName = colorName;
        }

        public LineAttribute(ColorValue color) => Color = color;
    }
}