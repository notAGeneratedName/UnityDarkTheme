namespace MightyAttributes
{
    public class ShowNonSerializedAttribute : BaseDrawerAttribute
    {
        public bool Enabled { get; }

        public bool DrawPrettyName { get; }

        public ShowNonSerializedAttribute(bool enabled = false, bool drawPrettyName = true, FieldOption option = FieldOption.Nothing) :
            base(option)
        {
            Enabled = enabled;
            DrawPrettyName = drawPrettyName;
        }
    }
}