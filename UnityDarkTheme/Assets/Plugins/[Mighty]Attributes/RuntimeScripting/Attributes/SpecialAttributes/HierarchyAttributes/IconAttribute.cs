namespace MightyAttributes
{
    public class IconAttribute : BaseHierarchyAttribute
    {
        public string IconPath { get; }
        
        public IconAttribute(string iconPath, int priority = 0) : base(priority) => IconPath = iconPath;
    }
}