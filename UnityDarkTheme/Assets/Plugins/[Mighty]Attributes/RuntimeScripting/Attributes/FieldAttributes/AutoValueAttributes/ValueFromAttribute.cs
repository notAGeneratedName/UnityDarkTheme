namespace MightyAttributes
{
    public class ValueFromAttribute : BaseAutoValueAttribute
    {
        public readonly string ValueName;

        public ValueFromAttribute(string valueName, bool executeInPlayMode = false) : base(executeInPlayMode)
        {
            ValueName = valueName;
        }
    }
}