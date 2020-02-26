#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(WidthAttribute))]
    public class WidthDecoratorDrawer : BaseDecoratorDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(MightyInfo<float>, MightyInfo<float?>)> m_widthCache =
            new MightyCache<(MightyInfo<float>, MightyInfo<float?>)>();
        
        private readonly MightyCache<(float, float)> m_previousWidthCache = new MightyCache<(float, float)>();

        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            SetWidthsForMember(mightyMember, (EditorGUIUtility.labelWidth, EditorGUIUtility.fieldWidth));

            if (!m_widthCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (labelWidth, fieldWidth) = m_widthCache[mightyMember];

            EditorGUIUtility.labelWidth = labelWidth.Value;
            if (fieldWidth.Value != null) EditorGUIUtility.fieldWidth = (float) fieldWidth.Value;
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) =>
            (EditorGUIUtility.labelWidth, EditorGUIUtility.fieldWidth) = GetWidthsForProperty(mightyMember);

        private void SetWidthsForMember(BaseMightyMember mightyMember, (float, float) widths) =>
            m_previousWidthCache[mightyMember] = widths;

        private (float, float) GetWidthsForProperty(BaseMightyMember mightyMember) =>
            m_previousWidthCache.TryGetValue(mightyMember, out var value) ? value : (0, 0);

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (WidthAttribute) mightyAttribute;
            var property = mightyMember.Property;
            var target = mightyMember.InitAttributeTarget<WidthAttribute>();

            if (!property.GetInfoFromMember<float>(target, attribute.LabelWidthCallback, out var labelWidthInfo))
                labelWidthInfo = new MightyInfo<float>(attribute.LabelWidth);

            if (!property.GetInfoFromMember<float?>(target, attribute.FieldWidthCallback, out var fieldWidthInfo))
                fieldWidthInfo = new MightyInfo<float?>(attribute.FieldWidth);
            
            m_widthCache[mightyMember] = (labelWidthInfo, fieldWidthInfo);
        }

        public override void ClearCache()
        {
            m_widthCache.ClearCache();
            m_previousWidthCache.ClearCache();
        }

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_widthCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }
            var (labelWidth, fieldWidth) = m_widthCache[mightyMember];
            labelWidth.RefreshValue();
            fieldWidth.RefreshValue();
        }
    }
}
#endif