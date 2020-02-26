namespace MightyAttributes
{
    public abstract class BaseOnInspectorGUIMethodAttribute : BaseMethodAttribute
    {
        protected BaseOnInspectorGUIMethodAttribute(bool executeInPlayMode) : base(executeInPlayMode)
        {
        }
    }
}