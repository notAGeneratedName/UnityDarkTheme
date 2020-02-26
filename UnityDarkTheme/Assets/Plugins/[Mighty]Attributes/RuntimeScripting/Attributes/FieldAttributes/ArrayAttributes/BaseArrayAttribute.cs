namespace MightyAttributes
{
    public abstract class BaseArrayAttribute : BaseFieldAttribute
    {
        public ArrayOption Option { get; }
        
        public string OptionName { get; }

        protected BaseArrayAttribute(ArrayOption option) => Option = option;

        protected BaseArrayAttribute(string optionName) => OptionName = optionName;
    }
}