using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class BaseMethodAttribute : BaseMightyAttribute
    {
        public bool ExecuteInPlayMode { get; }
        
        protected BaseMethodAttribute(bool executeInPlayMode) => ExecuteInPlayMode = executeInPlayMode;
    }
}