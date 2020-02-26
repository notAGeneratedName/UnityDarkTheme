#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ArraySizeAttribute))]
    public class ArraySizeDrawer : BaseAutoValueDrawer
    {
        protected override InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            if (!property.isArray) return new InitState(false, "\"" + property.displayName + "\" should be an array");

            var attribute = (ArraySizeAttribute) baseAttribute;
            var attributeTarget = property.GetAttributeTarget<ArraySizeAttribute>();

            if (!property.GetValueFromMember(attributeTarget, attribute.SizeCallback, out int size))
                size = attribute.Size;
            
            if (size != property.arraySize)
            {
                property.arraySize = size;
                property.serializedObject.ApplyModifiedProperties();
            }
            return new InitState(true);
        }
    }
}
#endif