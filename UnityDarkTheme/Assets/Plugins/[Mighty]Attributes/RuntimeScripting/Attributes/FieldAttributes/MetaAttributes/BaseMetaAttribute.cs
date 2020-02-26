using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true)]
    public abstract class BaseMetaAttribute : BaseFieldAttribute
    {
    }
}
