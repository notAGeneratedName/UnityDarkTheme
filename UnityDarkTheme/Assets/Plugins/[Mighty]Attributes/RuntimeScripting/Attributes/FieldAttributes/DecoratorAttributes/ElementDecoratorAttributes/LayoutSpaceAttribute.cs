namespace MightyAttributes
{
    public class LayoutSpaceAttribute : BaseElementDecoratorAttribute
    {
        public float Size { get; }

        public DecoratorPosition Position { get; } = DecoratorPosition.Before;
        
        public string PositionName { get; }

        public LayoutSpaceAttribute(float size = 8, DecoratorPosition position = DecoratorPosition.Before)
        {
            Size = size;
            Position = position;
        }

        public LayoutSpaceAttribute(DecoratorPosition position)
        {
            Size = 8;
            Position = position;
        }

        public LayoutSpaceAttribute(float size, string positionName)
        {
            Size = size;
            PositionName = positionName;
        }
        
        public LayoutSpaceAttribute(string positionName)
        {
            Size = 8;
            PositionName = positionName;
        }
    }
}