namespace MightyAttributes
{
    public class ReorderableListAttribute : BaseArrayAttribute
    {
        public bool DrawButtons { get; }

        public ReorderableListAttribute(bool drawButtons = true, ArrayOption option = ArrayOption.Nothing) : base(option) => 
            DrawButtons = drawButtons;
    }
    
    public class ReorderableAttribute : ReorderableListAttribute
    {
        public ReorderableAttribute(bool drawButtons = true, ArrayOption option = ArrayOption.Nothing) : base(drawButtons, option)
        {
            
        }
    }
}