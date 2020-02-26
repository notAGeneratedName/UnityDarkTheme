#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(RequiredAttribute))]
    public class RequiredPropertyValidator : BasePropertyValidator
    {
        public override void ValidateProperty(BaseMightyMember mightyMember, BaseValidatorAttribute baseAttribute)
        {
            var property = mightyMember.Property;
            var requiredAttribute = (RequiredAttribute) baseAttribute;

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null) return;
                
                var errorMessage = $"{property.name} is required";
                if (!string.IsNullOrEmpty(requiredAttribute.Message)) errorMessage = requiredAttribute.Message;

                EditorDrawUtility.DrawHelpBox(errorMessage, MessageType.Error, property.GetTargetObject(), true);
            }
            else
                EditorDrawUtility.DrawHelpBox($"{typeof(RequiredAttribute).Name} works only on reference types");
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