using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class BasePropertyAttribute : BaseMightyAttribute
    {
    }
}