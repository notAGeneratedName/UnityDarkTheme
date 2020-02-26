namespace MightyAttributes
{
    public class OnValueChangedAttribute : BaseMetaAttribute
    {
        public string CallbackName { get; }

        public OnValueChangedAttribute(string callbackName) => CallbackName = callbackName;
    }
}
