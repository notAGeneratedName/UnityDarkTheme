#if UNITY_EDITOR
using System.Reflection;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(AvailableEnumAttribute))]
    public class AvailableEnumPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(bool, MightyInfo<int[]>, string[])> m_availableEnumCache =
            new MightyCache<(bool, MightyInfo<int[]>, string[])>();


        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawAvailableEnum(mightyMember, property, (AvailableEnumAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAvailableEnum(mightyMember, mightyMember.GetElement(index), (AvailableEnumAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAvailableEnum(mightyMember, mightyMember.GetElement(index), (AvailableEnumAttribute) baseAttribute,
                label);

        public void DrawElement(Rect position, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawAvailableEnum(position, mightyMember, mightyMember.GetElement(index),
                (AvailableEnumAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            mightyMember.GetElement(index).propertyType != SerializedPropertyType.Enum ? 64 : 20;

        public void DrawAvailableEnum(BaseMightyMember mightyMember, SerializedProperty property, AvailableEnumAttribute attribute,
            GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" type should be an enum");
                return;
            }

            if (!m_availableEnumCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, valuesInfo, names) = m_availableEnumCache[mightyMember];

            if (!valid) return;

            if (valuesInfo.Value.Length == 0)
            {
                property.intValue = 0;
                return;
            }

            var selectedIndex = GetSelectedIndex(property, valuesInfo.Value);

            EditorGUI.BeginChangeCheck();

            selectedIndex = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUILayout.Popup(selectedIndex, names)
                : EditorGUILayout.Popup(label ?? EditorGUIUtility.TrTextContent(property.displayName), selectedIndex, names);

            if (EditorGUI.EndChangeCheck() && !EditorApplication.isPlaying)
                property.intValue = GetSelectedValue(selectedIndex, valuesInfo.Value);
        }

        public void DrawAvailableEnum(Rect position, BaseMightyMember mightyMember, SerializedProperty property,
            AvailableEnumAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorDrawUtility.DrawPropertyField(position, property);
                EditorDrawUtility.DrawHelpBox(position, $"\"{property.displayName}\" type should be an enum");
                return;
            }

            if (!m_availableEnumCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, valuesInfo, names) = m_availableEnumCache[mightyMember];

            if (!valid) return;

            if (valuesInfo.Value.Length == 0)
            {
                property.intValue = 0;
                return;
            }

            var selectedIndex = GetSelectedIndex(property, valuesInfo.Value);

            EditorGUI.BeginChangeCheck();

            selectedIndex = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUI.Popup(position, selectedIndex, names)
                : EditorGUI.Popup(position, property.displayName, selectedIndex, names);

            if (EditorGUI.EndChangeCheck() && !EditorApplication.isPlaying)
                property.intValue = GetSelectedValue(selectedIndex, valuesInfo.Value);
        }

        public static string[] GetNames(Array allValues, int[] availableValues, bool allowNothing)
        {
            if (availableValues == null || availableValues.Length == 0) return null;

            var names = new string[availableValues.Length];

            for (int i = 0, j = allowNothing ? 1 : 0; i < allValues.Length; i++)
            {
                var value = allValues.GetValue(i);
                if (!availableValues.Contains(Convert.ToInt32(value))) continue;
                names[j++] = value.ToString();
            }

            if (allowNothing) names[0] = "Nothing";

            return names;
        }

        public static int GetSelectedIndex(SerializedProperty property, int[] availableValues)
        {
            var selectedIndex = 0;
            var selectedValue = property.intValue;

            for (var i = 0; i < availableValues.Length; i++)
            {
                if (selectedValue != availableValues[i]) continue;
                selectedIndex = i;
                break;
            }

            return selectedIndex;
        }

        public static int GetSelectedValue(int selectedIndexes, int[] values) => values[selectedIndexes];

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var property = mightyMember.Property;
            var attribute = (AvailableEnumAttribute) mightyAttribute;
            var type = mightyMember.PropertyType;
            var target = mightyMember.InitAttributeTarget<AvailableEnumAttribute>();

            if (!property.GetArrayInfoFromMember(target, attribute.AvailableValuesName, out var mightyInfo))
            {
                m_availableEnumCache[mightyMember] = (false, null, null);
                return;
            }

            var availableValues = mightyInfo.Value.Select(Convert.ToInt32).ToArray();

            var allowNothing = attribute.AllowNothing && type.GetCustomAttribute(typeof(FlagsAttribute), true) != null;
            if (allowNothing) availableValues = availableValues.AddArrayElement(0, 0);

            var valuesInfo = new MightyInfo<int[]>(mightyInfo, availableValues);
            var names = GetNames(Enum.GetValues(type), availableValues, allowNothing);

            m_availableEnumCache[mightyMember] = (true, valuesInfo, names);
        }

        public override void ClearCache() => m_availableEnumCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_availableEnumCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }
            
            var (valid, valuesInfo, _) = m_availableEnumCache[mightyMember];
            if (!valid) return;
            
            var type = mightyMember.PropertyType;
            
            var values = valuesInfo.RefreshValue().Select(Convert.ToInt32).ToArray();

            var allowNothing = ((AvailableEnumAttribute) mightyAttribute).AllowNothing &&
                               type.GetCustomAttribute(typeof(FlagsAttribute), true) != null;

            if (allowNothing) values = values.AddArrayElement(0, 0);

            valuesInfo.Value = values;
            var names = GetNames(Enum.GetValues(type), values, allowNothing);

            m_availableEnumCache[mightyMember] = (true, valuesInfo, names);
        }
    }
}
#endif