#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(LayerNameAttribute))]
    public class LayerNameDrawer : BaseAutoValueDrawer
    {
        protected override InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var layerNameAttribute = (LayerNameAttribute) baseAttribute;

            if (property.propertyType != SerializedPropertyType.Integer)
                return new InitState(false, "\"" + property.displayName + "\" should be of type int");

            property.intValue = layerNameAttribute.LayerId;
            return new InitState(true);
        }
    }
}
#endif