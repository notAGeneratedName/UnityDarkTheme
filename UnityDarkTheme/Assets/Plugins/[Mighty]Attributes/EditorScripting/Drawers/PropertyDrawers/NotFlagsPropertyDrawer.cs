#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(NotFlagsAttribute))]
    public class NotFlagsPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer
    {
        private readonly MightyCache<(bool, Type, string[])> m_notFlagsCache = new MightyCache<(bool, Type, string[])>();

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawNotFlags(mightyMember, property, (NotFlagsAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawNotFlags(mightyMember,mightyMember.GetElement(index), (NotFlagsAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawNotFlags(mightyMember,mightyMember.GetElement(index), (NotFlagsAttribute) baseAttribute, label);

        public void DrawElement(Rect position, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawNotFlags(position, mightyMember, mightyMember.GetElement(index), (NotFlagsAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (!m_notFlagsCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            return m_notFlagsCache[mightyMember].Item1 ? 20 : 64;
        }

        public void DrawNotFlags(BaseMightyMember mightyMember, SerializedProperty property, NotFlagsAttribute attribute, GUIContent label = null)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" type should be an enum");
                return;
            }

            if (!m_notFlagsCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, type, names) = m_notFlagsCache[mightyMember];
            
            if (attribute.AllowNothing)
            {
                if (property.intValue == -1) property.intValue = 0;

                try
                {
                    var enumObject = Enum.ToObject(type, property.intValue);

                    var index = attribute.Option.Contains(FieldOption.HideLabel)
                        ? EditorGUILayout.Popup(Convert.ToUInt32(enumObject).GetBitIndex(true), names)
                        : EditorGUILayout.Popup(label ?? EditorGUIUtility.TrTextContent(property.displayName),
                            Convert.ToUInt32(enumObject).GetBitIndex(true), names);

                    property.intValue = index == 0 ? 0 : (index - 1).ToBitMask();
                }
                catch
                {
                    property.intValue = 0;
                }
            }
            else
            {
                if (property.intValue == -1 || property.intValue == 0) property.intValue = 1;

                var enumObject = (Enum) Enum.ToObject(type, property.intValue);

                var enumValue = attribute.Option.Contains(FieldOption.HideLabel)
                    ? EditorGUILayout.EnumPopup(enumObject)
                    : EditorGUILayout.EnumPopup(label ?? EditorGUIUtility.TrTextContent(property.displayName), enumObject);

                property.intValue = Convert.ToInt32(enumValue);
            }

            if (!valid)
                EditorDrawUtility.DrawHelpBox($"Enum \"{property.displayName}\" is not marked by [Flags] attribute");
        }

        public void DrawNotFlags(Rect position, BaseMightyMember mightyMember, SerializedProperty property, NotFlagsAttribute attribute)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorDrawUtility.DrawPropertyField(position, property);
                EditorDrawUtility.DrawHelpBox(position, $"\"{property.displayName}\" type should be an enum");
                return;
            }

            if (!m_notFlagsCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, type, names) = m_notFlagsCache[mightyMember];
            
            if (attribute.AllowNothing)
            {
                if (property.intValue == -1) property.intValue = 0;

                try
                {
                    var enumObject = Enum.ToObject(type, property.intValue);

                    var index = attribute.Option.Contains(FieldOption.HideLabel)
                        ? EditorGUI.Popup(position, Convert.ToUInt32(enumObject).GetBitIndex(true), names)
                        : EditorGUI.Popup(position, property.displayName, Convert.ToUInt32(enumObject).GetBitIndex(true), names);

                    property.intValue = index == 0 ? 0 : (index - 1).ToBitMask();
                }
                catch
                {
                    property.intValue = 0;
                }
            }
            else
            {
                if (property.intValue == -1 || property.intValue == 0) property.intValue = 1;

                var enumObject = (Enum) Enum.ToObject(type, property.intValue);

                var enumValue = attribute.Option.Contains(FieldOption.HideLabel)
                    ? EditorGUI.EnumPopup(position, enumObject)
                    : EditorGUI.EnumPopup(position, property.displayName, enumObject);

                property.intValue = Convert.ToInt32(enumValue);
            }

            if (!valid)
                EditorDrawUtility.DrawHelpBox(position, $"Enum \"{property.displayName}\" is not marked by [Flags] attribute");
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var type = mightyMember.PropertyType;
            
            List<string> optionsList = null;
            if (((NotFlagsAttribute) mightyAttribute).AllowNothing)
            {
                optionsList = new List<string>(Enum.GetNames(type));
                optionsList.Insert(0, "Nothing");
            }

            var valid = mightyMember.Property.propertyType == SerializedPropertyType.Enum &&
                        type.GetCustomAttributes(typeof(FlagsAttribute), true).Length != 0;

            m_notFlagsCache[mightyMember] = (valid, type, optionsList?.ToArray());
        }

        public override void ClearCache() => m_notFlagsCache.ClearCache();
    }
}
#endif