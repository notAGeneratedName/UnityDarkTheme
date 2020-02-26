namespace MightyAttributes
{
    public class OnInspectorGUIAttribute : BaseOnInspectorGUIMethodAttribute
    {
        public OnInspectorGUIAttribute(bool executeInPlayMode = false) : base(executeInPlayMode)
        {
        }
    }
}