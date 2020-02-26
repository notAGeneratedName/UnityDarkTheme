namespace MightyAttributes
{
    public class GetComponentInChildrenAttribute : NameObjectAttribute
    {
        public GetComponentInChildrenAttribute(bool includeInactive = false, bool executeInPlayMode = false) : base(includeInactive, executeInPlayMode)
        {
        }

        public GetComponentInChildrenAttribute(string name, bool includeInactive = false, bool executeInPlayMode = false) :
            base(name, includeInactive, executeInPlayMode)
        {
        }
    }
}