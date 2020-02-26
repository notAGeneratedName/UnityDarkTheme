namespace MightyAttributes
{
    public class EditorSerializeAttribute : BaseDrawerAttribute
    {
        public EditorFieldOption Options { get; }

        public string OldName { get; }

        public bool ExecuteInEditMode { get; }

        public EditorSerializeAttribute(bool executeInEditMode) : base(FieldOption.Nothing)
        {
            Options = EditorFieldOption.Default;
            ExecuteInEditMode = executeInEditMode;
        }

        public EditorSerializeAttribute(string oldName = null, bool executeInEditMode = false) : base(FieldOption.Nothing)
        {
            Options = EditorFieldOption.Default;
            OldName = oldName;
            ExecuteInEditMode = executeInEditMode;
        }

        public EditorSerializeAttribute(EditorFieldOption options, string oldName = null, bool executeInEditMode = false) : base(
            FieldOption.Nothing)
        {
            Options = options;
            OldName = oldName;
            ExecuteInEditMode = executeInEditMode;
        }

        public EditorSerializeAttribute(EditorFieldOption options, bool executeInEditMode) : base(FieldOption.Nothing)
        {
            Options = options;
            ExecuteInEditMode = executeInEditMode;
        }
    }
}