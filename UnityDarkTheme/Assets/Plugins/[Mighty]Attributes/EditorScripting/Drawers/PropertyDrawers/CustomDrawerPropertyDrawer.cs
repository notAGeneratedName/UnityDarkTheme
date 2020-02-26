#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(CustomDrawerAttribute))]
    public class CustomDrawerPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        #region Signatures

        private readonly CallbackSignature m_fieldCallback = new CallbackSignature(typeof(object), typeof(string), typeof(object));

        private readonly CallbackSignature m_propertyCallback = new CallbackSignature(null, typeof(SerializedProperty));

        private readonly CallbackSignature m_elementCallback = new CallbackSignature(null,
            typeof(SerializedProperty), typeof(SerializedProperty), typeof(int));

        private readonly CallbackSignature m_labelledElementCallback = new CallbackSignature(null,
            typeof(GUIContent), typeof(SerializedProperty), typeof(SerializedProperty), typeof(int));

        private readonly CallbackSignature m_rectElementCallback = new CallbackSignature(null,
            typeof(Rect), typeof(SerializedProperty), typeof(SerializedProperty), typeof(int));

        private readonly CallbackSignature m_elementHeightCallback = new CallbackSignature(typeof(float),
            typeof(SerializedProperty), typeof(SerializedProperty));

        #endregion /Signatures

        private readonly MightyCache<CallbackSignature, MightyMethod<object>> m_customDrawerCache =
            new MightyCache<CallbackSignature, MightyMethod<object>>();

        public object DrawField(FieldInfo fieldInfo, Object context, object value, CustomDrawerAttribute attribute)
        {
            if (!GetDrawerForMember(fieldInfo.GetUniqueID(), m_fieldCallback, out var drawerMethod))
            {
                context.GetMightyMethod<object>(attribute.DrawerCallback, m_fieldCallback, out var mightyMethod);
                m_customDrawerCache[fieldInfo.GetUniqueID(), m_fieldCallback] = mightyMethod;
            }

            return InvokeDrawer(drawerMethod, $"object {attribute.DrawerCallback}(string label, object value)",
                fieldInfo.Name.DrawPrettyName(), value);
        }

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            GetDrawerForMember(mightyMember, m_propertyCallback, out var drawerMethod, baseAttribute);

            InvokeDrawer(drawerMethod, $"void {((CustomDrawerAttribute) baseAttribute).DrawerCallback}(SerializedProperty property)",
                property);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            GetDrawerForMember(mightyMember, m_elementCallback, out var drawerMethod, baseAttribute);

            InvokeDrawer(drawerMethod,
                $"void {((CustomDrawerAttribute) baseAttribute).DrawerCallback}(SerializedProperty property, SerializedProperty element, int index)",
                mightyMember.Property, mightyMember.GetElement(index), index);
        }

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            GetDrawerForMember(mightyMember, m_elementCallback, out var drawerMethod, baseAttribute);

            InvokeDrawer(drawerMethod,
                $"void {((CustomDrawerAttribute) baseAttribute).DrawerCallback}(GUIContent label, SerializedProperty property, SerializedProperty element, int index)",
                label, mightyMember.Property, mightyMember.GetElement(index), index);
        }

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            GetDrawerForMember(mightyMember, m_elementCallback, out var drawerMethod, baseAttribute);

            InvokeDrawer(drawerMethod,
                $"void {((CustomDrawerAttribute) baseAttribute).DrawerCallback}(Rect rect, SerializedProperty property, SerializedProperty element, int index)",
                rect, mightyMember.Property, mightyMember.GetElement(index), index);
        }

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (string.IsNullOrWhiteSpace(((CustomDrawerAttribute) baseAttribute).ElementHeightCallback)) return 0;

            if (!GetDrawerForMember(mightyMember, m_elementCallback, out var callback, baseAttribute)) return 0;

            return (float) callback.Invoke();
        }

        private object InvokeDrawer(MightyMethod<object> drawerMethod, string signature, params object[] parameters)
        {
            if (drawerMethod != null) return drawerMethod.Invoke(parameters);
            EditorDrawUtility.DrawHelpBox($"Callback is invalid, it should be like this: \"{signature}\"");
            return null;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (CustomDrawerAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<CustomDrawerAttribute>();

            if (!mightyMember.Property.isArray || mightyMember.Property.propertyType == SerializedPropertyType.String)
                InitCallback(mightyMember, target, attribute, m_propertyCallback);
            else
            {
                InitCallback(mightyMember, target, attribute, m_elementCallback);
                InitCallback(mightyMember, target, attribute, m_labelledElementCallback);
                InitCallback(mightyMember, target, attribute, m_rectElementCallback);
                InitCallback(mightyMember, target, attribute, m_elementHeightCallback);
            }
        }

        private void InitCallback(BaseMightyMember member, object target, CustomDrawerAttribute attribute, CallbackSignature signature)
        {
            member.Property.GetMightyMethod<object>(target, attribute.DrawerCallback, signature, out var mightyMethod);
            m_customDrawerCache[member, signature] = mightyMethod;
        }

        private bool GetDrawerForMember(BaseMightyMember member, CallbackSignature signature, out MightyMethod<object> drawerMethod,
            BaseDrawerAttribute attribute)
        {
            if (GetDrawerForMember(member.ID, signature, out drawerMethod)) return true;
            InitDrawer(member, attribute);
            return GetDrawerForMember(member.ID, signature, out drawerMethod);
        }

        private bool GetDrawerForMember(long id, CallbackSignature signature, out MightyMethod<object> drawerMethod) =>
            m_customDrawerCache.TryGetValue(id, signature, out drawerMethod);

        public override void ClearCache() => m_customDrawerCache.ClearCache();
    }
}
#endif