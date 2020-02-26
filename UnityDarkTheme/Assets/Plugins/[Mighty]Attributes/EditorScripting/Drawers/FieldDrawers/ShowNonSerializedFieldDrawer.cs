#if UNITY_EDITOR
using System.Reflection;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ShowNonSerializedAttribute))]
    public class ShowNonSerializedFieldDrawer : BaseFieldDrawer
    {
        public override void DrawField(MightyMember<FieldInfo> mightyMember, BaseDrawerAttribute baseAttribute)
        {
            var attribute = (ShowNonSerializedAttribute) baseAttribute;
            var field = mightyMember.MemberInfo;
            var value = field.GetValue(mightyMember.Target);

            if (EditorDrawUtility.DrawLayoutField(attribute.DrawPrettyName ? field.Name.DrawPrettyName() : field.Name, value,
                attribute.Enabled)) return;

            EditorDrawUtility.DrawHelpBox($"{typeof(ShowNonSerializedAttribute).Name} doesn't support {field.FieldType.Name} types");
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif