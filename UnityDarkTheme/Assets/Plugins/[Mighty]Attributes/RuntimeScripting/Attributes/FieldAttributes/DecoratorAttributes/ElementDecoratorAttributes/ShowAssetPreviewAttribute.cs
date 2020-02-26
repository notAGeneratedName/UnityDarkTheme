namespace MightyAttributes
{
    public class AssetPreviewAttribute : ShowAssetPreviewAttribute
    {
        public AssetPreviewAttribute(int size = 64, Align align = Align.Right) : base(size, align)
        {
        }

        public AssetPreviewAttribute(Align align) : base(align)
        {
        }
    }

    public class ShowAssetPreviewAttribute : BaseElementDecoratorAttribute
    {
        public int Size { get; }
        public Align Align { get; }

        public ShowAssetPreviewAttribute(int size = 64, Align align = Align.Right)
        {
            this.Size = size;
            Align = align;
        }

        public ShowAssetPreviewAttribute(Align align)
        {
            this.Size = 64;
            Align = align;
        }
    }
}