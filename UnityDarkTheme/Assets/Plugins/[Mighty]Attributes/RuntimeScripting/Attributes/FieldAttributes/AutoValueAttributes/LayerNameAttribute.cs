using UnityEngine;

namespace MightyAttributes
{
    public class LayerNameAttribute : BaseAutoValueAttribute
    {
        public readonly int LayerId;

        public LayerNameAttribute(string layerName, bool executeInPlayMode = false) : base(executeInPlayMode)
        {
            LayerId = LayerMask.NameToLayer(layerName);
        }
    }
}