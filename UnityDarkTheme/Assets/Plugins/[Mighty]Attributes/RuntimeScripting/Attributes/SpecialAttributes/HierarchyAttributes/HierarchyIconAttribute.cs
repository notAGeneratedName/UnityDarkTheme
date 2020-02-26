namespace MightyAttributes
{
    public class HierarchyIconAttribute : BaseHierarchyAttribute
    {
        public string IconPath { get; }

        public bool PathAsCallback { get; }

        public HierarchyIconAttribute(string iconPath, int priority = 0) : base(priority)
        {
            IconPath = iconPath;
            PathAsCallback = false;
        }

        public HierarchyIconAttribute(string iconPath, bool pathAsCallback, int priority = 0) : base(priority)
        {
            IconPath = iconPath;
            PathAsCallback = pathAsCallback;
        }
    }
}