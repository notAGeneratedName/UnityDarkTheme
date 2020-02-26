namespace MightyAttributes
{
    public class AlignAttribute : BaseDecoratorAttribute
    {
        public Align Align { get; private set; }

        public AlignAttribute(Align align)
        {
            Align = align;
        }
    }
}