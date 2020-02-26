#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public class MightyDrawer
    {
        #region Fields

        private SerializedProperty m_script;
        private Object m_context;

        private Dictionary<string, List<MightyMember<FieldInfo>>> m_groupedFieldsByGroup;
        private Dictionary<string, List<MightyMember<FieldInfo>>> m_conditionalGroupedFieldsByGroup;

        private IEnumerable<BaseClassAttribute> m_classAttributes;

        private MightyMemberCache<FieldInfo> m_serializeFieldsCache;
        private MightyMemberCache<FieldInfo> m_nonSerializeFieldsCache;
        private MightyMemberCache<PropertyInfo> m_propertiesCache;
        private MightyMemberCache<MethodInfo> m_methodsCache;

        private HashSet<string> m_drawnGroups;
        private HashSet<string> m_drawnConditionalGroups;

        private bool m_isMonoScript;

        public HideStatus hideStatus = HideStatus.Nothing;

        #endregion

        #region Enable

        public void OnEnableMonoScript(Object context, SerializedObject serializedObject) =>
            OnEnable(context.GetType(), context, context, serializedObject.FindProperty, true, serializedObject);

        public void OnEnableSerializableClass(Object context, object target, SerializedProperty property) =>
            OnEnable(property.GetSystemType(), context, target, property.FindPropertyRelative, false, null);

        public void OnEnableWindow(MightyWindow window) => OnEnable(window.GetType(), window, window, null, false, null);

        public void OnEnable(Type type, Object context, object target, Func<string, SerializedProperty> findProperty,
            bool isMonoScript, SerializedObject serializedObject)
        {
            m_context = context;
            m_isMonoScript = isMonoScript;

            if (isMonoScript && findProperty != null) m_script = findProperty("m_Script");

            if (context != null)
            {
                m_classAttributes = context.GetType().GetCustomAttributes(typeof(BaseClassAttribute), true).Cast<BaseClassAttribute>()
                    .Where(x => !(x is IExcludeFromAutoRun));


                foreach (var attribute in m_classAttributes)
                {
                    DrawersDatabase.GetDrawerForAttribute<BaseClassDrawer>(attribute.GetType())
                        ?.OnEnableClass(m_script, context, attribute, this);
                }
            }

            if (findProperty != null)
                EnableSerializedFields(type, context, target, findProperty);

            if ((hideStatus & HideStatus.Content) != HideStatus.Content)
            {
                EnableNonSerializedFields(type, context, target, findProperty);
                EnableNativeProperties(type, context, target);
            }

            EnableMethods(type, context, target);

            if (!isMonoScript) return;

            ApplyAutoValues(true);
            if (serializedObject != null && serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
        }

        private void EnableSerializedFields(Type type, Object context, object target, Func<string, SerializedProperty> findProperty)
        {
            var serializedFields = ReflectionUtility.GetAllFields(type, f => findProperty(f.Name) != null).ToArray();

            if (serializedFields.Length == 0) return;

            m_serializeFieldsCache = new MightyMemberCache<FieldInfo>();

            var customWrapperDrawer = new MightyWrapperDrawer();

            if ((hideStatus & HideStatus.Content) != HideStatus.Content)
            {
                m_groupedFieldsByGroup = new Dictionary<string, List<MightyMember<FieldInfo>>>();
                m_drawnGroups = new HashSet<string>();

                m_conditionalGroupedFieldsByGroup = new Dictionary<string, List<MightyMember<FieldInfo>>>();
                m_drawnConditionalGroups = new HashSet<string>();
            }

            foreach (var serializedField in serializedFields)
            {
                var property = findProperty(serializedField.Name);
                var mightyField = m_serializeFieldsCache.Add(new MightyMember<FieldInfo>(serializedField, context, target, property));

                var wrappedAttributes = GetAttributesFromCustomWrappers<BaseFieldAttribute>(serializedField, customWrapperDrawer);

                CacheAutoValueDrawerForField(serializedField, mightyField, wrappedAttributes);

                if ((hideStatus & HideStatus.Content) == HideStatus.Content) continue;

                CacheValidatorsForField(serializedField, mightyField, wrappedAttributes);
                CacheMetasForField(serializedField, mightyField, wrappedAttributes);
                CacheOnValueChangedForField(serializedField, mightyField, wrappedAttributes);
                CacheDecoratorsForField(serializedField, mightyField, wrappedAttributes);
                CacheElementDecoratorsForField(serializedField, mightyField, wrappedAttributes);
                CacheDrawConditionForField(serializedField, mightyField, wrappedAttributes);
                CachePropertyDrawerForField(serializedField, mightyField, wrappedAttributes);
                CacheArrayDrawerForField(serializedField, mightyField, wrappedAttributes);
                CacheConditionnalGrouperForField(serializedField, mightyField, wrappedAttributes);
                CacheGrouperForField(serializedField, mightyField, wrappedAttributes);

                if (mightyField.IsConditionnalGrouped)
                {
                    var groupID = mightyField.GroupID;
                    if (m_conditionalGroupedFieldsByGroup.ContainsKey(groupID))
                        m_conditionalGroupedFieldsByGroup[groupID].Add(mightyField);
                    else m_conditionalGroupedFieldsByGroup[groupID] = new List<MightyMember<FieldInfo>> {mightyField};
                }

                if (mightyField.IsGrouped)
                {
                    var groupID = mightyField.GroupID;
                    if (m_groupedFieldsByGroup.ContainsKey(groupID)) m_groupedFieldsByGroup[groupID].Add(mightyField);
                    else m_groupedFieldsByGroup[groupID] = new List<MightyMember<FieldInfo>> {mightyField};
                }
            }
        }

        private void EnableNonSerializedFields(Type type, Object context, object target, Func<string, SerializedProperty> findProperty)
        {
            var nonSerializedFields = ReflectionUtility.GetAllFields(type,
                f => findProperty == null ||
                     findProperty(f.Name) == null && f.GetCustomAttributes(typeof(BaseMightyAttribute), true).Length > 0).ToArray();

            m_nonSerializeFieldsCache = new MightyMemberCache<FieldInfo>();

            if (nonSerializedFields.Length == 0) return;

            foreach (var nonSerializedField in nonSerializedFields)
            {
                var mightyField = m_nonSerializeFieldsCache.Add(new MightyMember<FieldInfo>(nonSerializedField, context, target));

                CacheNonSerializedField(nonSerializedField, mightyField);
            }
        }

        private void EnableNativeProperties(Type type, Object context, object target)
        {
            var nativeProperties = ReflectionUtility.GetAllProperties(type,
                p => p.GetCustomAttributes(typeof(BaseMightyAttribute), true).Length > 0).ToArray();

            if (nativeProperties.Length == 0) return;

            m_propertiesCache = new MightyMemberCache<PropertyInfo>();

            foreach (var property in nativeProperties)
            {
                var mightyProperty = m_propertiesCache.Add(new MightyMember<PropertyInfo>(property, context, target));

                CacheNativeProperty(property, mightyProperty);
            }
        }

        private void EnableMethods(Type type, Object context, object target)
        {
            var methods = ReflectionUtility.GetAllMethods(type,
                m => m.GetCustomAttributes(typeof(BaseMightyAttribute), true).Length > 0).ToArray();

            if (methods.Length == 0) return;

            m_methodsCache = new MightyMemberCache<MethodInfo>();

            foreach (var method in methods)
            {
                var mightyMethod = m_methodsCache.Add(new MightyMember<MethodInfo>(method, context, target));

                CacheMethod(method, mightyMethod);

                if (GetOnEnableDrawer(mightyMethod, out var drawer, out var onEnableAttribute))
                    drawer.DrawMethod(mightyMethod, onEnableAttribute);

                if (GetOnModifiedPropertiesDrawer(mightyMethod, out drawer, out var onModifiedPropertiesAttribute))
                    drawer.DrawMethod(mightyMethod, onModifiedPropertiesAttribute);
            }
        }

        #endregion /Enable

        #region Disable

        public void OnDisable()
        {
            if (m_classAttributes != null)
                foreach (var attribute in m_classAttributes)
                    DrawersDatabase.GetDrawerForAttribute<BaseClassDrawer>(attribute)?.OnDisableClass(m_script, m_context, attribute, this);

            m_conditionalGroupedFieldsByGroup?.Clear();
            m_drawnConditionalGroups?.Clear();

            m_groupedFieldsByGroup?.Clear();
            m_drawnGroups?.Clear();

            m_serializeFieldsCache?.ClearCache();
            m_nonSerializeFieldsCache?.ClearCache();
            m_propertiesCache?.ClearCache();
            m_methodsCache?.ClearCache();
        }

        #endregion /Disable

        #region InspectorGUI

        public bool OnGUI(Object context, SerializedObject serializedObject)
        {
            var drawn = BeginOnGUI(context);
            drawn = DrawSerializedFields(serializedObject, out var valueChanged) || drawn;
            drawn = DrawNonSerialized(valueChanged) || drawn;

            if (m_isMonoScript && valueChanged) ManageValueChanged(serializedObject);

            drawn = EndOnGUI(context) || drawn;
            return drawn;
        }

        public bool BeginOnGUI(Object context)
        {
            if (m_classAttributes == null) return true;
            foreach (var attribute in m_classAttributes)
                DrawersDatabase.GetDrawerForAttribute<BaseClassDrawer>(attribute)?.BeginDrawClass(m_script, context, attribute, this);
            return true;
        }

        public bool EndOnGUI(Object context)
        {
            if (m_classAttributes == null)
            {
                hideStatus = HideStatus.Nothing;
                return false;
            }

            foreach (var attribute in m_classAttributes)
                DrawersDatabase.GetDrawerForAttribute<BaseClassDrawer>(attribute)?.EndDrawClass(m_script, context, attribute, this);

            hideStatus = HideStatus.Nothing;
            return true;
        }

        public bool DrawSerializedFields(SerializedObject serializedObject, out bool valueChanged)
        {
            if (m_serializeFieldsCache == null || m_serializeFieldsCache.Count == 0)
            {
                valueChanged = false;
                return false;
            }

            if ((hideStatus & HideStatus.ScriptField) != HideStatus.ScriptField && m_isMonoScript)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(m_script);
                GUI.enabled = true;
            }

            m_drawnGroups.Clear();
            m_drawnConditionalGroups.Clear();

            EditorGUI.BeginChangeCheck();

            foreach (var member in m_serializeFieldsCache.Values) ManageField(member);

            valueChanged = EditorGUI.EndChangeCheck();

            return true;
        }

        public bool DrawNonSerialized(bool valueChanged)
        {
            var drawn = false;
            if ((hideStatus & HideStatus.Content) != HideStatus.Content)
            {
                if (m_nonSerializeFieldsCache != null)
                {
                    foreach (var mightyField in m_nonSerializeFieldsCache.Values)
                    {
                        var hasDecorators = GetDecoratorDrawers(mightyField, out var decorators, out var decoratorAttributes);

                        if (hasDecorators)
                            for (var i = 0; i < decoratorAttributes.Length; i++)
                                if (decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)
                                    drawAnywhereDecorator.BeginDrawAnywhere(mightyField, decoratorAttributes[i]);

                        if (GetFieldDrawer(mightyField, out var drawer, out var attribute))
                            drawer.DrawField(mightyField, attribute);

                        if (hasDecorators)
                            for (var i = 0; i < decoratorAttributes.Length; i++)
                                if (decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)
                                    drawAnywhereDecorator.EndDrawAnywhere(mightyField, decoratorAttributes[i]);
                    }

                    drawn = true;
                }

                if (m_propertiesCache != null)
                {
                    foreach (var mightyProperty in m_propertiesCache.Values)
                    {
                        var hasDecorators = GetDecoratorDrawers(mightyProperty, out var decorators, out var decoratorAttributes);

                        if (hasDecorators)
                            for (var i = 0; i < decoratorAttributes.Length; i++)
                                if (decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)
                                    drawAnywhereDecorator.BeginDrawAnywhere(mightyProperty, decoratorAttributes[i]);

                        if (GetNativePropertyDrawer(mightyProperty, out var drawer, out var attribute))
                            drawer.DrawNativeProperty(mightyProperty, attribute);

                        if (hasDecorators)
                            for (var i = 0; i < decoratorAttributes.Length; i++)
                                if (decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)
                                    drawAnywhereDecorator.EndDrawAnywhere(mightyProperty, decoratorAttributes[i]);
                    }

                    drawn = true;
                }
            }

            if (m_methodsCache != null)
            {
                foreach (var mightyMethod in m_methodsCache.Values)
                {
                    if ((hideStatus & HideStatus.Content) != HideStatus.Content)
                    {
                        if (GetOnInspectorGUIMethodDrawer(mightyMethod, out var drawer, out var attribute))
                            drawer.DrawMethod(mightyMethod, attribute);

                        if (valueChanged && GetOnModifiedPropertiesDrawer(mightyMethod, out var methodInvokerDrawer,
                                out var onModifiedPropertiesAttribute))
                            methodInvokerDrawer.DrawMethod(mightyMethod, onModifiedPropertiesAttribute);
                    }
                    else
                    {
                        var hasDecorators = GetDecoratorDrawers(mightyMethod, out var decorators, out var decoratorAttributes);

                        if (hasDecorators)
                            for (var i = 0; i < decoratorAttributes.Length; i++)
                                if (decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)
                                    drawAnywhereDecorator.BeginDrawAnywhere(mightyMethod, decoratorAttributes[i]);

                        if (GetOnInspectorGUIMethodDrawer(mightyMethod, out var drawer, out var attribute))
                            drawer.DrawMethod(mightyMethod, attribute);

                        if (valueChanged && GetOnModifiedPropertiesDrawer(mightyMethod, out var methodInvokerDrawer,
                                out var onModifiedPropertiesAttribute))
                            methodInvokerDrawer.DrawMethod(mightyMethod, onModifiedPropertiesAttribute);

                        if (hasDecorators)
                            for (var i = 0; i < decoratorAttributes.Length; i++)
                                if (decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)
                                    drawAnywhereDecorator.EndDrawAnywhere(mightyMethod, decoratorAttributes[i]);
                    }
                }

                drawn = true;
            }

            return drawn;
        }

        public void ManageValueChanged(SerializedObject serializedObject)
        {
            if (serializedObject != null && serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
            serializedObject?.UpdateIfRequiredOrScript();
            
            ApplyAutoValues(true);
            
            if (serializedObject != null && serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
            serializedObject?.UpdateIfRequiredOrScript();
        }

        public void ManageField(MightyMember<FieldInfo> mightyField)
        {
            if (!mightyField.IsSerialized) return;

            if ((hideStatus & HideStatus.Content) == HideStatus.Content) return;

            if (mightyField.IsConditionnalGrouped)
            {
                // Draw conditional grouped fields
                var groupID = mightyField.GroupID;
                if (m_drawnConditionalGroups.Contains(groupID)) return;

                if (!GetConditionnalGrouper(mightyField, out var conditionalGrouper, out var conditionalGroupAttribute))
                {
                    ValidateAndDrawField(mightyField);
                    return;
                }

                var currentFields = m_conditionalGroupedFieldsByGroup[groupID];

                var canDrawGroup = true;
                var flag = false;
                foreach (var currentField in currentFields)
                    canDrawGroup = flag = !GetPropertyDrawCondition(currentField, out var condition, out var conditionAttribute) ||
                                          condition.CanDrawProperty(currentField, conditionAttribute) || flag;

                if (canDrawGroup)
                {
                    m_drawnConditionalGroups.Add(groupID);

                    var canDraw = conditionalGrouper.CanDraw(mightyField, conditionalGroupAttribute);

                    if (canDraw)
                    {
                        conditionalGrouper.BeginGroup(mightyField);
                        ValidateAndDrawFields(currentFields);
                    }

                    conditionalGrouper.EndGroup(mightyField, canDraw);
                }
            }
            else if (mightyField.IsGrouped)
            {
                // Draw grouped fields
                var groupID = mightyField.GroupID;
                if (m_drawnGroups.Contains(groupID)) return;

                if (!GetPropertyGrouper(mightyField, out var grouper, out var groupAttribute))
                {
                    ValidateAndDrawField(mightyField);
                    return;
                }

                var currentFields = m_groupedFieldsByGroup[groupID];

                var canDrawGroup = true;
                var flag = false;
                foreach (var currentField in currentFields)
                    canDrawGroup = flag = !GetPropertyDrawCondition(currentField, out var condition, out var conditionAttribute) ||
                                          condition.CanDrawProperty(currentField, conditionAttribute) || flag;

                if (canDrawGroup)
                {
                    m_drawnGroups.Add(groupID);

                    grouper.BeginGroup(mightyField, groupAttribute);

                    ValidateAndDrawFields(currentFields);

                    grouper.EndGroup(mightyField);
                }
            }
            else ValidateAndDrawField(mightyField);
        }

        private void ValidateAndDrawFields(List<MightyMember<FieldInfo>> serializedFields, bool canDraw = true)
        {
            foreach (var field in serializedFields) ValidateAndDrawField(field, canDraw);
        }

        private void ValidateAndDrawField(MightyMember<FieldInfo> serializedField, bool canDraw = true)
        {
            if (canDraw)
            {
                ValidateField(serializedField);
                ApplyFieldMeta(serializedField);
            }

            DrawField(serializedField, canDraw && (hideStatus & HideStatus.Content) != HideStatus.Content);
        }

        private void ValidateField(MightyMember<FieldInfo> serializedField)
        {
            if (!serializedField.GetDrawers(out BasePropertyValidator[] validators, out BaseValidatorAttribute[] attributes)) return;

            for (var i = 0; i < validators.Length; i++)
                validators[i].ValidateProperty(serializedField, attributes[i]);
        }

        private void ApplyFieldMeta(MightyMember<FieldInfo> serializedField)
        {
            if (!serializedField.GetDrawers(out BasePropertyMeta[] metas, out BaseMetaAttribute[] attributes)) return;

            for (var i = 0; i < metas.Length; i++)
                metas[i].ApplyPropertyMeta(serializedField, attributes[i]);
        }

        private void DrawField(MightyMember<FieldInfo> serializedField, bool canDraw)
        {
            if (!canDraw) return;

            var hasDecorators = GetDecoratorDrawers(serializedField, out var decorators, out var decoratorAttributes);
            var hasElementDecorators =
                GetElementDecoratorDrawers(serializedField, out var elementDecorators, out var elementDecoratorAttributes);

            if (GetPropertyDrawCondition(serializedField, out var condition, out var conditionAttribute))
            {
                var canDrawProperty = condition.CanDrawProperty(serializedField, conditionAttribute);
                if (!canDrawProperty)
                {
                    if (hasDecorators)
                        for (var i = 0; i < decorators.Length; i++)
                        {
                            if (!(decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)) continue;

                            var attribute = decoratorAttributes[i];
                            drawAnywhereDecorator.BeginDrawAnywhere(serializedField, attribute);
                        }

                    if (hasElementDecorators)
                        for (var i = 0; i < elementDecorators.Length; i++)
                        {
                            if (!(elementDecorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)) continue;

                            var attribute = elementDecoratorAttributes[i];
                            drawAnywhereDecorator.BeginDrawAnywhere(serializedField, attribute);
                            drawAnywhereDecorator.EndDrawAnywhere(serializedField, attribute);
                        }

                    if (hasDecorators)
                        for (var i = 0; i < decorators.Length; i++)
                        {
                            if (!(decorators[i] is IDrawAnywhereDecorator drawAnywhereDecorator)) continue;

                            var attribute = decoratorAttributes[i];
                            drawAnywhereDecorator.EndDrawAnywhere(serializedField, attribute);
                        }

                    return;
                }
            }

            if (hasDecorators)
                for (var i = 0; i < decorators.Length; i++)
                    decorators[i].BeginDraw(serializedField, decoratorAttributes[i]);

            var length = hasElementDecorators ? elementDecorators.Length : 0;

            var hasArrayDrawer = GetArrayDrawer(serializedField, out var arrayDrawer, out var arrayAttribute);

            if (!hasArrayDrawer && hasElementDecorators)
                for (var i = 0; i < length - 1; i++)
                    elementDecorators[i].BeginDraw(serializedField, elementDecoratorAttributes[i], null);

            var hasDrawer = GetPropertyDrawer(serializedField, out var drawer, out var drawerAttribute);

            EditorGUI.BeginChangeCheck();

            // Draw the field
            if (!hasArrayDrawer)
            {
                DrawPropertyField(hasElementDecorators, serializedField, (m, p, a) =>
                {
                    if (hasDrawer)
                        drawer.DrawProperty(m, p, a);
                    else
                        EditorDrawUtility.DrawPropertyField(p);
                }, elementDecorators, elementDecoratorAttributes, drawerAttribute);
            }

            if (hasArrayDrawer) arrayDrawer.DrawArray(serializedField, arrayAttribute, drawer as IArrayElementDrawer, drawerAttribute);

            if (EditorGUI.EndChangeCheck()) ApplyOnValueChanged(serializedField);

            if (!hasArrayDrawer && hasElementDecorators)
                for (var i = length - 2; i >= 0; i--)
                    elementDecorators[i].EndDraw(serializedField, elementDecoratorAttributes[i], null);

            if (hasDecorators)
                for (var i = decorators.Length - 1; i >= 0; i--)
                    decorators[i].EndDraw(serializedField, decoratorAttributes[i]);
        }

        private void DrawPropertyField(bool fromElementDecorator, BaseMightyMember mightyMember,
            Action<BaseMightyMember, SerializedProperty, BaseDrawerAttribute> drawCallback,
            BaseElementDecoratorDrawer[] elementDecoratorDrawers,
            BaseElementDecoratorAttribute[] elementDecoratorAttributes, BaseDrawerAttribute drawerAttribute = null)
        {
            if (fromElementDecorator)
            {
                elementDecoratorDrawers.Last().BeginDraw(mightyMember, elementDecoratorAttributes.Last(), drawCallback, drawerAttribute);
                elementDecoratorDrawers.Last().EndDraw(mightyMember, elementDecoratorAttributes.Last(), drawCallback, drawerAttribute);
            }
            else
                drawCallback(mightyMember, mightyMember.Property, drawerAttribute);
        }

        public void ApplyAutoValues(bool refreshDrawers)
        {
            if (m_serializeFieldsCache != null)
                foreach (var serializeField in m_serializeFieldsCache.Values)
                {
                    var property = serializeField.Property;
                    ApplyAutoValue(serializeField, property);

                    if (serializeField.GetDrawer<NestPropertyDrawer, NestAttribute>(out var nestDrawer, out var attribute))
                        nestDrawer.ApplyAutoValues(serializeField, attribute, refreshDrawers);

                    if (refreshDrawers)
                        RefreshDrawers(serializeField);
                }

            if (!refreshDrawers) return;

            RefreshAllDrawers(false);
        }

        public void RefreshAllDrawers(bool refreshSerializeFields = true)
        {
            if (refreshSerializeFields && m_serializeFieldsCache != null)
                foreach (var serializeField in m_serializeFieldsCache.Values)
                    RefreshDrawers(serializeField);

            if (m_nonSerializeFieldsCache != null)
                foreach (var mightyField in m_nonSerializeFieldsCache.Values)
                    RefreshDrawers(mightyField);

            if (m_propertiesCache != null)
                foreach (var mightyProperty in m_propertiesCache.Values)
                    RefreshDrawers(mightyProperty);

            if (m_methodsCache != null)
                foreach (var mightyMethod in m_methodsCache.Values)
                    RefreshDrawers(mightyMethod);
        }

        private void ApplyAutoValue(BaseMightyMember mightyMember, SerializedProperty property)
        {
            if (!GetAutoValueDrawer(mightyMember, out var autoValue, out var autoValueAttribute)) return;

            var state = autoValue.InitProperty(property, autoValueAttribute);
            if (!state.isOk) EditorDrawUtility.DrawHelpBox(state.message);
        }

        private void ApplyOnValueChanged(BaseMightyMember mightyMember)
        {
            if (mightyMember.GetDrawer(out OnValueChangedPropertyMeta onValueChanged, out OnValueChangedAttribute attribute))
                onValueChanged.ApplyPropertyMeta(mightyMember, attribute);
        }

        private void RefreshDrawers(BaseMightyMember mightyMember)
        {
            var (drawers, attributes) = mightyMember.GetRefreshDrawers();
            for (var i = 0; i < drawers.Length; i++) drawers[i].RefreshDrawer(mightyMember, attributes[i]);
        }

        #endregion /InspectorGUI

        #region Mighty Members Cache

        #region Generics

        private bool CacheSinglePair<Td, Ta>(BaseMightyMember member, object[] attributes, Func<Type, Td> predicate, out Td outDrawer,
            out Ta outAttribute)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
        {
            foreach (var attribute in attributes)
                if (CacheSinglePair(member, attribute, predicate, out outDrawer, out outAttribute))
                    return true;

            outDrawer = default;
            outAttribute = default;
            return false;
        }

        private bool CacheSinglePair<Td, Ta>(BaseMightyMember member, object[] attributes, Func<Type, Td> predicate)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
        {
            foreach (var attribute in attributes)
                if (CacheSinglePair<Td, Ta>(member, attribute, predicate))
                    return true;

            return false;
        }

        private bool CacheSinglePair<Td, Ta>(BaseMightyMember member, object attribute, Func<Type, Td> predicate, out Td outDrawer,
            out Ta outAttribute)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
        {
            if (attribute is IExcludeFromAutoRun || !(attribute is Ta mightyAttribute) ||
                !(predicate(mightyAttribute.GetType()) is Td mightyDrawer))
            {
                outDrawer = default;
                outAttribute = default;
                return false;
            }

            member.SetDrawer(mightyDrawer, mightyAttribute);
            outDrawer = mightyDrawer;
            outAttribute = mightyAttribute;
            return true;
        }

        private bool CacheSinglePair<Td, Ta>(BaseMightyMember member, object attribute, Func<Type, Td> predicate)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
        {
            if (attribute is IExcludeFromAutoRun || !(attribute is Ta mightyAttribute) ||
                !(predicate(mightyAttribute.GetType()) is Td mightyDrawer)) return false;

            member.SetDrawer(mightyDrawer, mightyAttribute);
            return true;
        }

        private bool PopulatePairLists<Td, Ta>(List<Td> drawers, List<Ta> attributes, object[] allAttributes, Func<Type, Td> predicate)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
        {
            var any = false;
            foreach (var attribute in allAttributes)
            {
                if (attribute is IExcludeFromAutoRun || !(attribute is Ta mightyAttribute) ||
                    !(predicate(attribute.GetType()) is Td mightyDrawer)) continue;

                any = true;

                drawers.Add(mightyDrawer);
                attributes.Add(mightyAttribute);
            }

            return any;
        }

        private bool PopulatePairListsWithAttributeException<Td, Ta>(List<Td> drawers, List<Ta> attributes, object[] allAttributes,
            Func<Type, Td> predicate, Type attributeExceptionType, bool shouldBeException)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
        {
            var any = false;
            foreach (var attribute in allAttributes)
            {
                if (attribute is IExcludeFromAutoRun || !(attribute is Ta mightyAttribute) ||
                    attributeExceptionType.IsInstanceOfType(mightyAttribute) != shouldBeException ||
                    !(predicate(attribute.GetType()) is Td mightyDrawer)) continue;

                any = true;

                drawers.Add(mightyDrawer);
                attributes.Add(mightyAttribute);
            }

            return any;
        }

        private bool PopulatePairListsWithDrawerException<Td, Ta>(List<Td> drawers, List<Ta> attributes, object[] allAttributes,
            Func<Type, Td> predicate, Type drawerExceptionType, bool shouldBeException)
            where Td : BaseMightyDrawer where Ta : BaseMightyAttribute
        {
            var any = false;
            foreach (var attribute in allAttributes)
            {
                if (attribute is IExcludeFromAutoRun || !(attribute is Ta mightyAttribute) ||
                    !(predicate(attribute.GetType()) is Td mightyDrawer) ||
                    drawerExceptionType.IsInstanceOfType(mightyDrawer) != shouldBeException) continue;

                any = true;

                drawers.Add(mightyDrawer);
                attributes.Add(mightyAttribute);
            }

            return any;
        }

        #endregion /Generics

        private void CacheNonSerializedField(FieldInfo field, BaseMightyMember mightyMember)
        {
            var decoratorDrawers = new List<BaseGlobalDecoratorDrawer>();
            var decoratorAttributes = new List<BaseGlobalDecoratorAttribute>();

            if (PopulatePairListsWithDrawerException(decoratorDrawers, decoratorAttributes,
                field.GetCustomAttributes(typeof(BaseGlobalDecoratorAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseGlobalDecoratorDrawer>,
                typeof(IDrawAnywhereDecorator), true))
                mightyMember.SetDrawers(decoratorDrawers.ToArray(), decoratorAttributes.ToArray());

            CacheSinglePair<BaseFieldDrawer, BaseDrawerAttribute>(mightyMember,
                field.GetCustomAttributes(typeof(BaseDrawerAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseFieldDrawer>);
        }

        private void CacheNativeProperty(PropertyInfo property, BaseMightyMember mightyMember)
        {
            var decoratorDrawers = new List<BaseDecoratorDrawer>();
            var decoratorAttributes = new List<BaseDecoratorAttribute>();

            if (PopulatePairLists(decoratorDrawers, decoratorAttributes,
                property.GetCustomAttributes(typeof(BaseElementDecoratorAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseDecoratorDrawer>))
                mightyMember.SetDrawers(decoratorDrawers.ToArray(), decoratorAttributes.ToArray());

            CacheSinglePair<BaseNativePropertyDrawer, BasePropertyAttribute>(mightyMember,
                property.GetCustomAttributes(typeof(BasePropertyAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseNativePropertyDrawer>);
        }

        private void CacheMethod(MethodInfo method, BaseMightyMember mightyMember)
        {
            var decoratorDrawers = new List<BaseDecoratorDrawer>();
            var decoratorAttributes = new List<BaseDecoratorAttribute>();

            if (PopulatePairLists(decoratorDrawers, decoratorAttributes,
                method.GetCustomAttributes(typeof(BaseElementDecoratorAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseDecoratorDrawer>))
                mightyMember.SetDrawers(decoratorDrawers.ToArray(), decoratorAttributes.ToArray());

            CacheSinglePair<MethodInvokerDrawer, OnEnableAttribute>(mightyMember,
                method.GetCustomAttributes(typeof(OnEnableAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<MethodInvokerDrawer>);

            CacheSinglePair<MethodInvokerDrawer, OnModifiedPropertiesAttribute>(mightyMember,
                method.GetCustomAttributes(typeof(OnModifiedPropertiesAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<MethodInvokerDrawer>);

            CacheSinglePair<BaseMethodDrawer, BaseOnInspectorGUIMethodAttribute>(mightyMember,
                method.GetCustomAttributes(typeof(BaseOnInspectorGUIMethodAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseMethodDrawer>);
        }

        private void CacheValidatorsForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            var validators = new List<BasePropertyValidator>();
            var validatorAttributes = new List<BaseValidatorAttribute>();

            var any = PopulatePairLists(validators, validatorAttributes,
                field.GetCustomAttributes(typeof(BaseValidatorAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BasePropertyValidator>);

            any = PopulatePairLists(validators, validatorAttributes, wrappedAttributes,
                      DrawersDatabase.GetDrawerForAttribute<BasePropertyValidator>) || any;

            if (any) mightyMember.SetDrawers(validators.ToArray(), validatorAttributes.ToArray());
        }

        private void CacheMetasForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            var validators = new List<BasePropertyValidator>();
            var validatorAttributes = new List<BaseValidatorAttribute>();

            var any = PopulatePairListsWithAttributeException(validators, validatorAttributes,
                field.GetCustomAttributes(typeof(BaseElementDecoratorAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BasePropertyValidator>, typeof(OnValueChangedAttribute), false);

            any = PopulatePairListsWithAttributeException(validators, validatorAttributes, wrappedAttributes,
                      DrawersDatabase.GetDrawerForAttribute<BasePropertyValidator>, typeof(OnValueChangedAttribute), false) || any;

            if (any) mightyMember.SetDrawers(validators.ToArray(), validatorAttributes.ToArray());
        }

        private void CacheOnValueChangedForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            if (!CacheSinglePair<OnValueChangedPropertyMeta, OnValueChangedAttribute>(mightyMember,
                field.GetCustomAttributes(typeof(OnValueChangedAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<OnValueChangedPropertyMeta>))
                return;

            CacheSinglePair<OnValueChangedPropertyMeta, OnValueChangedAttribute>(mightyMember, wrappedAttributes,
                DrawersDatabase.GetDrawerForAttribute<OnValueChangedPropertyMeta>);
        }

        private void CacheElementDecoratorsForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            var decoratorDrawers = new List<BaseElementDecoratorDrawer>();
            var decoratorAttributes = new List<BaseElementDecoratorAttribute>();

            var any = PopulatePairLists(decoratorDrawers, decoratorAttributes,
                field.GetCustomAttributes(typeof(BaseElementDecoratorAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseElementDecoratorDrawer>);

            any = PopulatePairLists(decoratorDrawers, decoratorAttributes, wrappedAttributes,
                      DrawersDatabase.GetDrawerForAttribute<BaseElementDecoratorDrawer>) || any;

            if (any) mightyMember.SetDrawers(decoratorDrawers.ToArray(), decoratorAttributes.ToArray());
        }

        private void CacheDecoratorsForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            var decoratorDrawers = new List<BaseDecoratorDrawer>();
            var decoratorAttributes = new List<BaseDecoratorAttribute>();

            var any = PopulatePairLists(decoratorDrawers, decoratorAttributes,
                field.GetCustomAttributes(typeof(BaseDecoratorAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseDecoratorDrawer>);

            any = PopulatePairLists(decoratorDrawers, decoratorAttributes, wrappedAttributes,
                      DrawersDatabase.GetDrawerForAttribute<BaseDecoratorDrawer>) || any;

            if (any) mightyMember.SetDrawers(decoratorDrawers.ToArray(), decoratorAttributes.ToArray());
        }

        private void CachePropertyDrawerForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            if (CacheSinglePair<BasePropertyDrawer, BaseDrawerAttribute>(mightyMember,
                field.GetCustomAttributes(typeof(BaseDrawerAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BasePropertyDrawer>)) return;

            CacheSinglePair<BasePropertyDrawer, BaseDrawerAttribute>(mightyMember, wrappedAttributes,
                DrawersDatabase.GetDrawerForAttribute<BasePropertyDrawer>);
        }

        private void CacheArrayDrawerForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            if (CacheSinglePair<BaseArrayDrawer, BaseArrayAttribute>(mightyMember,
                field.GetCustomAttributes(typeof(BaseArrayAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseArrayDrawer>)) return;

            CacheSinglePair<BaseArrayDrawer, BaseArrayAttribute>(mightyMember, wrappedAttributes,
                DrawersDatabase.GetDrawerForAttribute<BaseArrayDrawer>);
        }

        private void CacheAutoValueDrawerForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            if (CacheSinglePair<BaseAutoValueDrawer, BaseAutoValueAttribute>(mightyMember,
                field.GetCustomAttributes(typeof(BaseAutoValueAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BaseAutoValueDrawer>)) return;

            CacheSinglePair<BaseAutoValueDrawer, BaseAutoValueAttribute>(mightyMember, wrappedAttributes,
                DrawersDatabase.GetDrawerForAttribute<BaseAutoValueDrawer>);
        }

        private void CacheDrawConditionForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            if (CacheSinglePair<BasePropertyDrawCondition, BaseDrawConditionAttribute>(mightyMember,
                field.GetCustomAttributes(typeof(BaseDrawConditionAttribute), true),
                DrawersDatabase.GetDrawerForAttribute<BasePropertyDrawCondition>)) return;

            CacheSinglePair<BasePropertyDrawCondition, BaseDrawConditionAttribute>(mightyMember, wrappedAttributes,
                DrawersDatabase.GetDrawerForAttribute<BasePropertyDrawCondition>);
        }

        private void CacheGrouperForField(FieldInfo field, BaseMightyMember mightyMember, BaseFieldAttribute[] wrappedAttributes)
        {
            if (!CacheSinglePair(mightyMember, field.GetCustomAttributes(typeof(BaseGroupAttribute), true),
                    DrawersDatabase.GetDrawerForAttribute<BasePropertyGrouper>, out var grouper, out BaseGroupAttribute attribute) &&
                !CacheSinglePair(mightyMember, wrappedAttributes, DrawersDatabase.GetDrawerForAttribute<BasePropertyGrouper>, out grouper,
                    out attribute))
                return;

            mightyMember.IsGrouped = true;
            mightyMember.GroupName = grouper.GetName(mightyMember, attribute);
        }

        private void CacheConditionnalGrouperForField(FieldInfo field, BaseMightyMember mightyMember,
            BaseFieldAttribute[] wrappedAttributes)
        {
            if (!CacheSinglePair(mightyMember, field.GetCustomAttributes(typeof(BaseConditionalGroupAttribute), true),
                    DrawersDatabase.GetDrawerForAttribute<BaseConditionalGrouper>, out var grouper,
                    out BaseConditionalGroupAttribute attribute) &&
                !CacheSinglePair(mightyMember, wrappedAttributes, DrawersDatabase.GetDrawerForAttribute<BaseConditionalGrouper>,
                    out grouper,
                    out attribute))
                return;

            mightyMember.IsConditionnalGrouped = true;
            mightyMember.GroupName = grouper.GetName(mightyMember, attribute);
        }

        #endregion /Mighty Members Initializers

        #region Drawers Getters

        private bool GetElementDecoratorDrawers(BaseMightyMember member, out BaseElementDecoratorDrawer[] decorators,
            out BaseElementDecoratorAttribute[] baseAttributes) => member.GetDrawers(out decorators, out baseAttributes);

        private bool GetDecoratorDrawers(BaseMightyMember member, out BaseDecoratorDrawer[] decorators,
            out BaseDecoratorAttribute[] baseAttributes) => member.GetDrawers(out decorators, out baseAttributes);

        private bool GetPropertyDrawer(BaseMightyMember member, out BasePropertyDrawer drawer, out BaseDrawerAttribute baseAttribute) =>
            member.GetDrawer(out drawer, out baseAttribute);

        private bool GetFieldDrawer(BaseMightyMember member, out BaseFieldDrawer drawer, out BaseDrawerAttribute baseAttribute) =>
            member.GetDrawer(out drawer, out baseAttribute);

        private bool GetNativePropertyDrawer(BaseMightyMember member, out BaseNativePropertyDrawer drawer,
            out BasePropertyAttribute baseAttribute) => member.GetDrawer(out drawer, out baseAttribute);

        private bool GetOnInspectorGUIMethodDrawer(BaseMightyMember member, out BaseMethodDrawer drawer,
            out BaseOnInspectorGUIMethodAttribute baseAttribute) => member.GetDrawer(out drawer, out baseAttribute);

        private bool GetOnEnableDrawer(BaseMightyMember member, out MethodInvokerDrawer drawer,
            out OnEnableAttribute baseAttribute) => member.GetDrawer(out drawer, out baseAttribute);

        private bool GetOnModifiedPropertiesDrawer(BaseMightyMember member, out MethodInvokerDrawer drawer,
            out OnModifiedPropertiesAttribute baseAttribute) => member.GetDrawer(out drawer, out baseAttribute);

        private bool GetArrayDrawer(BaseMightyMember member, out BaseArrayDrawer drawer, out BaseArrayAttribute baseAttribute) =>
            member.GetDrawer(out drawer, out baseAttribute);

        private bool GetAutoValueDrawer(BaseMightyMember member, out BaseAutoValueDrawer drawer,
            out BaseAutoValueAttribute baseAttribute) => member.GetDrawer(out drawer, out baseAttribute);

        private bool GetPropertyDrawCondition(BaseMightyMember member, out BasePropertyDrawCondition condition,
            out BaseDrawConditionAttribute baseAttribute) => member.GetDrawer(out condition, out baseAttribute);

        private bool GetPropertyGrouper(BaseMightyMember member, out BasePropertyGrouper grouper, out BaseGroupAttribute baseAttribute)
            => member.GetDrawer(out grouper, out baseAttribute);

        private bool GetConditionnalGrouper(BaseMightyMember member, out BaseConditionalGrouper grouper,
            out BaseConditionalGroupAttribute baseAttribute) => member.GetDrawer(out grouper, out baseAttribute);

        #endregion /Drawers Getters

        #region Wrapped Attributes Getters

        public static T[] GetAttributesFromCustomWrappers<T>(FieldInfo field, MightyWrapperDrawer drawer) where T : Attribute
        {
            var wrappedAttributes = new List<T>();

            if (field.GetCustomAttributes(typeof(MightyWrapperAttribute), true) is MightyWrapperAttribute[] MightyWrapperAttributes
                && MightyWrapperAttributes.Length > 0)
                GetAttributesFromCustomWrapper(drawer, MightyWrapperAttributes, wrappedAttributes);

            return wrappedAttributes.ToArray();
        }

        public static void GetAttributesFromCustomWrapper<T>(MightyWrapperDrawer drawer, MightyWrapperAttribute[] wrappers,
            List<T> wrappedAttributes) where T : Attribute
        {
            foreach (var wrapper in wrappers)
            {
                var newWrapperAttributes = drawer.GetAttributes<MightyWrapperAttribute>(wrapper.GetType());
                if (newWrapperAttributes.Length > 0)
                    GetAttributesFromCustomWrapper(drawer, newWrapperAttributes, wrappedAttributes);

                wrappedAttributes.AddRange(drawer.GetAttributes<T>(wrapper.GetType()).Where(x => !(x is MightyWrapperAttribute)));
            }
        }

        public static object[] GetAttributesFromCustomWrappers(Type attributeType, FieldInfo field, MightyWrapperDrawer drawer)
        {
            var wrappedAttributes = new List<object>();

            if (field.GetCustomAttributes(typeof(MightyWrapperAttribute), true) is MightyWrapperAttribute[] MightyWrapperAttributes &&
                MightyWrapperAttributes.Length > 0)
                GetAttributesFromCustomWrapper(attributeType, drawer, MightyWrapperAttributes, wrappedAttributes);

            return wrappedAttributes.ToArray();
        }

        public static void GetAttributesFromCustomWrapper(Type attributeType, MightyWrapperDrawer drawer, MightyWrapperAttribute[] wrappers,
            List<object> wrappedAttributes)
        {
            foreach (var wrapper in wrappers)
            {
                var newWrapperAttributes = drawer.GetAttributes<MightyWrapperAttribute>(wrapper.GetType());
                if (newWrapperAttributes.Length > 0)
                    GetAttributesFromCustomWrapper(attributeType, drawer, newWrapperAttributes, wrappedAttributes);

                wrappedAttributes.AddRange(
                    drawer.GetAttributes(wrapper.GetType(), attributeType).Where(x => !(x is MightyWrapperAttribute)));
            }
        }

        #endregion /Wrapped Attributes Getters
    }
}
#endif