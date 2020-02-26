using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MightyWrapperAttribute : BaseMightyAttribute
    {
    }
}