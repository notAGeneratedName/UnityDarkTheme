using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class BaseHierarchyAttribute : BaseSpecialAttribute
    {
        public int Priority { get; }
        
        protected BaseHierarchyAttribute(int priority) => Priority = priority;
    }
}