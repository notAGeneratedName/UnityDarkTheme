#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(MaskAttribute))]
    public class MaskPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(bool, MightyInfo<string[]>)> m_maskCache = new MightyCache<(bool, MightyInfo<string[]>)>();

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawMask(mightyMember, property, (MaskAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMask(mightyMember, mightyMember.GetElement(index), (MaskAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMask(mightyMember, mightyMember.GetElement(index), (MaskAttribute) baseAttribute, label);

        public void DrawElement(Rect position, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawMask(position, mightyMember, mightyMember.GetElement(index), (MaskAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute)
        {
            if (!m_maskCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            return m_maskCache[mightyMember].Item1 ? 20 : 64;
        }

        public void DrawMask(BaseMightyMember mightyMember, SerializedProperty property, MaskAttribute attribute, GUIContent label = null)
        {
            if (!m_maskCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, maskNames) = m_maskCache[mightyMember];

            if (!valid)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" type should be an integer");
                return;
            }

            if (maskNames.Value.Length == 0)
            {
                property.intValue = 0;
                return;
            }

            property.intValue = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUILayout.MaskField(property.intValue, maskNames.Value)
                : EditorGUILayout.MaskField(label ?? EditorGUIUtility.TrTextContent(property.displayName), property.intValue,
                    maskNames.Value);
        }

        public void DrawMask(Rect position, BaseMightyMember mightyMember, SerializedProperty property, MaskAttribute attribute)
        {
            if (!m_maskCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (valid, maskNames) = m_maskCache[mightyMember];

            if (!valid)
            {
                EditorDrawUtility.DrawPropertyField(position, property);
                EditorDrawUtility.DrawHelpBox(position, $"\"{property.displayName}\" type should be an integer");
                return;
            }

            if (maskNames.Value.Length == 0)
            {
                property.intValue = 0;
                return;
            }

            property.intValue = attribute.Option.Contains(FieldOption.HideLabel)
                ? EditorGUI.MaskField(position, property.intValue, maskNames.Value)
                : EditorGUI.MaskField(position, property.displayName, property.intValue, maskNames.Value);
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var property = mightyMember.Property;
            var target = mightyMember.InitAttributeTarget<MaskAttribute>();
            var attribute = (MaskAttribute) mightyAttribute;

            var maskNames = attribute.MaskNames;
            var maskNamesInfo = new MightyInfo<string[]>(null, null, maskNames);
            var valid = property.propertyType == SerializedPropertyType.Integer;

            MightyInfo<object[]> valuesFromInfo = null;

            valid = valid && property.GetArrayInfoFromMember(target, attribute.MaskNamesCallback, out valuesFromInfo);

            if (valid) maskNamesInfo = new MightyInfo<string[]>(valuesFromInfo, maskNamesInfo.Value.Select(n => n.ToString()).ToArray());

            m_maskCache[mightyMember] = (valid, maskNamesInfo);
        }

        public override void ClearCache() => m_maskCache.ClearCache();

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_maskCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, maskNamesInfo) = m_maskCache[mightyMember];
            
            if (valid) maskNamesInfo.RefreshValue();
        }
    }
}
#endif