#if UNITY_EDITOR
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(AvailableMaskAttribute))]
    public class AvailableMaskPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(bool, MightyInfo<int>, int[], string[])> m_availableMaskCache =
            new MightyCache<(bool, MightyInfo<int>, int[], string[])>();

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawAvailableMask(mightyMember, property, (AvailableMaskAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAvailableMask(mightyMember, mightyMember.GetElement(index), (AvailableMaskAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAvailableMask(mightyMember, mightyMember.GetElement(index), (AvailableMaskAttribute) baseAttribute,
                label);

        public void DrawElement(Rect position, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAvailableMask(position, mightyMember, mightyMember.GetElement(index),
                (AvailableMaskAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (!m_availableMaskCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            return m_availableMaskCache[mightyMember].Item1 ? 20 : 64;
        }

        public void DrawAvailableMask(BaseMightyMember mightyMember, SerializedProperty property, AvailableMaskAttribute attribute,
            GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" type should be an enum");
                return;
            }

            if (!m_availableMaskCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, _, values, names) = m_availableMaskCache[mightyMember];

            if (values == null || values.Length == 0)
            {
                property.intValue = 0;
                return;
            }

            var selectedIndexes = GetSelectedIndexes(property, values, attribute.AllowEverything);

            EditorGUI.BeginChangeCheck();

            selectedIndexes = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUILayout.MaskField(selectedIndexes, names)
                : EditorGUILayout.MaskField(label ?? EditorGUIUtility.TrTextContent(property.displayName), selectedIndexes, names);

            if (EditorGUI.EndChangeCheck() && !EditorApplication.isPlaying)
                property.intValue = GetSelectedMask(selectedIndexes, values);

            if (!valid)
                EditorDrawUtility.DrawHelpBox($"Enum \"{property.displayName}\" is not marked by [Flags] attribute");
        }

        public void DrawAvailableMask(Rect position, BaseMightyMember mightyMember, SerializedProperty property,
            AvailableMaskAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorDrawUtility.DrawPropertyField(position, property);
                EditorDrawUtility.DrawHelpBox(position, $"\"{property.displayName}\" type should be an enum");
                return;
            }

            if (!m_availableMaskCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, _, values, names) = m_availableMaskCache[mightyMember];

            if (values == null || values.Length == 0)
            {
                property.intValue = 0;
                return;
            }

            var selectedIndexes = GetSelectedIndexes(property, values, attribute.AllowEverything);

            EditorGUI.BeginChangeCheck();

            selectedIndexes = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUI.MaskField(position, selectedIndexes, names)
                : EditorGUI.MaskField(position, property.displayName, selectedIndexes, names);

            if (EditorGUI.EndChangeCheck() && !EditorApplication.isPlaying)
                property.intValue = GetSelectedMask(selectedIndexes, values);

            if (!valid)
                EditorDrawUtility.DrawHelpBox(position, $"Enum \"{property.displayName}\" is not marked by [Flags] attribute");
        }

        public static (int[], string[]) GetValuesAndNames(Type enumType, int availableMask)
        {
            if (availableMask == 0) return (null, null);

            var allValues = Enum.GetValues(enumType);

            var nameList = new List<string>();
            var valueList = new List<int>();

            for (var i = 0; i < allValues.Length; i++)
            {
                var value = allValues.GetValue(i);
                var intValue = Convert.ToInt32(value);
                if (!availableMask.MaskContains(intValue)) continue;

                nameList.Add(value.ToString());
                valueList.Add(intValue);
            }

            return (valueList.ToArray(), nameList.ToArray());
        }

        public static int GetSelectedIndexes(SerializedProperty property, int[] values, bool allowEverything)
        {
            var everything = allowEverything;
            var selectedIndexes = 0;
            var selectedValues = property.intValue;

            for (var i = 0; i < values.Length; i++)
                if (selectedValues.MaskContains(values[i]))
                    selectedIndexes |= i.ToBitMask();
                else
                    everything = false;

            if (everything) selectedIndexes = -1;

            return selectedIndexes;
        }

        public static int GetSelectedMask(int selectedIndexes, int[] values)
        {
            var selectedValues = 0;

            for (var i = 0; i < values.Length; i++)
                if ((selectedIndexes & i.ToBitMask()) != 0)
                    selectedValues = selectedValues.AddFlag(values[i]);

            return selectedValues;
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var attribute = (AvailableMaskAttribute) mightyAttribute;
            var target = mightyMember.InitAttributeTarget<AvailableMaskAttribute>();
            var property = mightyMember.Property;

            var availableMask = attribute.AvailableMask;
            if (property.GetInfoFromMember<int>(target, attribute.AvailableMaskName, out var availableMaskInfo))
                availableMask = availableMaskInfo.Value;
            else 
                availableMaskInfo = new MightyInfo<int>(availableMask);

            var enumType = mightyMember.PropertyType;
            var valid = property.propertyType == SerializedPropertyType.Enum &&
                        enumType.GetCustomAttribute(typeof(FlagsAttribute), true) != null;

            var (values, names) = GetValuesAndNames(enumType, availableMask);

            m_availableMaskCache[mightyMember] = (valid, availableMaskInfo, values, names);
        }

        public override void ClearCache() => m_availableMaskCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_availableMaskCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, availableMaskInfo, _, _) = m_availableMaskCache[mightyMember];
            
            var (values, names) = GetValuesAndNames(mightyMember.PropertyType, availableMaskInfo.RefreshValue());

            m_availableMaskCache[mightyMember] = (valid, availableMaskInfo, values, names);
        }
    }
}
#endif