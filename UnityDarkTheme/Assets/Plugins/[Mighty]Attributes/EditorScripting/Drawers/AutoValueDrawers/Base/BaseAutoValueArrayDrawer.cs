#if UNITY_EDITOR
using UnityEditor;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public abstract class BaseAutoValueArrayDrawer : BaseAutoValueDrawer
    {
        protected override InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            if (!property.isArray)
                return new InitState(false, "\"" + property.displayName + "\" should be an array");

            if (!property.GetSystemType().IsSubclassOf(typeof(Object)))
                return new InitState(false, "\"" + property.displayName + "\" array type should inherit from UnityEngine.Object");
            
            var objects = FoundArray(property, baseAttribute);
            if (property.CompareArrays(objects)) return new InitState(true);
            
            property.ClearArray();
            for (var i = 0; i < objects.Length; i++)
            {
                property.InsertArrayElementAtIndex(i);
                property.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
            }

            return new InitState(true);
        }

        protected abstract Object[] FoundArray(SerializedProperty property, BaseAutoValueAttribute baseAttribute);
    }
}
#endif