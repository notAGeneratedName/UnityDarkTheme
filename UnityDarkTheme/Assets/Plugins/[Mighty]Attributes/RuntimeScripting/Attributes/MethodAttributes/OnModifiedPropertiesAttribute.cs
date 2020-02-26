namespace MightyAttributes
{
    public class OnModifiedPropertiesAttribute : BaseMethodAttribute
    {
        public OnModifiedPropertiesAttribute(bool executeInPlayMode = false) : base(executeInPlayMode)
        {
        }
    }
}