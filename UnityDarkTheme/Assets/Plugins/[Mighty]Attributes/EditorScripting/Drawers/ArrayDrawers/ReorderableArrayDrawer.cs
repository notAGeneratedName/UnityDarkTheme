#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ReorderableListAttribute), typeof(ReorderableAttribute))]
    public class ReorderableArrayDrawer : BaseArrayDrawer
    {
        private readonly MightyCache<int> m_indentCache = new MightyCache<int>();
        private readonly MightyCache<ReorderableList> m_reorderableCache = new MightyCache<ReorderableList>();

        public override void DrawArray(BaseMightyMember mightyMember, BaseArrayAttribute baseAttribute, IArrayElementDrawer drawer,
            BaseDrawerAttribute drawerAttribute)
        {
            var property = mightyMember.Property;

            if (property.isArray)
            {
                var attribute = (ReorderableListAttribute) baseAttribute;

                if (!ArrayCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
                var (optionInfo, decoratorAttributes, decoratorDrawers) = ArrayCache[mightyMember];

                var option = optionInfo.Value;

                var decoratorLength = decoratorAttributes.Length;

                EditorGUILayout.BeginVertical();

                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].BeginDrawArray(mightyMember, decoratorAttributes[i]);

                if (!option.Contains(ArrayOption.HideLabel))
                {
                    if (!option.Contains(ArrayOption.LabelInHeader))
                    {
                        for (var i = 0; i < decoratorLength; i++)
                            decoratorDrawers[i].BeginDrawHeader(mightyMember, decoratorAttributes[i]);

                        if (!EditorDrawUtility.DrawFoldout(property))
                        {
                            EditorGUILayout.EndVertical();

                            for (var i = 0; i < decoratorLength; i++)
                                decoratorDrawers[i].EndDrawHeader(mightyMember, decoratorAttributes[i]);

                            for (var i = 0; i < decoratorLength; i++)
                                decoratorDrawers[i].EndDrawArray(mightyMember, decoratorAttributes[i]);

                            return;
                        }

                        for (var i = 0; i < decoratorLength; i++)
                            decoratorDrawers[i].EndDrawHeader(mightyMember, decoratorAttributes[i]);
                    }
                }
                else property.isExpanded = true;

                if (!option.Contains(ArrayOption.DontIndent))
                {
                    m_indentCache[mightyMember] = EditorGUI.indentLevel;
                    EditorDrawUtility.BeginLayoutIndent();
                    EditorGUI.indentLevel = 0;
                    EditorGUILayout.BeginVertical();
                }

                if (!m_reorderableCache.Contains(mightyMember))
                {
                    ReorderableList reorderableList = new ReorderableList(property.serializedObject, property,
                        !option.Contains(ArrayOption.ReadOnly),
                        option.Contains(ArrayOption.LabelInHeader) || !option.Contains(ArrayOption.HideSizeField),
                        attribute.DrawButtons, attribute.DrawButtons)
                    {
                        drawHeaderCallback = rect =>
                        {
                            var drawLabel = option.Contains(ArrayOption.LabelInHeader);

                            if (!drawLabel && option.Contains(ArrayOption.HideSizeField)) return;

                            for (var i = 0; i < decoratorLength; i++)
                                rect = decoratorDrawers[i].BeginDrawHeader(rect, mightyMember, decoratorAttributes[i]);

                            if (!drawLabel)
                            {
                                var enabled = GUI.enabled;
                                GUI.enabled = !option.Contains(ArrayOption.DisableSizeField);
                                EditorDrawUtility.DrawArraySizeField(rect, property);
                                GUI.enabled = enabled;
                            }
                            else
                                EditorGUI.LabelField(rect, property.displayName);

                            var newRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, 0);
                            for (var i = 0; i < decoratorLength; i++)
                                newRect = decoratorDrawers[i].EndDrawHeader(newRect, mightyMember, decoratorAttributes[i]);
                        },

                        drawElementCallback = (rect, index, isActive, isFocused) =>
                        {
                            rect.y += 2f;

                            var newRect = new Rect(rect.x, rect.y + 21, rect.width, 0);
                            for (var i = 0; i < decoratorLength; i++)
                                newRect = decoratorDrawers[i].BeginDrawElement(newRect, mightyMember, i, decoratorAttributes[i]);

                            if (drawer != null)
                            {
                                var height = drawer.GetElementHeight(mightyMember, index, drawerAttribute);
                                drawer.DrawElement(new Rect(rect.x, rect.y, rect.width, height), mightyMember, index, drawerAttribute);
                            }
                            else
                                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                                    property.GetArrayElementAtIndex(index));

                            for (var i = 0; i < decoratorLength; i++)
                                newRect = decoratorDrawers[i].EndDrawElement(newRect, mightyMember, i, decoratorAttributes[i]);
                        },

                        elementHeightCallback = (index) =>
                        {
                            var height = drawer?.GetElementHeight(mightyMember, index, drawerAttribute) ?? 20;

                            for (var i = 0; i < decoratorLength; i++)
                                height += decoratorDrawers[i].GetElementHeight(mightyMember, i, decoratorAttributes[i]);

                            return height;
                        }
                    };

                    m_reorderableCache[mightyMember] = reorderableList;
                }

                m_reorderableCache[mightyMember].DoLayoutList();

                if (!option.Contains(ArrayOption.DontIndent))
                {
                    EditorGUI.indentLevel = m_indentCache[mightyMember];
                    EditorDrawUtility.EndLayoutIndent();
                    EditorGUILayout.EndVertical();
                }

                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].EndDrawArray(mightyMember, decoratorAttributes[i]);

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorDrawUtility.DrawHelpBox($"{baseAttribute.GetType().Name} can be used only on arrays or lists");

                EditorDrawUtility.DrawPropertyField(property);
            }
        }

        public override void ClearCache()
        {
            base.ClearCache();
            m_indentCache.ClearCache();
            m_reorderableCache.ClearCache();
        }
    }
}
#endif