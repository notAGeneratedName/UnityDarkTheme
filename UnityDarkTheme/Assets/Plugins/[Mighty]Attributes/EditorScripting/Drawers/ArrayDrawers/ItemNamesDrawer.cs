#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(ItemNamesAttribute))]
    public class ItemNamesDrawer : BaseArrayDrawer
    {
        private readonly MightyCache<(bool, MightyInfo<object[]>)> m_itemNamesCache = new MightyCache<(bool, MightyInfo<object[]>)>();

        public override void DrawArray(BaseMightyMember mightyMember, BaseArrayAttribute baseAttribute, IArrayElementDrawer drawer,
            BaseDrawerAttribute drawerAttribute)
        {
            var property = mightyMember.Property;
            if (!property.isArray)
            {
                EditorDrawUtility.DrawHelpBox($"{typeof(ItemNamesAttribute).Name} can be used only on arrays or lists");

                EditorDrawUtility.DrawPropertyField(property);
                return;
            }

            var attribute = (ItemNamesAttribute) baseAttribute;

            if (!m_itemNamesCache.Contains(mightyMember)) InitDrawer(mightyMember, baseAttribute);
            var (valid, namesInfo) = m_itemNamesCache[mightyMember];

            if (!valid)
            {
                EditorDrawUtility.DrawHelpBox($"Callback name: \"{attribute.ItemNamesCallback}\" is invalid");

                EditorDrawUtility.DrawPropertyField(property);
                return;
            }

            var names = namesInfo.Value;

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

            if (!option.Contains(ArrayOption.HideSizeField))
            {
                GUI.enabled = !option.Contains(ArrayOption.DisableSizeField);
                EditorDrawUtility.DrawArraySizeField(property);
                GUI.enabled = true;
            }

            for (var i = 0; i < decoratorLength; i++)
                decoratorDrawers[i].EndDrawHeader(mightyMember, decoratorAttributes[i]);

            EditorDrawUtility.DrawArrayBody(property, index =>
            {
                var element = property.GetArrayElementAtIndex(index);

                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].BeginDrawElement(mightyMember, index, decoratorAttributes[i]);

                var canDrawName = index < names.Length && names[index] is string;

                if (drawer != null)
                {
                    if (canDrawName)
                        drawer.DrawElement(new GUIContent((string) names[index]), mightyMember, index, drawerAttribute);
                    else
                        drawer.DrawElement(mightyMember, index, drawerAttribute);
                }
                else
                {
                    if (canDrawName)
                        EditorGUILayout.PropertyField(element, new GUIContent((string) names[index]));
                    else
                        EditorGUILayout.PropertyField(element);
                }

                for (var i = 0; i < decoratorLength; i++)
                    decoratorDrawers[i].EndDrawElement(mightyMember, index, decoratorAttributes[i]);
            });

            if (!option.Contains(ArrayOption.HideLabel) && !option.Contains(ArrayOption.DontIndent))
                EditorGUI.indentLevel--;


            for (var i = 0; i < decoratorLength; i++)
                decoratorDrawers[i].EndDrawArray(mightyMember, decoratorAttributes[i]);
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            base.InitDrawer(mightyMember, mightyAttribute);
            InternalInitDrawer(mightyMember, mightyAttribute);
        }

        private void InternalInitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            var target = mightyMember.InitAttributeTarget<ItemNamesAttribute>();

            var valid = mightyMember.Property.GetArrayInfoFromMember(target, ((ItemNamesAttribute) mightyAttribute).ItemNamesCallback,
                out var namesInfo);

            m_itemNamesCache[mightyMember] = (valid, namesInfo);
        }

        public override void ClearCache()
        {
            base.ClearCache();
            m_itemNamesCache.ClearCache();
        }

        protected override void RefreshDrawerImpl(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
            if (!m_itemNamesCache.Contains(mightyMember))
            {
                InternalInitDrawer(mightyMember, mightyAttribute);
                return;
            }

            var (valid, info) = m_itemNamesCache[mightyMember];
            if (valid) info.RefreshValue();
        }
    }
}
#endif