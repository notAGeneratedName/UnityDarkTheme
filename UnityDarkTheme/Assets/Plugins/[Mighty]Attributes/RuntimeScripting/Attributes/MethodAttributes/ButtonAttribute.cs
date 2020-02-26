using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : BaseOnInspectorGUIMethodAttribute
    {
        public string Text { get; }

        public string EnabledCallback { get; }

        public int Height { get; }

        public ButtonAttribute(string text = null, string enabledCallback = null, int height = 20, bool executeInPlayMode = true) : base(
            executeInPlayMode)
        {
            Text = text;
            EnabledCallback = enabledCallback;
            Height = height;
        }
    }
}