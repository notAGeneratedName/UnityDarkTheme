namespace MightyAttributes
{
    public abstract class BaseAutoValueAttribute : BaseFieldAttribute
    {
        public bool ExecuteInPlayMode { get; }

        protected BaseAutoValueAttribute(bool executeInPlayMode) => ExecuteInPlayMode = executeInPlayMode;
    }
}