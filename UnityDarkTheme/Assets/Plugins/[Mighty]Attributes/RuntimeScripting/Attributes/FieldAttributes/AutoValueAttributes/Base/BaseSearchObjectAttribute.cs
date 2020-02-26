namespace MightyAttributes
{
    public abstract class BaseSearchObjectAttribute : BaseAutoValueAttribute
    {
        public readonly bool IncludeInactive;

        protected BaseSearchObjectAttribute(bool includeInactive, bool executeInPlayMode) : base(executeInPlayMode)
        {
            IncludeInactive = includeInactive;
        }
    }
}