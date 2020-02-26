#if UNITY_EDITOR
using System;

namespace MightyAttributes.Editor
{
    public class MightyWrapperDrawer : BaseMightyDrawer
    {
        public T[] GetAttributes<T>(Type type) where T : Attribute => GetAttributes(type, typeof(T)) as T[];

        public object[] GetAttributes(Type type, Type attributeType) => type.GetCustomAttributes(attributeType, true);
        
        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif