namespace MightyAttributes
{
    [Box("BoxName", false, false, true)]
    public class NoLabelBoxAttribute : MightyWrapperAttribute
    {
        public string BoxName { get; }

        public NoLabelBoxAttribute(string boxName) => BoxName = boxName;
    }
}