#if UNITY_EDITOR
using System.Linq;

namespace MightyAttributes.Editor
{
    public abstract class BaseArrayDrawer : BaseMightyDrawer, IRefreshDrawer
    {
        protected readonly MightyCache<(MightyInfo<ArrayOption>, BaseElementDecoratorAttribute[], BaseElementDecoratorDrawer[])>
            ArrayCache = new MightyCache<(MightyInfo<ArrayOption>, BaseElementDecoratorAttribute[], BaseElementDecoratorDrawer[])>();

        public abstract void DrawArray(BaseMightyMember mightyMember, BaseArrayAttribute baseAttribute, IArrayElementDrawer drawer,
            BaseDrawerAttribute drawerAttribute);

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (BaseArrayAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<BaseArrayAttribute>();
            var property = mightyMember.Property;

            var decoratorAttributes = property.GetAttributes<BaseElementDecoratorAttribute>();
            var decoratorDrawers = decoratorAttributes.Select(DrawersDatabase.GetDrawerForAttribute<BaseElementDecoratorDrawer>).ToArray();

            if (!property.GetInfoFromMember<ArrayOption>(target, attribute.OptionName, out var optionInfo))
                optionInfo = new MightyInfo<ArrayOption>(attribute.Option);

            ArrayCache[mightyMember] = (optionInfo, decoratorAttributes, decoratorDrawers);
        }

        public override void ClearCache() => ArrayCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!ArrayCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            ArrayCache[mightyMember].Item1.RefreshValue();
            
            RefreshDrawerImpl(mightyMember, mightyAttribute);
        }

        protected virtual void RefreshDrawerImpl(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            
        }
    }
}
#endif