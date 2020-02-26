#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(BoldLabelAttribute))]
    public class BoldLabelDecoratorDrawer : BaseDecoratorDrawer
    {
        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) => 
            EditorDrawUtility.SetBoldDefaultFont(true);

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) => 
            EditorDrawUtility.SetBoldDefaultFont(false);

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif