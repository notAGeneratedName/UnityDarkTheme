namespace MightyAttributes
{
    public class GetComponentsInChildrenAttribute : NameObjectAttribute
    {
        public GetComponentsInChildrenAttribute(bool includeInactive = true, bool executeInPlayMode = false) : base(includeInactive, executeInPlayMode)
        {
        }

        public GetComponentsInChildrenAttribute(string name, bool includeInactive = true, bool executeInPlayMode = false) :
            base(name, includeInactive, executeInPlayMode)
        {
        }
    }
}