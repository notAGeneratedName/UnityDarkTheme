namespace MightyAttributes
{
    public class FindAssetsAttribute : BaseSearchAssetAttribute
    {
        public FindAssetsAttribute(bool executeInPlayMode = false) : base(executeInPlayMode)
        {
        }
        
        public FindAssetsAttribute(string name, bool executeInPlayMode = false) : base(name, executeInPlayMode)
        {
        }
    }
}