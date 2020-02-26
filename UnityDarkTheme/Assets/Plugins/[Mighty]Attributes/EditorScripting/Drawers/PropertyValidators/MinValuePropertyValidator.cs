#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(MinValueAttribute))]
    public class MinValuePropertyValidator : BasePropertyValidator
    {
        public override void ValidateProperty(BaseMightyMember mightyMember, BaseValidatorAttribute baseAttribute)
        {
            var property = mightyMember.Property;
            var minValueAttribute = (MinValueAttribute) baseAttribute;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (property.intValue < minValueAttribute.MinValue) property.intValue = (int) minValueAttribute.MinValue;
                    break;
                case SerializedPropertyType.Float:
                    if (property.floatValue < minValueAttribute.MinValue) property.floatValue = minValueAttribute.MinValue;
                    break;
                default:
                    EditorDrawUtility.DrawHelpBox($"{typeof(MinValueAttribute).Name} can be used only on int or float fields");
                    break;
            }
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif