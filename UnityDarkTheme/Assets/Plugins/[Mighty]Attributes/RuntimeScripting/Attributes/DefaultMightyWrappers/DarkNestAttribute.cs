namespace MightyAttributes
{
    [DarkBox("BoxName", false, false, true)]
    public class DarkNestAttribute : LineNestAttribute
    {
        private string BoxName { get; }

        public DarkNestAttribute(string linePosition = null, string boxName = null,
            NestOption option = NestOption.Nothing) : base(linePosition, option) => BoxName = boxName;
    }
}