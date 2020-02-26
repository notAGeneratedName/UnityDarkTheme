#if UNITY_EDITOR
using UnityEngine;
using System;
using UnityEditor;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(NestAttribute))]
    public class NestPropertyDrawer : BasePropertyDrawer, IArrayElementDrawer, IRefreshDrawer
    {
        private readonly MightyCache<(object[], MightyDrawer[], int, MightyInfo<NestOption>)> m_nestCache =
            new MightyCache<(object[], MightyDrawer[], int, MightyInfo<NestOption>)>();

        public MightyDrawer EnableProperty(SerializedProperty property, object classReference)
        {
            if (classReference.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length == 0)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" type should be a Serializable class");
                return null;
            }

            var drawer = new MightyDrawer();
            drawer.OnEnableSerializableClass(property.GetTargetObject(), classReference, property);
            return drawer;
        }

        public void DrawProperty(SerializedProperty property, NestAttribute attribute, MightyDrawer drawer)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawPropertyField(property);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" can't be an array");
                return;
            }

            DrawNest(property, attribute, drawer);
        }

        private void DrawNest(SerializedProperty property, NestAttribute attribute, MightyDrawer drawer)
        {
            var context = property.GetTargetObject();

            var option = attribute.NestOption;
            attribute.Option = (FieldOption) option;

            if (option == NestOption.ContentOnly)
                drawer.OnGUI(context, property.serializedObject);
            else
            {
                if (!option.Contains(NestOption.DontFold))
                {
                    if (!EditorDrawUtility.DrawFoldout(property,
                        option.Contains(NestOption.BoldLabel) ? GUIStyleUtility.BoldFoldout : EditorStyles.foldout))
                        return;
                }
                else
                    DrawLabel(property, attribute, EditorGUIUtility.TrTextContent(property.displayName));

                if (!option.Contains(NestOption.DontIndent)) EditorGUI.indentLevel++;
                drawer.OnGUI(context, property.serializedObject);
                if (!option.Contains(NestOption.DontIndent)) EditorGUI.indentLevel--;
            }
        }

        public override void DrawProperty(BaseMightyMember mightyMember, SerializedProperty property, BaseDrawerAttribute baseAttribute)
        {
            if (property.isArray)
            {
                EditorDrawUtility.DrawArray(property, index => DrawElement(mightyMember, index, baseAttribute));
                return;
            }

            DrawNest(mightyMember, property, 0, (NestAttribute) baseAttribute);
        }

        public void DrawElement(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawNest(mightyMember, mightyMember.GetElement(index), index, (NestAttribute) baseAttribute);

        public void DrawElement(GUIContent label, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawNest(mightyMember, mightyMember.GetElement(index), index, (NestAttribute) baseAttribute,
                label);

        public void DrawElement(Rect rect, BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) =>
            DrawNest(mightyMember, mightyMember.GetElement(index), index, (NestAttribute) baseAttribute);

        public float GetElementHeight(BaseMightyMember mightyMember, int index, BaseDrawerAttribute baseAttribute) => 0;

        private void DrawNest(BaseMightyMember mightyMember, SerializedProperty property, int index,
            NestAttribute attribute, GUIContent label = null)
        {
            var context = mightyMember.Context;
            if (mightyMember.PropertyType.GetCustomAttributes(typeof(SerializableAttribute), true).Length == 0)
            {
                EditorDrawUtility.DrawPropertyField(property, label);
                EditorDrawUtility.DrawHelpBox($"\"{property.displayName}\" type should be a Serializable class");
                return;
            }

            if (!m_nestCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            var (_, drawers, _, optionInfo) = m_nestCache[mightyMember];

            if (index >= drawers.Length) return;

            var option = optionInfo.Value;

            attribute.Option = (FieldOption) option;

            if (option == NestOption.ContentOnly)
                drawers[index].OnGUI(context, property.serializedObject);
            else
            {
                if (!option.Contains(NestOption.DontFold))
                {
                    if (!EditorDrawUtility.DrawFoldout(property, label ?? EditorGUIUtility.TrTextContent(property.displayName),
                        option.Contains(NestOption.BoldLabel) ? GUIStyleUtility.BoldFoldout : EditorStyles.foldout))
                        return;
                }
                else
                    DrawLabel(property, attribute, label);

                if (!option.Contains(NestOption.DontIndent)) EditorGUI.indentLevel++;
                drawers[index].OnGUI(context, property.serializedObject);
                if (!option.Contains(NestOption.DontIndent)) EditorGUI.indentLevel--;
            }
        }

        public void ApplyAutoValues(BaseMightyMember mightyMember, NestAttribute attribute, bool refreshDrawers)
        {
            if (!m_nestCache.Contains(mightyMember)) InitDrawer(mightyMember, attribute);
            foreach (var drawer in m_nestCache[mightyMember].Item2) drawer.ApplyAutoValues(refreshDrawers);
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var property = mightyMember.Property;
            var target = mightyMember.InitAttributeTarget<NestAttribute>();
            var context = mightyMember.Context;

            var isArray = property.isArray;
            var size = isArray ? property.arraySize : 1;
            var classReferences = new object[size];
            var drawers = new MightyDrawer[size];
            MightyInfo<NestOption> optionInfo = null;

            if (m_nestCache.TryGetValue(mightyMember, out var cache))
            {
                var (cacheClasses, cacheDrawers, cacheSize, cacheInfo) = cache;
                optionInfo = cacheInfo;
                for (var i = 0; i < size; i++)
                {
                    if (cacheSize < size) continue;
                    classReferences[i] = cacheClasses[i];
                    drawers[i] = cacheDrawers[i];
                }
            }

            if (isArray)
                for (var i = 0; i < size; i++)
                {
                    var element = property.GetArrayElementAtIndex(i);
                    if (classReferences[i] == null) classReferences[i] = element.GetSerializableClassReference();
                    if (drawers[i] == null)
                        (drawers[i] = new MightyDrawer()).OnEnableSerializableClass(context, classReferences[i], element);
                }
            else
            {
                if (classReferences[0] == null) classReferences[0] = property.GetSerializableClassReference();
                if (drawers[0] == null)
                    (drawers[0] = new MightyDrawer()).OnEnableSerializableClass(context, classReferences[0], property);
            }


            if (optionInfo == null)
            {
                var attribute = (NestAttribute) mightyAttribute;
                if (!property.GetInfoFromMember(target, attribute.OptionCallback, out optionInfo))
                    optionInfo = new MightyInfo<NestOption>(attribute.NestOption);
            }

            m_nestCache[mightyMember] = (classReferences, drawers, size, optionInfo);
        }

        public override void ClearCache()
        {
            foreach (var (_, drawers, _, _) in m_nestCache.Values)
            foreach (var drawer in drawers)
                drawer.OnDisable();

            m_nestCache.ClearCache();
        }

        public void RefreshDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_nestCache.Contains(mightyMember))
            {
                InitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (_, _, arraySize, optionInfo) = m_nestCache[mightyMember];

            if (mightyMember.Property.isArray && mightyMember.Property.arraySize != arraySize)
                InitDrawer(mightyMember, mightyAttribute);
            else
                optionInfo.RefreshValue();
        }
    }
}
#endif