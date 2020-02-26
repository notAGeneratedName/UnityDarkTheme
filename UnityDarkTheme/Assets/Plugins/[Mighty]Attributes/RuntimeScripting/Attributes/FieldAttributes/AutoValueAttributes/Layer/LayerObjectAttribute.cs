namespace MightyAttributes
{
    public class LayerObjectAttribute : BaseSearchObjectAttribute
    {
        public readonly string Layer;
        
        public LayerObjectAttribute(string layer, bool includeInactive, bool executeInPlayMode) : base(includeInactive, executeInPlayMode)
        {
            Layer = layer;
        }
    }
}