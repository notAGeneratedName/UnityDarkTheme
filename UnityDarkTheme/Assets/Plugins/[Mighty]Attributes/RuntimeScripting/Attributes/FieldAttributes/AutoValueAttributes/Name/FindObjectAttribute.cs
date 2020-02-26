namespace MightyAttributes
{
    public class FindObjectAttribute : NameObjectAttribute
    {
        public FindObjectAttribute(bool includeInactive = true, bool executeInPlayMode = false) : base(includeInactive, executeInPlayMode)
        {
        }
        
        public FindObjectAttribute(string name, bool includeInactive = true, bool executeInPlayMode = false) : 
            base(name, includeInactive, executeInPlayMode)
        {
        }
    }
}