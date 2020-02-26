#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(EnableIfAttribute))]
    public class EnableIfDecoratorDrawer : BaseDecoratorDrawer, IRefreshDrawer
    {
        private bool _previouslyEnabled;

        private readonly MightyCache<(bool, bool, MightyInfo<bool>[])> m_enableIfCache =
            new MightyCache<(bool, bool, MightyInfo<bool>[])>();

        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute)
        {
            if (!m_enableIfCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (valid, enabled, _) = m_enableIfCache[mightyMember];

            _previouslyEnabled = GUI.enabled;

            if (valid)
                GUI.enabled = enabled;
            else
                EditorDrawUtility.DrawHelpBox($"{typeof(EnableIfAttribute).Name} needs a valid boolean condition to work");
        }

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) =>
            GUI.enabled = _previouslyEnabled;

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var target = mightyMember.InitAttributeTarget<EnableIfAttribute>();
            var attribute = (EnableIfAttribute) mightyAttribute;
            var property = mightyMember.Property;

            var enabled = true;
            var valid = false;
            var infos = new MightyInfo<bool>[attribute.ConditionNames.Length];
            for (var i = 0; i < attribute.ConditionNames.Length; i++)
            {
                var conditionName = attribute.ConditionNames[i];
                if (!property.GetBoolInfo(target, conditionName, out infos[i])) continue;
                enabled = enabled && infos[i].Value;
                valid = true;
            }

            m_enableIfCache[mightyMember] = (valid, enabled, infos);
        }

        public override void ClearCache() => m_enableIfCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_enableIfCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, _, infos) = m_enableIfCache[mightyMember];
            if (!valid) return;

            var enabled = true;
            foreach (var info in infos) enabled = enabled && info.RefreshValue();

            m_enableIfCache[mightyMember] = (true, enabled, infos);
        }
    }
}
#endif