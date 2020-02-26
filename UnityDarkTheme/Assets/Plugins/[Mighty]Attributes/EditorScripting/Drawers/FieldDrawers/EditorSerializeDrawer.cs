#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    [DrawerTarget(typeof(EditorSerializeAttribute))]
    public class EditorSerializeDrawer : BaseFieldDrawer
    {
        public delegate object DrawCallback(EditorSerializeAttribute attribute, Object context, object target, object value);

        public void DrawField(string fieldName, Object context) => DrawField(fieldName, context, context);

        public void DrawField(string fieldName, Object context, object target)
        {
            var field = target.GetField(fieldName);
            var attribute = field.GetCustomAttribute<EditorSerializeAttribute>();

            if (attribute.OldName != null)
                EditorFieldsDatabase.RenameField(context, attribute.OldName, field.Name);

            var editorField = EditorFieldsDatabase.GetEditorField(context, field.Name);
            var value = field.GetValue(target);

            Deserialize(attribute, editorField, target, field, ref value);

            EditorGUI.BeginChangeCheck();

            value = EditorDrawUtility.DrawLayoutField(field, context, target, value,
                !attribute.Options.Contains(EditorFieldOption.DontFold), attribute.Options.Contains(EditorFieldOption.Asset));

            if (EditorGUI.EndChangeCheck()) Serialize(attribute, editorField, value, field.FieldType);
        }

        public void DrawField(string fieldName, Object context, DrawCallback drawCallback) =>
            DrawField(fieldName, context, context, drawCallback);

        public void DrawField(string fieldName, Object context, object target, DrawCallback drawCallback)
        {
            var field = target.GetField(fieldName);
            var attribute = field.GetCustomAttribute<EditorSerializeAttribute>();

            if (attribute.OldName != null)
                EditorFieldsDatabase.RenameField(context, attribute.OldName, field.Name);

            var editorField = EditorFieldsDatabase.GetEditorField(context, field.Name);
            var value = field.GetValue(target);

            Deserialize(attribute, editorField, target, field, ref value);

            EditorGUI.BeginChangeCheck();

            value = drawCallback(attribute, context, target, value);

            if (EditorGUI.EndChangeCheck()) Serialize(attribute, editorField, value, field.FieldType);
        }

        public override void DrawField(MightyMember<FieldInfo> mightyMember, BaseDrawerAttribute baseAttribute)
        {
            var attribute = (EditorSerializeAttribute) baseAttribute;
            var context = mightyMember.Context;
            var field = mightyMember.MemberInfo;
            var target = mightyMember.Target;

            if (attribute.OldName != null)
                EditorFieldsDatabase.RenameField(context, attribute.OldName, field.Name);

            if (attribute.Options == EditorFieldOption.Hide) return;

            var editorField = EditorFieldsDatabase.GetEditorField(context, field.Name);
            var value = field.GetValue(target);

            if (attribute.Options.Contains(EditorFieldOption.Deserialize)) Deserialize(attribute, editorField, target, field, ref value);

            if (attribute.Options.Contains(EditorFieldOption.Hide) &&
                field.GetCustomAttribute(typeof(HideAttribute), true) is HideAttribute)
            {
                if (attribute.Options.Contains(EditorFieldOption.Serialize)) Serialize(attribute, editorField, value, field.FieldType);
                return;
            }

            if (attribute.Options.Contains(EditorFieldOption.Hide)) return;

            EditorGUI.BeginChangeCheck();

            if (field.GetCustomAttribute(typeof(CustomDrawerAttribute), true) is CustomDrawerAttribute drawerAttribute &&
                (DrawersDatabase.GetDrawerForAttribute<CustomDrawerPropertyDrawer>(typeof(CustomDrawerAttribute)) is var drawer))
                value = drawer.DrawField(field, context, value, drawerAttribute);
            else
                value = EditorDrawUtility.DrawLayoutField(field, context, target, value,
                    !attribute.Options.Contains(EditorFieldOption.DontFold), attribute.Options.Contains(EditorFieldOption.Asset));

            if (EditorGUI.EndChangeCheck() && attribute.Options.Contains(EditorFieldOption.Serialize))
                Serialize(attribute, editorField, value, field.FieldType);
        }

        public void Serialize(EditorSerializeAttribute attribute, EditorSerializedField editorField, object value, Type fieldType)
        {
            if (attribute.ExecuteInEditMode || !EditorApplication.isPlaying)
                editorField.Serialize(value, fieldType);
        }

        public void Deserialize(EditorSerializeAttribute attribute, EditorSerializedField editorField, object target, FieldInfo field,
            ref object value)
        {
            var fieldType = field.FieldType;
            if (!editorField.DeserializeOverwrite(fieldType, out var jsonValue)) return;

            if (fieldType.IsEnum)
                jsonValue = Enum.ToObject(fieldType, jsonValue);
            if ((typeof(Object).IsAssignableFrom(fieldType) && !(jsonValue as Object)) || value == jsonValue) return;

            value = jsonValue;
            field.SetValue(target, value);
        }

        public override void InitDrawer(BaseMightyMember mightyMember, BaseMightyAttribute mightyAttribute)
        {
        }

        public override void ClearCache()
        {
        }
    }
}
#endif