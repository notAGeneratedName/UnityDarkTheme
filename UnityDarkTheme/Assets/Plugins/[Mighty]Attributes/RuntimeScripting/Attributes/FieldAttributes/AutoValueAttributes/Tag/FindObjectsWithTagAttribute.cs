namespace MightyAttributes
{
    public class FindObjectsWithTagAttribute : TagObjectAttribute
    {
        public FindObjectsWithTagAttribute(string tag, bool includeInactive = false, bool executeInPlayMode = false) : 
            base(tag, includeInactive, executeInPlayMode)
        {
        }
    }
}