#if UNITY_EDITOR
using System.Reflection;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ShowNativePropertyAttribute))]
    public class ShowNativePropertyNativePropertyDrawer : BaseNativePropertyDrawer, IRefreshDrawer
    {
        private readonly MightyCache<MightyInfo<object>> m_showNativeCache = new MightyCache<MightyInfo<object>>();

        public override void DrawNativeProperty(MightyMember<PropertyInfo> mightyMember, BasePropertyAttribute baseAttribute)
        {
            var property = mightyMember.MemberInfo;

            if (!m_showNativeCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var value = m_showNativeCache[mightyMember].Value;

            if (value == null)
                EditorDrawUtility.DrawHelpBox($"{typeof(ShowNativePropertyNativePropertyDrawer).Name} doesn't support Reference types");
            else if (!EditorDrawUtility.DrawLayoutField(property.Name, value, false))
                EditorDrawUtility.DrawHelpBox(
                    $"{typeof(ShowNativePropertyNativePropertyDrawer).Name} doesn't support {property.PropertyType.Name} types");
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var property = (MightyMember<PropertyInfo>) mightyMember;
            m_showNativeCache[mightyMember] =
                new MightyInfo<object>(property.Target, property.MemberInfo, property.MemberInfo.GetValue(mightyMember.Target));
        }

        public override void ClearCache() => m_showNativeCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_showNativeCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            m_showNativeCache[mightyMember].RefreshValue();
        }
    }
}
#endif