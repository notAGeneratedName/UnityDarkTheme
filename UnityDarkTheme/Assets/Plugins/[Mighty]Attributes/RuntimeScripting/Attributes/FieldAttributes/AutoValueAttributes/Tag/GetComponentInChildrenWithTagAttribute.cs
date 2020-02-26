namespace MightyAttributes
{
    public class GetComponentInChildrenWithTagAttribute : TagObjectAttribute
    {
        public GetComponentInChildrenWithTagAttribute(string tag, bool includeInactive = false, bool executeInPlayMode = false) :
            base(tag, includeInactive, executeInPlayMode)
        {
        }
    }
}