namespace MightyAttributes
{
    public class FindAssetsInFoldersAttribute : BaseSearchAssetAttribute
    {
        public readonly string[] Folders;

        public FindAssetsInFoldersAttribute(params string[] folders) : base(false)
        {
            Folders = folders;
        }
        
        public FindAssetsInFoldersAttribute(string[] folders, bool executeInPlayMode = false) : base(executeInPlayMode)
        {
            Folders = folders;
        }

        public FindAssetsInFoldersAttribute(string name, string[] folders, bool executeInPlayMode = false) : base(name, executeInPlayMode)
        {
            Folders = folders;
        }
    }
}