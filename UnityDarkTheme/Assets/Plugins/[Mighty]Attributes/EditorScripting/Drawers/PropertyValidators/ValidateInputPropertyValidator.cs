#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ValidateInputAttribute))]
    public class ValidateInputPropertyValidator : BasePropertyValidator, IRefreshDrawer
    {
        private readonly CallbackSignature m_validateCallbackSignature = new CallbackSignature(typeof(bool));

        private readonly MightyCache<(bool, MightyMethod<bool>, MightyInfo<object>)> m_validateInputCache =
            new MightyCache<(bool, MightyMethod<bool>, MightyInfo<object>)>();

        public override void ValidateProperty(BaseMightyMember mightyMember, BaseValidatorAttribute baseAttribute)
        {
            var property = mightyMember.Property;
            var validateInputAttribute = (ValidateInputAttribute) baseAttribute;

            if (!m_validateInputCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (valid, validateCallback, propertyFieldInfo) = m_validateInputCache[mightyMember];

            if (valid)
            {
                if (!validateCallback.Invoke(propertyFieldInfo.Value))
                    EditorDrawUtility.DrawHelpBox(string.IsNullOrEmpty(validateInputAttribute.Message)
                        ? $"{property.name} is not valid"
                        : validateInputAttribute.Message, MessageType.Error);
            }
            else
                EditorDrawUtility.DrawHelpBox(
                    $@"{typeof(ValidateInputAttribute).Name
                        } needs a callback with boolean return type and a single parameter of the same type as the field");
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (ValidateInputAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<ValidateInputAttribute>();
            var property = mightyMember.Property;

            m_validateCallbackSignature.SetParamsType(mightyMember.PropertyType);

            var valid = property.GetMightyMethod<bool>(target, attribute.CallbackName, m_validateCallbackSignature,
                out var validateCallback);

            MightyInfo<object> propertyFieldInfo = null;
            valid = valid && property.GetInfoFromMember(target, property.name, out propertyFieldInfo);

            m_validateInputCache[mightyMember] = (valid, validateCallback, propertyFieldInfo);
        }

        public override void ClearCache() => m_validateInputCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_validateInputCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, _, propertyFieldInfo) = m_validateInputCache[mightyMember];

            if (valid) propertyFieldInfo.RefreshValue();
        }
    }
}
#endif