#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ValueFromAttribute))]
    public class ValueFromDrawer : BaseAutoValueDrawer
    {
        protected override InitState InitPropertyImpl(SerializedProperty property, BaseAutoValueAttribute baseAttribute)
        {
            var attribute = (ValueFromAttribute) baseAttribute;
            var attributeTarget = property.GetAttributeTarget<ValueFromAttribute>();
            var propertyTarget = property.GetPropertyTargetReference();

            if (!attributeTarget.GetType().InfoExist(attribute.ValueName))
                return new InitState(false, "Callback name: \"" + attribute.ValueName + "\" is invalid");

            if (!attributeTarget.GetType().GetMemberInfo(attribute.ValueName, new CallbackSignature(property.GetSystemType(), true), out _))
                return new InitState(false, "\"" + attribute.ValueName + "\" type is invalid");

            if (!property.isArray || property.propertyType == SerializedPropertyType.String)
                return SerializedPropertyUtility.SetGenericValue(propertyTarget, property, attribute.ValueName, property.propertyType)
                    ? new InitState(true)
                    : new InitState(false, "\"" + property.displayName + "\" type is not serializable");

            var state = new InitState(true);
            var index = 0;

            if (property.GetArrayValueFromMember(attributeTarget, attribute.ValueName, out var outArray) &&
                property.CompareArrays(outArray, propertyTarget)) return state;

            if (property.arraySize == 0)
                property.InsertArrayElementAtIndex(0);

            if (property.GetArrayElementAtIndex(0).propertyType == SerializedPropertyType.Generic)
            {
                try
                {
                    var propertyType = property.GetSystemType();
                    var propertyField = propertyTarget.GetField(property.name);
                    var array = (IList) Array.CreateInstance(propertyType, outArray.Length);

                    for (var i = 0; i < outArray.Length; i++)
                        array[i] = outArray[i];

                    propertyField.SetValue(propertyTarget, array);
                    return new InitState(true);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    property.DeleteArrayElementAtIndex(index);
                    return new InitState(false, "\"" + property.displayName + "\" type is not serializable");
                }
            }

            property.ClearArray();
            while (index < outArray.Length)
            {
                try
                {
                    property.InsertArrayElementAtIndex(index);
                    if (!(state = SerializedPropertyUtility.SetArrayElementGenericValue(attributeTarget,
                        property.GetArrayElementAtIndex(index), attribute.ValueName,
                        property.GetArrayElementAtIndex(index).propertyType, index)
                        ? new InitState(true)
                        : new InitState(false, "\"" + property.displayName + "\" type is not serializable")).isOk)
                        return state;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    property.DeleteArrayElementAtIndex(index);
                    break;
                }

                index++;
            }

            return state;
        }
    }
}
#endif