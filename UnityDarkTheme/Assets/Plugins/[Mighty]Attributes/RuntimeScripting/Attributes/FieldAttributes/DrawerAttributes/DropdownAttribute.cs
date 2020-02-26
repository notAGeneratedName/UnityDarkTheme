namespace MightyAttributes
{
    public class DropdownAttribute : BaseDrawerAttribute
    {
        public string ValuesFieldName { get; }

        public DropdownAttribute(string valuesFieldName, FieldOption option = FieldOption.Nothing) : base(option) =>
            ValuesFieldName = valuesFieldName;
    }
}