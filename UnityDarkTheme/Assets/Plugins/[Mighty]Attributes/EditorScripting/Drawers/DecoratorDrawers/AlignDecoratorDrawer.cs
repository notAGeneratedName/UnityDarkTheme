#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(AlignAttribute))]
    public class AlignDecoratorDrawer : BaseDecoratorDrawer
    {
        public override void BeginDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) =>
            EditorDrawUtility.BeginDrawAlign(((AlignAttribute) baseAttribute).Align);

        public override void EndDraw(BaseMightyMember mightyMember, BaseDecoratorAttribute baseAttribute) =>
            EditorDrawUtility.EndDrawAlign(((AlignAttribute) baseAttribute).Align);

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif