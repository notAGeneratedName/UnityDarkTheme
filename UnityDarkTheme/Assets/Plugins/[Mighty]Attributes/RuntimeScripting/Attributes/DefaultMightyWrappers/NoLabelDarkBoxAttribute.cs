namespace MightyAttributes
{
    [DarkBox("BoxName", false, false, true)]
    public class NoLabelDarkBoxAttribute : MightyWrapperAttribute
    {
        public string BoxName { get; }

        public NoLabelDarkBoxAttribute(string boxName) => BoxName = boxName;
    }
}