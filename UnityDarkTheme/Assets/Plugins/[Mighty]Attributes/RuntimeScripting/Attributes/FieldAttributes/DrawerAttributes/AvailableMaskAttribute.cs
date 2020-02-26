namespace MightyAttributes
{
    public class AvailableMaskAttribute : BaseDrawerAttribute
    {
        public int AvailableMask { get; }

        public string AvailableMaskName { get; }

        public bool AllowEverything { get; }

        public AvailableMaskAttribute(string availableMaskName, bool allowEverything = true, FieldOption option = FieldOption.Nothing) :
            base(option)
        {
            AvailableMaskName = availableMaskName;
            AllowEverything = allowEverything;
        }

        public AvailableMaskAttribute(int availableMask, bool allowEverything = true, FieldOption option = FieldOption.Nothing) :
            base(option)
        {
            AvailableMask = availableMask;
            AllowEverything = allowEverything;
        }
    }
}