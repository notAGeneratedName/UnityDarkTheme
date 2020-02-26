#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ButtonArrayAttribute))]
    public class ButtonArrayDrawer : BaseArrayDrawer
    {
        public override void DrawArray(BaseMightyMember mightyMember, BaseArrayAttribute baseAttribute, IArrayElementDrawer drawer,
            BaseDrawerAttribute drawerAttribute)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                EditorDrawUtility.DrawHelpBox($"{typeof(ButtonArrayAttribute).Name} can be used only on arrays or lists");

                EditorDrawUtility.DrawPropertyField(property);
                return;
            }

            if (!ArrayCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (optionInfo, decoratorAttributes, decoratorDrawers) = ArrayCache[mightyMember];

            var option = optionInfo.Value;
            
            var decoratorLength = decoratorAttributes.Length;

            for (var i = 0; i < decoratorLength; i++)
                decoratorDrawers[i].BeginDrawArray(mightyMember, decoratorAttributes[i]);

            for (var i = 0; i < decoratorLength; i++)
                decoratorDrawers[i].BeginDrawHeader(mightyMember, decoratorAttributes[i]);

            if (!option.Contains(ArrayOption.HideLabel) && !EditorDrawUtility.DrawFoldout(property))
            {
                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].EndDrawHeader(mightyMember, decoratorAttributes[i]);

                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].EndDrawArray(mightyMember, decoratorAttributes[i]);

                return;
            }

            if (option.Contains(ArrayOption.HideLabel))
                property.isExpanded = true;
            else if (!option.Contains(ArrayOption.DontIndent))
                EditorGUI.indentLevel++;

            for (var i = 0; i < decoratorLength; i++)
                decoratorDrawers[i].EndDrawHeader(mightyMember, decoratorAttributes[i]);


            GUILayout.BeginVertical(GUIStyleUtility.ButtonArray, GUILayout.MinHeight(35));
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (property.arraySize == 0)
            {
                GUILayout.FlexibleSpace();
                if (EditorDrawUtility.DrawAddButton())
                {
                    property.InsertArrayElementAtIndex(0);
                    property.serializedObject.ApplyModifiedProperties();
                }

                GUILayout.FlexibleSpace();
            }

            EditorDrawUtility.DrawArrayBody(property, index =>
            {
                var element = property.GetArrayElementAtIndex(index);

                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].BeginDrawElement(mightyMember, index, decoratorAttributes[i]);

                GUILayout.BeginHorizontal(GUILayout.MinHeight(33));

                GUILayout.BeginVertical(GUILayout.Width(1));
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();

                if (EditorDrawUtility.DrawRemoveButton())
                {
                    property.DeleteArrayElementAtIndex(index);
                    property.serializedObject.ApplyModifiedProperties();

                    GUILayout.EndHorizontal();

                    for (var i = 0; i < decoratorLength; i++)
                        decoratorDrawers[i].EndDrawElement(mightyMember, index, decoratorAttributes[i]);
                    return;
                }

                if (EditorDrawUtility.DrawAddButton())
                {
                    property.InsertArrayElementAtIndex(index);
                    property.serializedObject.ApplyModifiedProperties();
                }

                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                if (drawer != null)
                    drawer.DrawElement(mightyMember, index, drawerAttribute);
                else if (option.Contains(ArrayOption.HideElementLabel))
                    EditorGUILayout.PropertyField(element, GUIContent.none);
                else
                    EditorGUILayout.PropertyField(element);

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].EndDrawElement(mightyMember, index, decoratorAttributes[i]);
            });

            EditorGUI.indentLevel = indent;
            GUILayout.EndVertical();

            if (!option.Contains(ArrayOption.HideLabel) && !option.Contains(ArrayOption.DontIndent))
                EditorGUI.indentLevel--;

            for (var i = 0; i < decoratorLength; i++)
                decoratorDrawers[i].EndDrawArray(mightyMember, decoratorAttributes[i]);
        }
    }
}
#endif