#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(DisableIfAttribute))]
    public class DisableIfDecoratorDrawer : BaseDecoratorDrawer, IRefreshDrawer
    {
        private bool _previouslyEnabled;

        private readonly MightyCache<(bool, bool, MightyInfo<bool>[])> m_disableIfCache =
            new MightyCache<(bool, bool, MightyInfo<bool>[])>();

        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            if (!m_disableIfCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (valid, disabled, _) = m_disableIfCache[mightyMember];

            _previouslyEnabled = GUI.enabled;

            if (valid)
                GUI.enabled = !disabled;
            else
                EditorDrawUtility.DrawHelpBox($"{typeof(DisableIfAttribute).Name} needs a valid boolean condition to work");
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) =>
            GUI.enabled = _previouslyEnabled;

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var target = mightyMember.InitAttributeTarget<DisableIfAttribute>();
            var attribute = (DisableIfAttribute) mightyAttribute;
            var property = mightyMember.Property;

            var disabled = false;
            var valid = false;
            var infos = new MightyInfo<bool>[attribute.ConditionNames.Length];
            for (var i = 0; i < attribute.ConditionNames.Length; i++)
            {
                var conditionName = attribute.ConditionNames[i];
                if (!property.GetBoolInfo(target, conditionName, out infos[i])) continue;
                disabled = disabled || infos[i].Value;
                valid = true;
            }

            m_disableIfCache[mightyMember] = (valid, disabled, infos);
        }

        public override void ClearCache() => m_disableIfCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_disableIfCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, _, infos) = m_disableIfCache[mightyMember];
            if (!valid) return;

            var disabled = false;
            foreach (var info in infos) disabled = disabled || info.RefreshValue();

            m_disableIfCache[mightyMember] = (true, disabled, infos);
        }
    }
}
#endif