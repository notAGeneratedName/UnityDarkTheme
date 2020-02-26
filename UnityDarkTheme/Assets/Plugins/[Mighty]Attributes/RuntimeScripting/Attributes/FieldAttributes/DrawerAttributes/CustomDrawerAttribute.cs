namespace MightyAttributes
{
    public class CustomDrawerAttribute : BaseDrawerAttribute
    {
        public string DrawerCallback { get; }

        public string ElementHeightCallback { get; }

        public CustomDrawerAttribute(string drawerCallback, string elementHeightCallback = null, FieldOption option = FieldOption.Nothing) :
            base(option)
        {
            DrawerCallback = drawerCallback;
            ElementHeightCallback = elementHeightCallback;
        }
    }
}