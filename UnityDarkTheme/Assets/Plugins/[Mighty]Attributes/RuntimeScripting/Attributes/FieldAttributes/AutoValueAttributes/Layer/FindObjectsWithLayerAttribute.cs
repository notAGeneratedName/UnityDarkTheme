namespace MightyAttributes
{
    public class FindObjectsWithLayerAttribute : LayerObjectAttribute
    {
        public FindObjectsWithLayerAttribute(string layer, bool includeInactive = false, bool executeInPlayMode = false) :
            base(layer, includeInactive, executeInPlayMode)
        {
        }
    }
}