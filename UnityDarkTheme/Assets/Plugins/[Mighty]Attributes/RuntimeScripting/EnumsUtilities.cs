using System;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MightyAttributes
{
    public enum InfoBoxType : byte
    {
        Normal,
        Warning,
        Error
    }

    public enum ColorValue : byte
    {
        Default,
        Red,
        Pink,
        Orange,
        Yellow,
        Green,
        Blue,
        Indigo,
        Violet,
        White,
        Brightest,
        Brighter,
        Bright,
        Grey,
        Dark,
        Darker,
        Darkest,
        Black
    }

    [Flags]
    public enum HideStatus : short
    {
        Nothing = 0,
        All = -1,

        Content = 1,
        ScriptField = 1 << 1
    }

    #region Positions

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum Align : byte
    {
        Left,
        Center,
        Right
    }

    [Flags]
    public enum FieldPosition : byte
    {
        Nothing = 0,

        Before = 1,
        After = 1 << 1,

        Horizontal = 1 << 2,
        Anywhere = 1 << 3,

        BeforeHorizontal = Before | Horizontal,
        AfterHorizontal = After | Horizontal
    }

    [Flags]
    public enum DecoratorPosition : short
    {
        Nothing = 0,
        All = -1,

        Before = 1,
        After = 1 << 1,

        BeforeElements = 1 << 2,
        BetweenElements = 1 << 3,
        AfterElements = 1 << 4,

        BeforeHeader = 1 << 5,
        AfterHeader = 1 << 6,

        Anywhere = 1 << 7,

        BeforeAnywhere = Before | Anywhere,
        AfterAnywhere = After | Anywhere,

        Wrap = Before | After,
        WrapAnywhere = Wrap | Anywhere,
        WrapElement = BeforeElements | AfterElements,
        WrapHeader = BeforeHeader | AfterHeader,
        WrapArray = WrapElement | WrapHeader
    }

    #endregion /Positions

    #region Options

    [Flags]
    public enum FieldOption : short
    {
        Nothing = 0,
        All = -1,

        HideLabel = 1,
        BoldLabel = 1 << 1
    }

    [Flags]
    public enum ArrayOption : short
    {
        Nothing = 0,
        All = -1,

        HideLabel = FieldOption.HideLabel,
        BoldLabel = FieldOption.BoldLabel,
        HideSizeField = 1 << 2,
        DisableSizeField = 1 << 3,
        DontIndent = 1 << 4,
        HideElementLabel = 1 << 5,
        LabelInHeader = 1 << 6,
        ReadOnly = 1 << 7,

        ContentOnly = HideLabel | HideSizeField | DontIndent
    }

    [Flags]
    public enum NestOption : short
    {
        Nothing = 0,
        All = -1,

        HideLabel = FieldOption.HideLabel,
        BoldLabel = FieldOption.BoldLabel,
        DontFold = 1 << 2,
        DontIndent = 1 << 3,

        ContentOnly = DontFold | DontIndent | HideLabel
    }

    [Flags]
    public enum EditorFieldOption : short
    {
        Nothing = 0,
        All = -1,

        HideLabel = FieldOption.HideLabel,
        BoldLabel = FieldOption.BoldLabel,
        DontFold = NestOption.DontFold,

        Deserialize = 1 << 3,
        Serialize = 1 << 4,

        Hide = 1 << 5,
        Asset = 1 << 6,

        HideDeserialize = Deserialize | Hide,
        HideDontFold = Hide | DontFold,
        HideAsset = Hide | Asset,

        Default = Deserialize | Serialize,
        HideDefault = Hide | Default,
        AssetDefault = Asset | Default
    }

    [Flags]
    public enum StyleOption : short
    {
        Nothing = 0,
        All = -1,

        SpaceBefore = 1,
        SpaceAfter = 1 << 1,

        Default = SpaceBefore | SpaceAfter
    }

    #endregion /Options

    public static class EnumsUtilities
    {
        public static bool Contains(this FieldPosition position, FieldPosition flag) => (position & flag) != 0;
        public static bool ContainsExact(this FieldPosition position, FieldPosition flag) => (position & flag) == flag;

        public static bool Contains(this DecoratorPosition position, DecoratorPosition flag) => (position & flag) != 0;
        public static bool ContainsExact(this DecoratorPosition position, DecoratorPosition flag) => (position & flag) == flag;

        public static bool Contains(this HideStatus hideStatus, HideStatus flag) => (hideStatus & flag) != 0;
        public static bool ContainsExact(this HideStatus hideStatus, HideStatus flag) => (hideStatus & flag) == flag;

        public static bool Contains(this FieldOption option, FieldOption flag) => (option & flag) != 0;
        public static bool ContainsExact(this FieldOption option, FieldOption flag) => (option & flag) == flag;

        public static bool Contains(this ArrayOption option, ArrayOption flag) => (option & flag) != 0;
        public static bool ContainsExact(this ArrayOption option, ArrayOption flag) => (option & flag) == flag;

        public static bool Contains(this NestOption option, NestOption flag) => (option & flag) != 0;
        public static bool ContainsExact(this NestOption option, NestOption flag) => (option & flag) == flag;

        public static bool Contains(this EditorFieldOption option, EditorFieldOption flag) => (option & flag) != 0;
        public static bool ContainsExact(this EditorFieldOption option, EditorFieldOption flag) => (option & flag) == flag;

        public static bool Contains(this StyleOption option, StyleOption flag) => (option & flag) != 0;
        public static bool ContainsExact(this StyleOption option, StyleOption flag) => (option & flag) == flag;


#if UNITY_EDITOR

        #region Build Target

        public static BuildTargetGroup ToBuildTargetGroup(this BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.PS4:
                    return BuildTargetGroup.PS4;
                case BuildTarget.XboxOne:
                    return BuildTargetGroup.XboxOne;
                case BuildTarget.tvOS:
                    return BuildTargetGroup.tvOS;
                case BuildTarget.Switch:
                    return BuildTargetGroup.Switch;
                case BuildTarget.Lumin:
                    return BuildTargetGroup.Lumin;
                default:
                    return BuildTargetGroup.Unknown;
            }
        }

        public static BuildTarget[] ToBuildTargets(this BuildTargetGroup targetGroup)
        {
            switch (targetGroup)
            {
                case BuildTargetGroup.Standalone:
                    return new[]
                    {
                        BuildTarget.StandaloneOSX, BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64,
                        BuildTarget.StandaloneLinux64
                    };
                case BuildTargetGroup.Android:
                    return new[] {BuildTarget.Android};
                case BuildTargetGroup.WebGL:
                    return new[] {BuildTarget.WebGL};
                case BuildTargetGroup.WSA:
                    return new[] {BuildTarget.WSAPlayer};
                case BuildTargetGroup.PS4:
                    return new[] {BuildTarget.PS4};
                case BuildTargetGroup.XboxOne:
                    return new[] {BuildTarget.XboxOne};
                case BuildTargetGroup.tvOS:
                    return new[] {BuildTarget.tvOS};
                case BuildTargetGroup.Switch:
                    return new[] {BuildTarget.Switch};
                case BuildTargetGroup.Lumin:
                    return new[] {BuildTarget.Lumin};
                default:
                    return new[] {BuildTarget.NoTarget};
            }
        }

        #endregion /Build Target

#endif
    }
}