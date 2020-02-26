namespace MightyAttributes
{
    public class OnEnableAttribute : BaseMethodAttribute, IExcludeFromAutoRun
    {
        public OnEnableAttribute(bool executeInPlayMode = false) : base(executeInPlayMode)
        {
        }
    }
}