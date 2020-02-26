namespace MightyAttributes
{
    [DarkBox("GroupName", nameAsCallback: true), ButtonArray(ArrayOption.ContentOnly)]
    public class DarkButtonArrayAttribute : MightyWrapperAttribute
    {
        public string GroupName { get; }
        
        public DarkButtonArrayAttribute(string groupName) => GroupName = groupName;
    }
}