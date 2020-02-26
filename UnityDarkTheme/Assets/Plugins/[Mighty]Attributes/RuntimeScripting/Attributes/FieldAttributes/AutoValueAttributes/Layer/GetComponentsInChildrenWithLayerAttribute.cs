namespace MightyAttributes
{
    public class GetComponentsInChildrenWithLayerAttribute : LayerObjectAttribute
    {
        public GetComponentsInChildrenWithLayerAttribute(string layer, bool includeInactive = false, bool executeInPlayMode = false) :
            base(layer, includeInactive, executeInPlayMode)
        {
        }
    }
}