namespace MightyAttributes
{
    public class GetComponentsInChildrenWithTagAttribute : TagObjectAttribute
    {
        public GetComponentsInChildrenWithTagAttribute(string tag, bool includeInactive = false, bool executeInPlayMode = false) : 
            base(tag, includeInactive, executeInPlayMode)
        {
        }
    }
}