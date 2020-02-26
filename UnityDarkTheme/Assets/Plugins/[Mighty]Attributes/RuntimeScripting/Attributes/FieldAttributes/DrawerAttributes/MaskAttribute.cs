namespace MightyAttributes
{
    public class MaskAttribute : BaseDrawerAttribute
    {
        public string[] MaskNames { get; }

        public string MaskNamesCallback { get; }

        public MaskAttribute(params string[] maskNames) : base(FieldOption.Nothing) => MaskNames = maskNames;

        public MaskAttribute(string[] maskNames, FieldOption option) : base(option) => MaskNames = maskNames;

        public MaskAttribute(string maskNamesCallback, FieldOption option = FieldOption.Nothing) : base(option) =>
            MaskNamesCallback = maskNamesCallback;
    }
}