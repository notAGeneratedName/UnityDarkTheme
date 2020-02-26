#if UNITY_EDITOR
namespace MightyAttributes.Editor
{
    public interface IDrawAnywhereDecorator
    {
        void BeginDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute);
        void EndDrawAnywhere(BaseMightyMember mightyMember, BaseGlobalDecoratorAttribute attribute);
    }
}
#endif