namespace MightyAttributes
{
    public abstract class HideStatusClassAttribute : BaseClassAttribute
    {
        public HideStatus HideStatus { get; }

        protected HideStatusClassAttribute(HideStatus hideStatus) => HideStatus = hideStatus;
    }
}