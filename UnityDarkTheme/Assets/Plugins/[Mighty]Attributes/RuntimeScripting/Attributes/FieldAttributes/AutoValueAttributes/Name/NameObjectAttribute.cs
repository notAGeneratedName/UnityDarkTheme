namespace MightyAttributes
{
    public class NameObjectAttribute : BaseSearchObjectAttribute
    {
        public readonly string Name;
        
        public NameObjectAttribute(bool includeInactive, bool executeInPlayMode) : base(includeInactive, executeInPlayMode)
        {
        }
        
        public NameObjectAttribute(string name, bool includeInactive, bool executeInPlayMode) : base(includeInactive, executeInPlayMode)
        {
            Name = name;
        }
    }
}