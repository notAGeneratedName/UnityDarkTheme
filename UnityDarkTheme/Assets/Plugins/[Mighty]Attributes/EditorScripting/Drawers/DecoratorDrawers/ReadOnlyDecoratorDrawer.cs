#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDecoratorDrawer : BaseDecoratorDrawer
    {
        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) => GUI.enabled = false;

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) => GUI.enabled = true;
        
        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif