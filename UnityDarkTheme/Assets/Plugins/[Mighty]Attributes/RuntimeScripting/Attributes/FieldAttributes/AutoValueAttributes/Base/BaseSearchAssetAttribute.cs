namespace MightyAttributes
{
    public abstract class BaseSearchAssetAttribute : BaseAutoValueAttribute
    {
        public readonly string Name;

        protected BaseSearchAssetAttribute(bool executeInPlayMode) : base(executeInPlayMode)
        {
        }

        protected BaseSearchAssetAttribute(string name, bool executeInPlayMode) : base(executeInPlayMode)
        {
            Name = name;
        }
    }
}