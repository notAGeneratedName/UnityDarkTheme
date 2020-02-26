namespace MightyAttributes
{
    public class FindObjectWithLayerAttribute : LayerObjectAttribute
    {
        public FindObjectWithLayerAttribute(string layer, bool includeInactive = false, bool executeInPlayMode = false) :
            base(layer, includeInactive, executeInPlayMode)
        {
        }
    }
}