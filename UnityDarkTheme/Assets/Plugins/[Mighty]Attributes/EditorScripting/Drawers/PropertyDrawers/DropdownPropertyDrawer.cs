#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(DropdownAttribute))]
    public class DropdownPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(bool, MightyInfo<object[]>, MightyInfo<object>)> m_dropdownCache =
            new MightyCache<(bool, MightyInfo<object[]>, MightyInfo<object>)>();

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            if (!m_dropdownCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (_, valuesInfo, propertyInfo) = m_dropdownCache[mightyMember];

            var value = Draw(valuesInfo, propertyInfo, property, propertyInfo.Value, (DropdownAttribute) baseAttribute);
            if (value != null) propertyInfo.Value = value;
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (!m_dropdownCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (_, valuesInfo, propertyInfo) = m_dropdownCache[mightyMember];

            if (!(propertyInfo.Value is IList array)) return;

            var value = Draw(valuesInfo, propertyInfo, mightyMember.GetElement(index), array[index], (DropdownAttribute) baseAttribute);

            if (value == null || array.Count <= index) return;

            array[index] = value;
            propertyInfo.Value = array;
        }

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (!m_dropdownCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (_, valuesInfo, propertyInfo) = m_dropdownCache[mightyMember];

            if (!(propertyInfo.Value is IList array)) return;

            var value = Draw(valuesInfo, propertyInfo, mightyMember.GetElement(index), array[index], (DropdownAttribute) baseAttribute,
                label);

            if (value == null || array.Count <= index) return;

            array[index] = value;
            propertyInfo.Value = array;
        }

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (!m_dropdownCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (_, valuesInfo, propertyInfo) = m_dropdownCache[mightyMember];

            if (!(propertyInfo.Value is IList array)) return;

            var value = Draw(rect, valuesInfo, propertyInfo, mightyMember.GetElement(index), array[index], (DropdownAttribute) baseAttribute);

            if (value == null || array.Count <= index) return;

            array[index] = value;
            propertyInfo.Value = array;
        }

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (!m_dropdownCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            return m_dropdownCache[mightyMember].Item1 ? 20 : 64;
        }


        private object Draw(MightyInfo<object[]> valuesInfo, MightyInfo<object> propertyInfo, SerializedProperty property,
            object selectedValue, DropdownAttribute dropdownAttribute, GUIContent label = null)
        {
            if (valuesInfo == null)
            {
                EditorDrawUtility.DrawPropertyField(property, label);
                EditorDrawUtility.DrawHelpBox(
                    $"{typeof(DropdownAttribute).Name} cannot find a values field with name \"{dropdownAttribute.ValuesFieldName}\"");
                return null;
            }

            if (GetDisplayOptions(valuesInfo, propertyInfo, selectedValue, out var displayOptions, out var values,
                out var selectedValueIndex))
                return DrawDropdown(property.GetTargetObject(), label ?? EditorGUIUtility.TrTextContent(property.displayName),
                    selectedValueIndex, values, displayOptions, dropdownAttribute);

            EditorDrawUtility.DrawPropertyField(property, label);
            EditorDrawUtility.DrawHelpBox(
                $"{typeof(DropdownAttribute).Name} works only when the type of the field is equal to the element type of the array");

            return null;
        }

        private object Draw(Rect rect, MightyInfo<object[]> valuesInfo, MightyInfo<object> propertyInfo, SerializedProperty property,
            object selectedValue, DropdownAttribute dropdownAttribute)
        {
            if (valuesInfo == null)
            {
                EditorDrawUtility.DrawPropertyField(rect, property);
                rect.y += 18;
                rect.height -= 24;
                EditorDrawUtility.DrawHelpBox(rect,
                    $"{typeof(DropdownAttribute).Name} cannot find a values field with name \"{dropdownAttribute.ValuesFieldName}\"");
                return null;
            }

            if (GetDisplayOptions(valuesInfo, propertyInfo, selectedValue, out var displayOptions, out var values,
                out var selectedValueIndex))
                return DrawDropdown(rect, property.GetTargetObject(), property.displayName, selectedValueIndex, values, displayOptions,
                    dropdownAttribute);

            EditorDrawUtility.DrawPropertyField(rect, property);
            rect.y += 18;
            rect.height -= 24;
            EditorDrawUtility.DrawHelpBox(rect,
                $"{typeof(DropdownAttribute).Name} works only when the type of the field is equal to the element type of the array");

            return null;
        }

        private static bool GetDisplayOptions(MightyInfo<object[]> valuesInfo, MightyInfo<object> propertyInfo,
            object selectedValue, out string[] displayOptions, out object[] values, out int selectedValueIndex)
        {
            selectedValueIndex = -1;
            values = null;
            displayOptions = null;

            if (valuesInfo.Value == null ||
                (propertyInfo.MemberType != valuesInfo.ElementType && propertyInfo.ElementType != valuesInfo.ElementType)) return false;

            displayOptions = GetDisplayOptions(valuesInfo.Value, selectedValue, out values, out selectedValueIndex);
            return true;
        }

        private static string[] GetDisplayOptions(IList valuesList, object selectedValue, out object[] values, out int selectedValueIndex)
        {
            values = new object[valuesList.Count];
            var displayOptions = new string[valuesList.Count];

            for (var i = 0; i < values.Length; i++)
            {
                var value = valuesList[i];
                values[i] = value;
                displayOptions[i] = value.ToString();
            }

            // Selected value index
            selectedValueIndex = Array.IndexOf(values, selectedValue);
            if (selectedValueIndex < 0) selectedValueIndex = 0;

            return displayOptions;
        }

        private object DrawDropdown(Object target, GUIContent label, int selectedValueIndex, object[] values,
            string[] displayOptions, DropdownAttribute attribute)
        {
            EditorGUI.BeginChangeCheck();

            int index;
            if (attribute.Option.Contains(FieldOption.HideLabel))
                index = EditorGUILayout.Popup(selectedValueIndex, displayOptions);
            else if (attribute.Option.Contains(FieldOption.BoldLabel))
                index = EditorGUILayout.Popup(label.text, selectedValueIndex, displayOptions, EditorStyles.boldLabel);
            else
                index = EditorGUILayout.Popup(label, selectedValueIndex, displayOptions);

            if (!EditorGUI.EndChangeCheck()) return null;

            Undo.RecordObject(target, "Dropdown");
            return values[index];
        }

        private object DrawDropdown(Rect rect, Object target, string label, int selectedValueIndex, object[] values,
            string[] displayOptions, DropdownAttribute attribute)
        {
            EditorGUI.BeginChangeCheck();

            int index;
            if (attribute.Option.Contains(FieldOption.HideLabel))
                index = EditorGUI.Popup(rect, selectedValueIndex, displayOptions);
            else if (attribute.Option.Contains(FieldOption.BoldLabel))
                index = EditorGUI.Popup(rect, label, selectedValueIndex, displayOptions, EditorStyles.boldLabel);
            else
                index = EditorGUI.Popup(rect, label, selectedValueIndex, displayOptions);

            if (!EditorGUI.EndChangeCheck()) return null;

            Undo.RecordObject(target, "Dropdown");
            return values[index];
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var property = mightyMember.Property;
            var attribute = (DropdownAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<DropdownAttribute>();

            var valid = property.GetArrayInfoFromMember(target, attribute.ValuesFieldName, out var valuesInfo);

            MightyInfo<object> propertyInfo = null;
            valid = valid && property.GetInfoFromMember(target, property.name, out propertyInfo);

            m_dropdownCache[mightyMember] = (valid, valuesInfo, propertyInfo);
        }

        public override void ClearCache() => m_dropdownCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_dropdownCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, valuesInfo, propertyInfo) = m_dropdownCache[mightyMember];
            if (!valid) return;

            valuesInfo.RefreshValue();
            propertyInfo.RefreshValue();
        }
    }
}
#endif