using System;

namespace MightyAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field)]
    public class CallbackNameAttribute : BaseMightyAttribute
    {
        
    }
}