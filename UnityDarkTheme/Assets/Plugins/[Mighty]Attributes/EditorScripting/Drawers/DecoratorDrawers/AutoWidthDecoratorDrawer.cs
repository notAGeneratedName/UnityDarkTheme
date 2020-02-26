#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(AutoWidthAttribute))]
    public class AutoWidthDecoratorDrawer : BaseDecoratorDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(float, MightyInfo<float?>)> m_autoWidthCache = new MightyCache<(float, MightyInfo<float?>)>();
        
        private readonly MightyCache<(float, float)> m_previousWidthCache = new MightyCache<(float, float)>();

        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            SetWidthsForMember(mightyMember, (EditorGUIUtility.labelWidth, EditorGUIUtility.fieldWidth));

            if (!m_autoWidthCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (labelWidth, fieldWidth) = m_autoWidthCache[mightyMember];

            EditorGUIUtility.labelWidth = labelWidth;
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
            var attribute = (AutoWidthAttribute) mightyAttribute;
            var property = mightyMember.Property;
            var target = mightyMember.InitAttributeTarget<AutoWidthAttribute>();

            var labelWidthInfo = EditorDrawUtility.TextWidth(property.displayName);

            if (!property.GetInfoFromMember<float?>(target, attribute.FieldWidthCallback, out var fieldWidthInfo))
                fieldWidthInfo = new MightyInfo<float?>(attribute.FieldWidth);
            
            m_autoWidthCache[mightyMember] = (labelWidthInfo, fieldWidthInfo);
        }

        public override void ClearCache()
        {
            m_autoWidthCache.ClearCache();
            m_previousWidthCache.ClearCache();
        }

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_autoWidthCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }
            m_autoWidthCache[mightyMember].Item2.RefreshValue();
        }
    }
}
#endif