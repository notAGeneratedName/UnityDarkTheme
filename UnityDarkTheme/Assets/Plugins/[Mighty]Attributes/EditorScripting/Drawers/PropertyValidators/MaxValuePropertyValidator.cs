#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(MaxValueAttribute))]
    public class MaxValuePropertyValidator : BasePropertyValidator
    {
        public override void ValidateProperty(BaseMightyMember mightyMember, BaseValidatorAttribute baseAttribute)
        {
            var property = mightyMember.Property;
            var maxValueAttribute = (MaxValueAttribute) baseAttribute;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (property.intValue > maxValueAttribute.MaxValue) property.intValue = (int) maxValueAttribute.MaxValue;
                    break;
                case SerializedPropertyType.Float:
                    if (property.floatValue > maxValueAttribute.MaxValue) property.floatValue = maxValueAttribute.MaxValue;
                    break;
                default:
                    EditorDrawUtility.DrawHelpBox($"{typeof(MaxValueAttribute).Name} can be used only on int or float fields");
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