namespace MightyAttributes
{
    public class StyleAttribute : BaseDecoratorAttribute
    {
        public string StyleName { get; }
        
        public bool EditorStyle { get; }
        
        public bool Indent { get; }
        
        public StyleAttribute(string styleName, bool indent = false, bool editorStyle = false)
        {
            StyleName = styleName;
            EditorStyle = editorStyle;
            Indent = indent;
        }     
    }
}