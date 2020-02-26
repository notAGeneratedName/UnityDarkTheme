namespace MightyAttributes
{
    public abstract class BaseDrawerAttribute : BaseFieldAttribute
    {
        public FieldOption Option { get; set; }

        protected BaseDrawerAttribute(FieldOption option) => Option = option;
    }
}