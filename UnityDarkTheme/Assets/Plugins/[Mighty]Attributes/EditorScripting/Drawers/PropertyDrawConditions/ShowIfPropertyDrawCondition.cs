#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawCondition : BasePropertyDrawCondition, IRefreshDrawer
    {
        private readonly MightyCache<(bool, bool, MightyInfo<bool>[])> m_showIfCache = new MightyCache<(bool, bool, MightyInfo<bool>[])>();

        public override bool CanDrawProperty(BaseMightyMember mightyMember, BaseDrawConditionAttribute baseAttribute)
        {
            if (!m_showIfCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (valid, canDraw, _) = m_showIfCache[mightyMember];

            if (valid) return canDraw;

            EditorDrawUtility.DrawHelpBox(
                $"{mightyMember.Property.displayName}'s ShowIf needs a valid boolean condition field or method name to work");

            return true;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var target = mightyMember.InitAttributeTarget<ShowIfAttribute>();
            var attribute = (ShowIfAttribute) mightyAttribute;
            var property = mightyMember.Property;

            var canDraw = true;
            var valid = false;
            var infos = new MightyInfo<bool>[attribute.ConditionNames.Length];
            for (var i = 0; i < attribute.ConditionNames.Length; i++)
            {
                var conditionName = attribute.ConditionNames[i];
                if (!property.GetBoolInfo(target, conditionName, out infos[i])) continue;
                canDraw = canDraw && infos[i].Value;
                valid = true;
            }

            m_showIfCache[mightyMember] = (valid, canDraw, infos);
        }

        public override void ClearCache() => m_showIfCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_showIfCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, _, infos) = m_showIfCache[mightyMember];
            if (!valid) return;

            var canDraw = true;
            foreach (var info in infos) canDraw = canDraw && info.RefreshValue();

            m_showIfCache[mightyMember] = (true, canDraw, infos);
        }
    }
}
#endif