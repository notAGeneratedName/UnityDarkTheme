namespace MightyAttributes
{
    public class NestAttribute : BaseDrawerAttribute
    {
        public NestOption NestOption { get; } = NestOption.Nothing;

        public string OptionCallback { get; }

        public NestAttribute(NestOption option = NestOption.Nothing) : base((FieldOption) option) =>
            NestOption = option;

        public NestAttribute(string optionCallback) : base(FieldOption.Nothing) => OptionCallback = optionCallback;
    }
}