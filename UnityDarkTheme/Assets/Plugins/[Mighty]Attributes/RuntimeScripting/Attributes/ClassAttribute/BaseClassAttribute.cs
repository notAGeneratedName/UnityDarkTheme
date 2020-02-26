using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public abstract class BaseClassAttribute : BaseMightyAttribute
    {
    }
}