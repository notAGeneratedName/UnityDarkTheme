namespace MightyAttributes
{
    public class FindObjectsAttribute : NameObjectAttribute
    {
        public FindObjectsAttribute(bool includeInactive = true, bool executeInPlayMode = false) : base(includeInactive, executeInPlayMode)
        {
        }

        public FindObjectsAttribute(string name, bool includeInactive = true, bool executeInPlayMode = false) :
            base(name, includeInactive, executeInPlayMode)
        {
        }
    }
}