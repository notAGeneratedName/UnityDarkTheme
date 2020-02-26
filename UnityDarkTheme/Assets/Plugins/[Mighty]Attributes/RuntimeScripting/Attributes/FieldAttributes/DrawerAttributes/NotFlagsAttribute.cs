namespace MightyAttributes
{
    public class NotFlagsAttribute : BaseDrawerAttribute
    {
        public bool AllowNothing { get; }

        public NotFlagsAttribute(bool allowNothing = false, FieldOption option = FieldOption.Nothing) : base(option) =>
            AllowNothing = allowNothing;
    }
}