#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(OnValueChangedAttribute))]
    public class OnValueChangedPropertyMeta : BasePropertyMeta
    {
        private readonly CallbackSignature m_onValueChangedSignature = new CallbackSignature(null);

        private readonly MightyCache<(bool, MightyVoidMethod)> m_onValueChangedCache = new MightyCache<(bool, MightyVoidMethod)>();

        public override void ApplyPropertyMeta(BaseMightyMember mightyMember, BaseMetaAttribute metaAttribute)
        {
            if (!m_onValueChangedCache.Contains(mightyMember)) InitDrawer(mightyMember, metaAttribute);
            var (valid, onValueChangedCallback) = m_onValueChangedCache[mightyMember];
            if (valid)
            {
                mightyMember.Property.serializedObject.ApplyModifiedProperties();
                onValueChangedCallback.Invoke();
            }
            else
                EditorDrawUtility.DrawHelpBox(
                    $"Callback is invalid, it should be like this: \"void {((OnValueChangedAttribute) metaAttribute).CallbackName}()\"",
                    MessageType.Warning, mightyMember.Context);
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (OnValueChangedAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<OnValueChangedAttribute>();

            var valid = mightyMember.Property.GetMightyVoidMethod(target, attribute.CallbackName, m_onValueChangedSignature,
                out var onValueChangedCallback);

            m_onValueChangedCache[mightyMember] = (valid, onValueChangedCallback);
        }

        public override void ClearCache() => m_onValueChangedCache.ClearCache();
    }
}
#endif