namespace MightyAttributes
{
    public class GetComponentInChildrenWithLayerAttribute : LayerObjectAttribute
    {
        public GetComponentInChildrenWithLayerAttribute(string layer, bool includeInactive = false, bool executeInPlayMode = false) :
            base(layer, includeInactive, executeInPlayMode)
        {
        }
    }
}