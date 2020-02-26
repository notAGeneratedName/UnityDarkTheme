#if UNITY_EDITOR
using UnityEditor;

namespace MightyAttributes.Editor
{
    public struct InitState
    {
        public readonly bool isOk;
        public readonly string message;

        public InitState(bool isOk, string message = null)
        {
            this.isOk = isOk;
            this.message = message;
        }
    }

    public abstract class BaseAutoValueDrawer : BaseMightyDrawer
    {
        public InitState InitProperty(SerializedProperty property, BaseAutoValueAttribute baseAttribute) =>
            baseAttribute.ExecuteInPlayMode ? InitPropertyImpl(property, baseAttribute) :
            !EditorApplication.isPlaying ? InitPropertyImpl(property, baseAttribute) : new InitState(true);

        protected abstract InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute);

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif