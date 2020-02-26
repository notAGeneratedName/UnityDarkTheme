namespace MightyAttributes
{
    public class FindObjectWithTagAttribute : TagObjectAttribute
    {
        public FindObjectWithTagAttribute(string tag, bool includeInactive = false, bool executeInPlayMode = false) : 
            base(tag, includeInactive, executeInPlayMode)
        {
        }
    }
}