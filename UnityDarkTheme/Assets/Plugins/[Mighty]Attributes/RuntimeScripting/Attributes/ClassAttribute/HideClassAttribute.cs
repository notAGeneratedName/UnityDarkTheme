namespace MightyAttributes
{
    public class HideClassAttribute : HideStatusClassAttribute
    {
        public HideClassAttribute(HideStatus hideStatus = HideStatus.All) : base(hideStatus){}
    }

    public class HideScriptFieldAttribute : HideStatusClassAttribute
    {
        public HideScriptFieldAttribute(HideStatus hideStatus = HideStatus.ScriptField) : base(hideStatus){}
    }
}