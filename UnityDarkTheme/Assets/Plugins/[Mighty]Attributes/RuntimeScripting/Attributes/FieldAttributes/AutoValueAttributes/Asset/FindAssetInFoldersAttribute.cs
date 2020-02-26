namespace MightyAttributes
{
    public class FindAssetInFoldersAttribute : BaseSearchAssetAttribute
    {
        public readonly string[] Folders;
        
        public FindAssetInFoldersAttribute(params string[] folders) : base(false)
        {
            Folders = folders;
        }
        
        public FindAssetInFoldersAttribute(string[] folders, bool executeInPlayMode = false) : base(executeInPlayMode)
        {
            Folders = folders;
        }

        public FindAssetInFoldersAttribute(string name, string[] folders, bool executeInPlayMode = false) : base(name, executeInPlayMode)
        {
            Folders = folders;
        }
    }
}