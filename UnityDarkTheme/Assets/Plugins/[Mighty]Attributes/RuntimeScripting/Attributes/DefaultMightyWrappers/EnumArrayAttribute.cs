using System;

namespace MightyAttributes
{
    [ArraySize("ArraySize"), ItemNames("EnumNames", "Option")]
    public class EnumArrayAttribute : MightyWrapperAttribute
    {
        protected Type EnumType { get; }
        public ArrayOption Option { get; }

        public EnumArrayAttribute(Type enumType, ArrayOption option = ArrayOption.DisableSizeField)
        {
            EnumType = enumType;
            Option = option;
        }

#if UNITY_EDITOR
        private int ArraySize => EnumNames.Length;
        private string[] EnumNames => Enum.GetNames(EnumType);
#endif
    }
}