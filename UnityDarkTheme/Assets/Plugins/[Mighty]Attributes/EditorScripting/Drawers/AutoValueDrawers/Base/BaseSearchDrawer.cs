#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
	public abstract class BaseSearchDrawer : BaseAutoValueDrawer
	{
		protected override InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
		{
			if (property.isArray)
				return new InitState(false, "\"" + property.displayName + "\" should not be an array");

			if (!typeof(Object).IsAssignableFrom(property.GetSystemType()))
				return new InitState(false, "\"" + property.displayName + "\" should inherit from UnityEngine.Object");

			Find(property, baseAttribute);
			return new InitState(true);
		}

		protected abstract void Find(SerializedProperty property, BaseAutoValueAttribute baseAttribute);
	}
}
#endif