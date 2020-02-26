namespace MightyAttributes
{
    public class FindAssetAttribute : BaseSearchAssetAttribute
    {
        public FindAssetAttribute(bool executeInPlayMode = false) : base(executeInPlayMode)
        {
        }

        public FindAssetAttribute(string name, bool executeInPlayMode = false) : base(name, executeInPlayMode)
        {
        }
    }
}