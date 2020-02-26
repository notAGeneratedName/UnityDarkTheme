#if UNITY_EDITOR
using UnityEngine;

namespace MightyAttributes.Editor
{
    public interface IArrayElementDrawer
    {
        void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute);

        void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute);

        void DrawElement(Rect position, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute);

        float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute);
    }
}
#endif