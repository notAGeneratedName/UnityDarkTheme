namespace MightyAttributes
{
    public class ItemNamesAttribute : BaseArrayAttribute
    {
        public string ItemNamesCallback { get; }
        
        public ItemNamesAttribute(string itemNamesCallback, ArrayOption option = ArrayOption.Nothing) : base(option) => 
            ItemNamesCallback = itemNamesCallback;

        public ItemNamesAttribute(string itemNamesCallback, string optionName) : base(optionName) => ItemNamesCallback = itemNamesCallback;
    }
}
