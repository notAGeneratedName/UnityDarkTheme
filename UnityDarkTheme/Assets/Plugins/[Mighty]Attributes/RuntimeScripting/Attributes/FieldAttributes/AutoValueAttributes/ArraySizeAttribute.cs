namespace MightyAttributes
{
    public class ArraySizeAttribute : BaseAutoValueAttribute
    {
        public int Size { get; }
        
        public string SizeCallback { get; }

        public ArraySizeAttribute(int size, bool executeInPlayMode = false) : base(executeInPlayMode)
        {
            Size = size;
        }
        
        public ArraySizeAttribute(string sizeCallback, bool executeInPlayMode = false) : base(executeInPlayMode)
        {
            SizeCallback = sizeCallback;
        }
    }
}
