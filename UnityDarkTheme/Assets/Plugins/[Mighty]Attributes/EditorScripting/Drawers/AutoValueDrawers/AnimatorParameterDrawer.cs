#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterDrawer : BaseAutoValueDrawer
    {
        protected override InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var animatorParameter = (AnimatorParameterAttribute) baseAttribute;
            var attributeTarget = property.GetAttributeTarget<AnimatorParameterAttribute>();

            if (property.propertyType != SerializedPropertyType.Integer)
                return new InitState(false, "\"" + property.displayName + "\" should be of type int");

            var name = animatorParameter.ParameterName;
            if (animatorParameter.NameAsCallback && property.GetValueFromMember(attributeTarget, name, out string nameValue))
                name = nameValue;

            property.intValue = Animator.StringToHash(name);
            return new InitState(true);
        }
    }
}
#endif