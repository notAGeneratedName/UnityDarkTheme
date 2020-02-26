using MightyAttributes;

public class HeaderAttribute : BaseDecoratorAttribute
{
    public string Header { get; }

    public HeaderAttribute(string header) => Header = header;
}
