namespace MightyAttributes
{
    public class FlexibleSpaceAttribute : BaseElementDecoratorAttribute
    {
        public DecoratorPosition Position { get; } = DecoratorPosition.Before;

        public string PositionName { get; }

        public FlexibleSpaceAttribute(DecoratorPosition position = DecoratorPosition.Before) => Position = position;

        public FlexibleSpaceAttribute(string positionName) => PositionName = positionName;
    }
}