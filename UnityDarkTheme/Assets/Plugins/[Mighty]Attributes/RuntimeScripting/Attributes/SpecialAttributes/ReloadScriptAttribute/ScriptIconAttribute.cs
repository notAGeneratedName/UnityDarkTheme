namespace MightyAttributes
{
    public class ScriptIconAttribute : BaseReloadScriptAttribute
    {
        public string IconPath { get; }

        public int Priority { get; }

        public ScriptIconAttribute(string iconPath, int priority = 0)
        {
            IconPath = iconPath;
            Priority = priority;
        }
    }
}