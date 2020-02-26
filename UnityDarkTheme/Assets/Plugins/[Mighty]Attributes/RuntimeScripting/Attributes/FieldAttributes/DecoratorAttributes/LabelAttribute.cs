namespace MightyAttributes
{
    public class LabelAttribute : BaseDecoratorAttribute
    {
        public string Prefix { get; }

        public string CallbackName { get; }

        public bool PrefixAsCallback { get; }

        public FieldPosition Position { get; }

        public string StyleName { get; } = "EditorStyles.boldLabel";

        public bool EditorStyle { get; }

        public LabelAttribute(string callbackName)
        {
            CallbackName = callbackName;
            Position = FieldPosition.After;
        }

        public LabelAttribute(string prefix, string callbackName)
        {
            Prefix = prefix;
            CallbackName = callbackName;
            Position = FieldPosition.After;
        }

        public LabelAttribute(string prefix, string callbackName, bool prefixAsCallback = false,
            FieldPosition position = FieldPosition.After)
        {
            Prefix = prefix;
            CallbackName = callbackName;
            PrefixAsCallback = prefixAsCallback;
            Position = position;
        }

        public LabelAttribute(string callbackName, FieldPosition position)
        {
            CallbackName = callbackName;
            Position = position;
        }

        public LabelAttribute(string callbackName, string styleName, bool editorStyle)
        {
            CallbackName = callbackName;
            Position = FieldPosition.After;
            StyleName = styleName;
            EditorStyle = editorStyle;
        }

        public LabelAttribute(string callbackName, FieldPosition position, string styleName, bool editorStyle)
        {
            CallbackName = callbackName;
            Position = position;
            StyleName = styleName;
            EditorStyle = editorStyle;
        }
    }
}