#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(InfoBoxAttribute))]
    public class InfoBoxPropertyMeta : BasePropertyMeta, IRefreshDrawer
    {
        private readonly MightyCache<MightyInfo<bool>> m_infoBoxCache = new MightyCache<MightyInfo<bool>>();

        public override void ApplyPropertyMeta(BaseMightyMember mightyMember, BaseMetaAttribute metaAttribute)
        {
            var infoBoxAttribute = (InfoBoxAttribute) metaAttribute;

            if (!m_infoBoxCache.Contains(mightyMember)) InitDrawer(mightyMember, metaAttribute);
            var visibleInfo = m_infoBoxCache[mightyMember];

            if (visibleInfo.Value)
                DrawInfoBox(infoBoxAttribute.Text, infoBoxAttribute.Type);
            else
                EditorDrawUtility.DrawHelpBox($"{typeof(InfoBoxAttribute).Name} needs a valid boolean condition to work");
        }

        private void DrawInfoBox(string infoText, InfoBoxType infoBoxType)
        {
            switch (infoBoxType)
            {
                case InfoBoxType.Normal:
                    EditorDrawUtility.DrawHelpBox(infoText, MessageType.Info);
                    break;

                case InfoBoxType.Warning:
                    EditorDrawUtility.DrawHelpBox(infoText);
                    break;

                case InfoBoxType.Error:
                    EditorDrawUtility.DrawHelpBox(infoText, MessageType.Error);
                    break;
            }
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (InfoBoxAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<InfoBoxAttribute>();

            if (!mightyMember.Property.GetBoolInfo(target, attribute.VisibleIf, out var visibleInfo))
                visibleInfo = new MightyInfo<bool>(true);

            m_infoBoxCache[mightyMember] = visibleInfo;
        }

        public override void ClearCache() => m_infoBoxCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_infoBoxCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            m_infoBoxCache[mightyMember].RefreshValue();
        }
    }
}
#endif