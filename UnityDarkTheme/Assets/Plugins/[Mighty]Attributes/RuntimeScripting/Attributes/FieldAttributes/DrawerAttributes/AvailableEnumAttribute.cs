namespace MightyAttributes
{
    public class AvailableEnumAttribute : BaseDrawerAttribute
    {
        public string AvailableValuesName { get; }

        public bool AllowNothing { get; set; }

        public AvailableEnumAttribute(string availableValuesName, bool allowNothing = false, FieldOption option = FieldOption.Nothing) :
            base(option)
        {
            AvailableValuesName = availableValuesName;
            AllowNothing = allowNothing;
        }
    }
}