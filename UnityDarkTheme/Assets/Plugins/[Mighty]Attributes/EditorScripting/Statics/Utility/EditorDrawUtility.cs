#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public static class IconName
    {
        public const string EYE = "ViewToolOrbit";
        public const string CS_SCRIPT_ICON = "cs Script Icon";
        public const string REFRESH = "TreeEditor.Refresh";
        public const string SAVE = "SaveActive";
        public const string SETTINGS = "Settings";
        public const string TRASH = "TreeEditor.Trash";
        public const string PLUS = "Toolbar Plus";
        public const string MINUS = "Toolbar Minus";
        public const string RECORD = "Animation.Record";
        public const string PLAY = "Animation.Play";
        public const string LOCK = "LockIcon-On";
        public const string UNLOCK = "LockIcon";
        public const string STAR_LIST = "CustomSorting";

        #region Loading

        public const string LOADING_0 = "WaitSpin00";
        public const string LOADING_1 = "WaitSpin01";
        public const string LOADING_2 = "WaitSpin02";
        public const string LOADING_3 = "WaitSpin03";
        public const string LOADING_4 = "WaitSpin04";
        public const string LOADING_5 = "WaitSpin05";
        public const string LOADING_6 = "WaitSpin06";
        public const string LOADING_7 = "WaitSpin07";
        public const string LOADING_8 = "WaitSpin08";
        public const string LOADING_9 = "WaitSpin09";
        public const string LOADING_10 = "WaitSpin10";
        public const string LOADING_11 = "WaitSpin11";

        public static string Loading(int index)
        {
            switch (index)
            {
                case 0: return LOADING_0;
                case 1: return LOADING_1;
                case 2: return LOADING_2;
                case 3: return LOADING_3;
                case 4: return LOADING_4;
                case 5: return LOADING_5;
                case 6: return LOADING_6;
                case 7: return LOADING_7;
                case 8: return LOADING_8;
                case 9: return LOADING_9;
                case 10: return LOADING_10;
                case 11: return LOADING_11;
            }

            return LOADING_0;
        }

        public static void NextLoadingIndex(ref int index)
        {
            if (index < 0 || index > 11) index = -1;
            ++index;
        }

        public static string NextLoadingIcon(ref int index)
        {
            NextLoadingIndex(ref index);
            return Loading(index);
        }

        #endregion /Loading
    }

    public static class EditorDrawUtility
    {
        public const float CHAR_SIZE = 8;
        public const float END_SPACE = 5;

        public static float TextWidth(string text) => text.Length * CHAR_SIZE + END_SPACE;

        public const int TAB_SIZE = 15;

        public static int IndentSpace => DoIndent(EditorGUI.indentLevel);

        public static int DoIndent(int indentLevel) => TAB_SIZE * indentLevel;

        public static void IndentOnce()
        {
            GUILayout.BeginHorizontal();
            DoIndent(1);
            GUILayout.EndHorizontal();
        }

        private static MethodInfo m_boldFontMethodInfo;

        private static bool m_guiIndented;

        private static readonly Dictionary<string, bool> FoldoutByName;

        static EditorDrawUtility()
        {
            FoldoutByName = new Dictionary<string, bool>();
        }

        #region Foldout

        public static bool DrawFoldout(SerializedProperty property, GUIStyle style = null) =>
            property.isExpanded = DrawFoldout(property.isExpanded, property.displayName, style);

        public static bool DrawFoldout(SerializedProperty property, GUIContent label, GUIStyle style = null) =>
            property.isExpanded = DrawFoldout(property.isExpanded, label, style);

        public static bool DrawFoldout(bool foldout, string label, GUIStyle style = null)
        {
            var changed = GUI.changed;
            foldout = EditorGUILayout.Foldout(foldout, label, true, style ?? EditorStyles.foldout);
            GUI.changed = changed;
            return foldout;
        }

        public static bool DrawFoldout(bool foldout, GUIContent label, GUIStyle style = null)
        {
            var changed = GUI.changed;
            foldout = EditorGUILayout.Foldout(foldout, label, true, style ?? EditorStyles.foldout);
            GUI.changed = changed;
            return foldout;
        }

        public static bool DrawFoldout(Rect rect, SerializedProperty property, GUIStyle style = null) =>
            property.isExpanded = DrawFoldout(rect, property.isExpanded, property.displayName, style);

        public static bool DrawFoldout(Rect rect, SerializedProperty property, GUIContent label, GUIStyle style = null) =>
            property.isExpanded = DrawFoldout(rect, property.isExpanded, label, style);

        public static bool DrawFoldout(Rect rect, bool foldout, string label, GUIStyle style = null)
        {
            var changed = GUI.changed;
            foldout = EditorGUI.Foldout(rect, foldout, label, true, style ?? EditorStyles.foldout);
            GUI.changed = changed;
            return foldout;
        }

        public static bool DrawFoldout(Rect rect, bool foldout, GUIContent label, GUIStyle style = null)
        {
            var changed = GUI.changed;
            foldout = EditorGUI.Foldout(rect, foldout, label, true, style ?? EditorStyles.foldout);
            GUI.changed = changed;
            return foldout;
        }


        public static void SetFoldout(string name, bool foldout)
        {
            if (!FoldoutByName.ContainsKey(name)) FoldoutByName.Add(name, foldout);
            else FoldoutByName[name] = foldout;
        }

        public static bool GetFoldout(string name)
        {
            if (!FoldoutByName.ContainsKey(name)) FoldoutByName.Add(name, false);
            return FoldoutByName[name];
        }

        #endregion /Foldout

        #region Name

        public static string DrawPrettyName(this string name, bool removeInterfacePrefix = false)
        {
            name = name.Replace("m_", "").Replace("_", "");
            name = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
            name = Regex.Replace(name, "([a-z])([0-9])", "$1 $2");

            if (removeInterfacePrefix && name[0] == 'I')
                name = name.Remove(0, 1);

            return name.FirstCharUpper();
        }

        public static string FirstCharUpper(this string value)
        {
            var array = value.ToCharArray();
            array[0] = char.ToUpper(value[0]);
            return new string(array);
        }

        #endregion /Name

        #region Header

        public static void DrawHeader(string header)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
        }

        public static void DrawHeader(Rect rect, string header)
        {
            EditorGUI.IndentedRect(rect);
            EditorGUI.LabelField(rect, header, EditorStyles.boldLabel);
        }

        public static bool DrawHeader(SerializedProperty property)
        {
            var headerAttr = property.GetAttribute<HeaderAttribute>();
            if (headerAttr != null)
            {
                DrawHeader(headerAttr.Header);
                return true;
            }

            return false;
        }

        public static bool DrawHeader(Rect rect, SerializedProperty property)
        {
            var headerAttr = property.GetAttribute<HeaderAttribute>();
            if (headerAttr != null)
            {
                DrawHeader(rect, headerAttr.Header);
                return true;
            }

            return false;
        }

        #endregion /Header

        #region HelpBox

        public static void DrawHelpBox(string message, MessageType type = MessageType.Warning, Object context = null,
            bool logToConsole = false)
        {
            EditorGUILayout.HelpBox(message, type);

            message = $"[Mighty]Attributes - {message}";

            if (logToConsole) MightyDebug(message, type, context);
        }

        public static void DrawHelpBox(Rect rect, string message, MessageType type = MessageType.Warning, Object context = null,
            bool logToConsole = false)
        {
            EditorGUI.HelpBox(rect, message, type);

            if (logToConsole) MightyDebug(message, type, context);
        }

        public static void MightyDebug(string message, MessageType type = MessageType.Info, Object context = null)
        {
            message = $"[Mighty]Attributes - {message}";

            switch (type)
            {
                case MessageType.None:
                case MessageType.Info:
                    Debug.Log(message, context);
                    break;
                case MessageType.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case MessageType.Error:
                    Debug.LogError(message, context);
                    break;
            }
        }

        #endregion /HelpBox

        #region Style

        public static GUIStyle GetStyle(SerializedProperty property, object target, string styleName, bool editorStyle)
        {
            if (string.IsNullOrEmpty(styleName) || !property.GetValueFromMember(target, styleName, out var guiStyle,
                    (string name, out GUIStyle value) => GetStyle(name, out value, editorStyle))) return null;

            return guiStyle;
        }

        public static GUIStyle GetStyle(object target, string styleName, bool editorStyle)
        {
            if (string.IsNullOrEmpty(styleName) || !target.GetValueFromMember(styleName, out var guiStyle,
                    (string name, out GUIStyle value) => GetStyle(name, out value, editorStyle))) return null;

            return guiStyle;
        }

        public static MightyInfo<GUIStyle> GetStyleInfo(SerializedProperty property, object target, string styleName, bool editorStyle)
        {
            if (string.IsNullOrEmpty(styleName) || !property.GetInfoFromMember(target, styleName, out var styleInfo,
                    (string name, out GUIStyle value) => GetStyle(name, out value, editorStyle))) return null;

            return styleInfo;
        }

        public static MightyInfo<GUIStyle> GetStyleInfo(object target, string styleName, bool editorStyle)
        {
            if (string.IsNullOrEmpty(styleName) || !target.GetInfoFromMember(styleName, out var styleInfo,
                    (string name, out GUIStyle value) => GetStyle(name, out value, editorStyle))) return null;

            return styleInfo;
        }

        private static bool GetStyle(string styleName, out GUIStyle guiStyle, bool editorStyle)
        {
            var namePath = styleName.Split('.');
            if (namePath.Length > 1)
            {
                switch (namePath[0])
                {
                    case "EditorStyles":
                        editorStyle = true;
                        break;
                    case "GUI":
                    case "GUISkin":
                        editorStyle = false;
                        break;
                }
            }

            if (editorStyle)
            {
                foreach (var styleProperty in GetAllStylesProperties<EditorStyles>())
                {
                    if (styleProperty.Name != styleName && $"EditorStyles.{styleProperty.Name}" != styleName) continue;
                    try
                    {
                        guiStyle = styleProperty.GetValue(null) as GUIStyle;
                        return true;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            else
            {
                foreach (var styleProperty in GetAllStylesProperties<GUISkin>())
                {
                    if (styleProperty.Name != styleName && $"GUI.skin.{styleProperty.Name}" != styleName &&
                        $"GUISkin.{styleProperty.Name}" != styleName) continue;
                    try
                    {
                        guiStyle = styleProperty.GetValue(GUI.skin) as GUIStyle;
                        return true;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            foreach (var style in editorStyle ? GetAllEditorStyles() ?? GUI.skin.customStyles : GUI.skin.customStyles)
                if (style.name == styleName)
                {
                    guiStyle = style;
                    return true;
                }

            guiStyle = null;
            return false;
        }

        public static PropertyInfo[] GetAllStylesProperties<T>() =>
            typeof(T).GetProperties().Where(x => x.PropertyType == typeof(GUIStyle)).ToArray();

        public static GUIStyle[] GetAllEditorStyles()
        {
            try
            {
                return new[]
                {
                    EditorStyles.label, EditorStyles.whiteLabel,
                    EditorStyles.miniLabel, EditorStyles.whiteMiniLabel,
                    EditorStyles.boldLabel, EditorStyles.whiteBoldLabel, EditorStyles.miniBoldLabel,
                    EditorStyles.largeLabel, EditorStyles.whiteLargeLabel,
                    EditorStyles.linkLabel,
                    EditorStyles.foldout, EditorStyles.foldoutHeader,
                    EditorStyles.toggle,
                    EditorStyles.colorField,
                    EditorStyles.popup,
                    EditorStyles.toolbar,
                    EditorStyles.helpBox,
                    EditorStyles.miniButton,
                    EditorStyles.numberField,
                    EditorStyles.objectField,
                    EditorStyles.radioButton,
                    EditorStyles.textArea,
                    EditorStyles.textField,
                    EditorStyles.toggleGroup,
                    EditorStyles.toolbarButton,
                    EditorStyles.toolbarPopup,
                    EditorStyles.foldoutHeaderIcon,
                    EditorStyles.foldoutPreDrop,
                    EditorStyles.inspectorDefaultMargins,
                    EditorStyles.layerMaskField,
                    EditorStyles.miniButtonLeft,
                    EditorStyles.miniButtonMid,
                    EditorStyles.miniButtonRight,
                    EditorStyles.miniPullDown,
                    EditorStyles.miniTextField,
                    EditorStyles.toolbarDropDown,
                    EditorStyles.toolbarSearchField,
                    EditorStyles.toolbarTextField,
                    EditorStyles.wordWrappedLabel,
                    EditorStyles.centeredGreyMiniLabel,
                    EditorStyles.inspectorFullWidthMargins,
                    EditorStyles.objectFieldThumb,
                    EditorStyles.wordWrappedMiniLabel,
                };
            }
            catch
            {
                return new GUIStyle[0];
            }
        }

        public static void SetBoldDefaultFont(bool value)
        {
            if (m_boldFontMethodInfo == null)
                m_boldFontMethodInfo =
                    typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
            m_boldFontMethodInfo.Invoke(null, new object[] {value});
        }

        #endregion /Style

        #region Color

        public static Color GetColor(SerializedProperty property, object target, string colorName, ColorValue colorValue,
            Color defaultColor) => GetColor(property, target, colorName, GetColor(colorValue, defaultColor));

        public static Color? GetColor(SerializedProperty property, object target, string colorName, ColorValue colorValue) =>
            GetColor(property, target, colorName, GetColor(colorValue, null));

        public static MightyInfo<Color?> GetColorInfo(SerializedProperty property, object target, string colorName, ColorValue colorValue)
            => GetColorInfo(property, target, colorName, GetColor(colorValue, null));

        public static MightyInfo<Color?> GetColorInfo(object target, string colorName, ColorValue colorValue)
            => GetColorInfo(target, colorName, GetColor(colorValue, null));

        public static Color GetColor(SerializedProperty property, object target, string colorName, Color defaultColor) =>
            property.GetValueFromMember(target, colorName, out Color color, ColorUtility.TryParseHtmlString) ? color : defaultColor;

        public static Color? GetColor(SerializedProperty property, object target, string colorName, Color? defaultColor) =>
            property.GetValueFromMember(target, colorName, out Color color, ColorUtility.TryParseHtmlString) ? color : defaultColor;

        public static MightyInfo<Color?> GetColorInfo(SerializedProperty property, object target, string colorName, Color? defaultColor) =>
            property.GetInfoFromMember(target, colorName, out var memberInfo,
                (string s, out Color? value) =>
                    (value = ColorUtility.TryParseHtmlString(s, out var colorValue) ? colorValue : (Color?) null) != null)
                ? memberInfo
                : new MightyInfo<Color?>(null, null, defaultColor);

        public static MightyInfo<Color?> GetColorInfo(object target, string colorName, Color? defaultColor) =>
            target.GetInfoFromMember(colorName, out var memberInfo,
                (string s, out Color? value) =>
                    (value = ColorUtility.TryParseHtmlString(s, out var colorValue) ? colorValue : (Color?) null) != null)
                ? memberInfo
                : new MightyInfo<Color?>(null, null, defaultColor);

        public static Color GetColor(ColorValue color) => GetColor(color, Color.white);

        public static Color GetColor(ColorValue color, Color defaultColor)
        {
            switch (color)
            {
                case ColorValue.Red:
                    return new Color32(255, 0, 63, 255);
                case ColorValue.Pink:
                    return new Color32(255, 152, 203, 255);
                case ColorValue.Orange:
                    return new Color32(255, 128, 0, 255);
                case ColorValue.Yellow:
                    return new Color32(255, 211, 0, 255);
                case ColorValue.Green:
                    return new Color32(102, 255, 0, 255);
                case ColorValue.Blue:
                    return new Color32(0, 135, 189, 255);
                case ColorValue.Indigo:
                    return new Color32(75, 0, 130, 255);
                case ColorValue.Violet:
                    return new Color32(127, 0, 255, 255);
                case ColorValue.White:
                    return Color.white;
                case ColorValue.Brightest:
                    return new Color32(210, 210, 210, 255);
                case ColorValue.Brighter:
                    return new Color32(180, 180, 180, 255);
                case ColorValue.Bright:
                    return new Color32(145, 145, 145, 255);
                case ColorValue.Grey:
                    return Color.grey;
                case ColorValue.Dark:
                    return new Color32(70, 70, 70, 255);
                case ColorValue.Darker:
                    return new Color32(41, 41, 41, 255);
                case ColorValue.Darkest:
                    return new Color32(20, 20, 20, 255);
                case ColorValue.Black:
                    return Color.black;
                default:
                    return defaultColor;
            }
        }

        public static Color? GetColor(ColorValue color, Color? defaultColor)
        {
            switch (color)
            {
                case ColorValue.Red:
                    return new Color32(255, 0, 63, 255);
                case ColorValue.Pink:
                    return new Color32(255, 152, 203, 255);
                case ColorValue.Orange:
                    return new Color32(255, 128, 0, 255);
                case ColorValue.Yellow:
                    return new Color32(255, 211, 0, 255);
                case ColorValue.Green:
                    return new Color32(102, 255, 0, 255);
                case ColorValue.Blue:
                    return new Color32(0, 135, 189, 255);
                case ColorValue.Indigo:
                    return new Color32(75, 0, 130, 255);
                case ColorValue.Violet:
                    return new Color32(127, 0, 255, 255);
                case ColorValue.White:
                    return Color.white;
                case ColorValue.Brightest:
                    return new Color32(210, 210, 210, 255);
                case ColorValue.Brighter:
                    return new Color32(180, 180, 180, 255);
                case ColorValue.Bright:
                    return new Color32(145, 145, 145, 255);
                case ColorValue.Grey:
                    return Color.grey;
                case ColorValue.Dark:
                    return new Color32(70, 70, 70, 255);
                case ColorValue.Darker:
                    return new Color32(41, 41, 41, 255);
                case ColorValue.Darkest:
                    return new Color32(20, 20, 20, 255);
                case ColorValue.Black:
                    return Color.black;
                default:
                    return defaultColor;
            }
        }

        #endregion /Color

        #region Align

        public static void DrawWithAlign(Align align, Action drawCallback)
        {
            GUILayout.BeginHorizontal();
            switch (align)
            {
                case Align.Left:
                    GUILayout.Space(EditorGUI.indentLevel * 20);
                    drawCallback.Invoke();
                    break;
                case Align.Center:
                    GUILayout.FlexibleSpace();
                    drawCallback.Invoke();
                    GUILayout.FlexibleSpace();
                    break;
                case Align.Right:
                    GUILayout.FlexibleSpace();
                    drawCallback.Invoke();
                    break;
            }

            GUILayout.EndHorizontal();
        }

        public static void BeginDrawAlign(Align align)
        {
            GUILayout.BeginHorizontal();
            switch (align)
            {
                case Align.Left:
                    GUILayout.Space(EditorGUI.indentLevel * 20);
                    break;
                case Align.Center:
                    GUILayout.FlexibleSpace();
                    break;
                case Align.Right:
                    GUILayout.FlexibleSpace();
                    break;
            }
        }

        public static void EndDrawAlign(Align align)
        {
            if (align == Align.Center)
                GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        #endregion /Align

        #region Layout

        public static void LayoutIndent(int indent = 1) => GUILayoutUtility.GetRect(TAB_SIZE * indent, 0);

        public static void DrawLayoutIndent() => GUILayout.Space(IndentSpace);

        public static void BeginLayoutIndent()
        {
            EditorGUILayout.BeginHorizontal();
            DrawLayoutIndent();
        }

        public static void EndLayoutIndent() => EditorGUILayout.EndHorizontal();

        #endregion /Layout

        #region Array

        public delegate void ElementDrawer(int index);

        public static void DrawArraySizeField(SerializedProperty property)
        {
            if (!property.isArray || !property.isExpanded) return;

            EditorGUI.BeginChangeCheck();
            property.arraySize = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", property.arraySize));
            if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();
        }

        public static void DrawArraySizeField(Rect rect, SerializedProperty property)
        {
            if (!property.isArray || !property.isExpanded) return;

            EditorGUI.BeginChangeCheck();
            property.arraySize = Mathf.Max(0, EditorGUI.DelayedIntField(rect, "Size", property.arraySize));
            if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();
        }

        public static void DrawArrayBody(SerializedProperty property, ElementDrawer elementDrawer)
        {
            if (!property.isArray || !property.isExpanded) return;

            for (var i = 0; i < property.arraySize; i++)
                elementDrawer(i);
        }

        public static void DrawArrayBody(Rect rect, SerializedProperty property, ElementDrawer elementDrawer)
        {
            if (!property.isArray || !property.isExpanded) return;

            for (var i = 0; i < property.arraySize; i++)
                elementDrawer(i);
        }

        public static void DrawArray(SerializedProperty property, ElementDrawer elementDrawer)
        {
            if (!property.isArray) return;

            if (!DrawFoldout(property)) return;

            EditorGUI.indentLevel++;
            DrawArraySizeField(property);
            DrawArrayBody(property, elementDrawer);
            EditorGUI.indentLevel--;
        }

        public static void DrawArray(Rect rect, SerializedProperty property, ElementDrawer elementDrawer, bool fromBox)
        {
            if (!property.isArray) return;

            if (!DrawFoldout(rect, property)) return;

            rect.x += 15;
            rect.width -= 15;
            DrawArraySizeField(rect, property);
            DrawArrayBody(rect, property, elementDrawer);
        }

        #endregion /Array

        #region Buttons

        public static bool DrawAddButton() => GUILayout.Button(DrawIcon(IconName.PLUS), GUILayout.Height(25), GUILayout.Width(25));
        public static bool DrawRemoveButton() => GUILayout.Button(DrawIcon(IconName.MINUS), GUILayout.Height(25), GUILayout.Width(25));

        #endregion /Buttons

        #region Icon

        public static Texture2D GetTexture(string texturePath) => 
            AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;

        public static GUIContent DrawIconLabel(string iconName, string label) => new GUIContent(DrawIcon(iconName)) {text = $" {label}"};

        public static GUIContent DrawIcon(string iconName) => EditorGUIUtility.IconContent(iconName);

        #endregion /Icon

        #region Fields

        public static void DrawPrefabSensitiveField(this SerializedProperty property, Rect rect, Action<GUIContent> fieldDrawer)
        {
            var label = EditorGUI.BeginProperty(rect, new GUIContent(property.displayName), property);
            fieldDrawer.Invoke(label);
            EditorGUI.EndProperty();
        }

        public static void DrawPropertyField(SerializedProperty property, GUIContent label = null, bool includeChildren = true)
        {
            if (label != null)
                EditorGUILayout.PropertyField(property, label, includeChildren);
            else
            {
                var name = property.name;
                EditorGUILayout.PropertyField(property, includeChildren);
            }
        }

        public static void DrawPropertyField(Rect rect, SerializedProperty property, bool includeChildren = true)
        {
            EditorGUI.PropertyField(rect, property, includeChildren);
        }

        public static bool DrawLayoutField(string label, object value, bool enabled)
        {
            GUI.enabled = enabled;

            var isDrawn = true;
            var valueType = value.GetType();

            if (valueType.IsArray)
                return DrawArrayLayoutField(label, value, valueType.GetElementType(), enabled);

            if (valueType.IsEnum)
            {
                if (valueType.GetCustomAttribute<FlagsAttribute>() != null)
                    EditorGUILayout.MaskField(label, Convert.ToInt32(value), Enum.GetNames(valueType));
                else
                    EditorGUILayout.EnumPopup(label, (Enum) value);
            }
            else if (valueType == typeof(bool))
                EditorGUILayout.Toggle(label, (bool) value);
            else if (valueType == typeof(int) || valueType == typeof(short) || valueType == typeof(byte))
                EditorGUILayout.IntField(label, (int) value);
            else if (valueType == typeof(long))
                EditorGUILayout.LongField(label, (long) value);
            else if (valueType == typeof(float))
                EditorGUILayout.FloatField(label, (float) value);
            else if (valueType == typeof(double))
                EditorGUILayout.DoubleField(label, (double) value);
            else if (valueType == typeof(string))
                EditorGUILayout.TextField(label, (string) value);
            else if (valueType == typeof(Vector2))
                EditorGUILayout.Vector2Field(label, (Vector2) value);
            else if (valueType == typeof(Vector3))
                EditorGUILayout.Vector3Field(label, (Vector3) value);
            else if (valueType == typeof(Vector4))
                EditorGUILayout.Vector4Field(label, (Vector4) value);
            else if (valueType == typeof(Vector2Int))
                EditorGUILayout.Vector2IntField(label, (Vector2Int) value);
            else if (valueType == typeof(Vector3Int))
                EditorGUILayout.Vector3IntField(label, (Vector3Int) value);
            else if (valueType == typeof(Rect))
                EditorGUILayout.RectField(label, (Rect) value);
            else if (valueType == typeof(Bounds))
                EditorGUILayout.BoundsField(label, (Bounds) value);
            else if (valueType == typeof(RectInt))
                EditorGUILayout.RectIntField(label, (RectInt) value);
            else if (valueType == typeof(BoundsInt))
                EditorGUILayout.BoundsIntField(label, (BoundsInt) value);
            else if (valueType == typeof(Quaternion))
                Quaternion.Euler(EditorGUILayout.Vector3Field(label, ((Quaternion) value).eulerAngles));
            else if (valueType == typeof(LayerMask))
                EditorGUILayout.LayerField(label, (LayerMask) value);
            else if (valueType == typeof(Color))
                EditorGUILayout.ColorField(label, (Color) value);
            else if (valueType == typeof(AnimationCurve))
                EditorGUILayout.CurveField(label, (AnimationCurve) value);
            else if (valueType == typeof(Gradient))
                EditorGUILayout.GradientField(label, (Gradient) value);
            else if (typeof(Object).IsAssignableFrom(valueType))
                EditorGUILayout.ObjectField(label, (Object) value, valueType, true);
            else if (valueType.GetCustomAttribute(typeof(SerializableAttribute), true) != null)
            {
                if (!EditorGUILayout.Foldout(GetFoldout(label), label, true))
                {
                    SetFoldout(label, false);
                    return true;
                }

                SetFoldout(label, true);
                EditorGUI.indentLevel++;
                foreach (var field in valueType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.IsPublic || f.GetCustomAttribute(typeof(SerializeField)) != null))
                {
                    DrawLayoutField(field.Name.DrawPrettyName(), field.GetValue(value), enabled);
                }

                EditorGUI.indentLevel--;
            }
            else
                isDrawn = false;

            GUI.enabled = true;

            return isDrawn;
        }

        public static object DrawLayoutField(string fieldName, Object context, object target, bool foldProperty = true) =>
            DrawLayoutField(ReflectionUtility.GetField(target.GetType(), fieldName), context, target, foldProperty);

        public static object DrawLayoutField(FieldInfo field, Object context, object target, bool foldProperty = true) =>
            field == null ? null : DrawLayoutField(field, context, target, field.GetValue(context), foldProperty);

        public static object DrawLayoutField(FieldInfo field, Object context, object target, object value, bool foldProperty = true,
            bool isAsset = false)
        {
            value = DrawLayoutField(context, field.Name, field.FieldType, field.Name.DrawPrettyName(), value, foldProperty,
                isAsset);
            field.SetValue(target, value);

            return value;
        }

        public static T DrawGenericLayoutField<T>(string label, T value, bool foldProperty = true, bool isAsset = false) =>
            (T) DrawLayoutField(null, label, typeof(T), label, value, foldProperty, isAsset);

        public static object DrawLayoutField(Object context, string fieldName, Type type, string label, object value,
            bool foldProperty = true, bool isAsset = false)
        {
            if (typeof(IList).IsAssignableFrom(type))
                return DrawArrayLayoutField(context, fieldName, type.GetElementType(), label, value);

            if (type.IsEnum || type == typeof(Enum))
            {
                if (type.GetCustomAttribute<FlagsAttribute>() != null)
                    return EditorGUILayout.MaskField(label, Convert.ToInt32(value), Enum.GetNames(type));
                return EditorGUILayout.EnumPopup(label, (Enum) Enum.ToObject(type, (int) value));
            }

            if (type == typeof(bool))
                return EditorGUILayout.Toggle(label, (bool) value);
            if (type == typeof(int) || type == typeof(short) || type == typeof(byte))
                return EditorGUILayout.IntField(label, (int) value);
            if (type == typeof(long))
                return EditorGUILayout.LongField(label, (long) value);
            if (type == typeof(float))
                return EditorGUILayout.FloatField(label, (float) value);
            if (type == typeof(double))
                return EditorGUILayout.DoubleField(label, (double) value);
            if (type == typeof(string))
                return EditorGUILayout.TextField(label, (string) value);
            if (type == typeof(Vector2))
                return EditorGUILayout.Vector2Field(label, (Vector2) value);
            if (type == typeof(Vector3))
                return EditorGUILayout.Vector3Field(label, (Vector3) value);
            if (type == typeof(Vector4))
                return EditorGUILayout.Vector4Field(label, (Vector4) value);
            if (type == typeof(Vector2Int))
                return EditorGUILayout.Vector2IntField(label, (Vector2Int) value);
            if (type == typeof(Vector3Int))
                return EditorGUILayout.Vector3IntField(label, (Vector3Int) value);
            if (type == typeof(Rect))
                return EditorGUILayout.RectField(label, (Rect) value);
            if (type == typeof(Bounds))
                return EditorGUILayout.BoundsField(label, (Bounds) value);
            if (type == typeof(RectInt))
                return EditorGUILayout.RectIntField(label, (RectInt) value);
            if (type == typeof(BoundsInt))
                return EditorGUILayout.BoundsIntField(label, (BoundsInt) value);
            if (type == typeof(Quaternion))
                return Quaternion.Euler(EditorGUILayout.Vector3Field(label, ((Quaternion) value).eulerAngles));
            if (type == typeof(LayerMask))
                return EditorGUILayout.LayerField(label, (LayerMask) value);
            if (type == typeof(Color))
                return EditorGUILayout.ColorField(label, (Color) value);
            if (type == typeof(AnimationCurve))
                return EditorGUILayout.CurveField(label, (AnimationCurve) value);
            if (type == typeof(Gradient))
                return EditorGUILayout.GradientField(label, (Gradient) value);
            if (typeof(Component).IsAssignableFrom(type))
                return EditorGUILayout.ObjectField(label, (Object) value, type, !isAsset);
            if (typeof(Object).IsAssignableFrom(type))
                return EditorGUILayout.ObjectField(label, (Object) value, type, false);

            if (type.GetCustomAttribute(typeof(SerializableAttribute), true) == null) return value;

            if (foldProperty)
            {
                var pathName = EditorSerializedFieldUtility.CreatePathName(context, fieldName);
                if (!EditorGUILayout.Foldout(GetFoldout(pathName), label, true))
                {
                    SetFoldout(pathName, false);
                    return value;
                }

                SetFoldout(pathName, true);
                EditorGUI.indentLevel++;
            }

            foreach (var field in type.GetSerializableFields(true, true))
            {
                var fieldValue = DrawLayoutField(context, $"{fieldName}.{field.Name}", field.FieldType,
                    field.Name.DrawPrettyName(), field.GetValue(value));

                if (field.FieldType.IsEnum)
                    fieldValue = Enum.ToObject(field.FieldType, fieldValue);

                field.SetValue(value, fieldValue);
            }

            if (!foldProperty) return value;

            EditorGUI.indentLevel--;

            return value;
        }

        public static bool DrawArrayLayoutField(string label, object arrayValue, Type elementType, bool enabled)
        {
            if (elementType.IsEnum)
            {
                if (elementType.GetCustomAttribute<FlagsAttribute>() != null)
                    return DrawArray(label, (IList<int>) arrayValue, enabled);
                return DrawArray(label, (IList<Enum>) arrayValue, enabled);
            }

            if (elementType == typeof(bool))
                return DrawArray(label, (IList<bool>) arrayValue, enabled);

            if (elementType == typeof(int) || elementType == typeof(short) || elementType == typeof(byte))
                return DrawArray(label, (IList<int>) arrayValue, enabled);

            if (elementType == typeof(long))
                return DrawArray(label, (IList<long>) arrayValue, enabled);

            if (elementType == typeof(float))
                return DrawArray(label, (IList<float>) arrayValue, enabled);

            if (elementType == typeof(double))
                return DrawArray(label, (IList<double>) arrayValue, enabled);

            if (elementType == typeof(string))
                return DrawArray(label, (IList<string>) arrayValue, enabled);

            if (elementType == typeof(Vector2))
                return DrawArray(label, (IList<Vector2>) arrayValue, enabled);

            if (elementType == typeof(Vector3))
                return DrawArray(label, (IList<Vector3>) arrayValue, enabled);

            if (elementType == typeof(Vector4))
                return DrawArray(label, (IList<Vector4>) arrayValue, enabled);

            if (elementType == typeof(Vector2Int))
                return DrawArray(label, (IList<Vector2Int>) arrayValue, enabled);

            if (elementType == typeof(Vector3Int))
                return DrawArray(label, (IList<Vector3Int>) arrayValue, enabled);

            if (elementType == typeof(Rect))
                return DrawArray(label, (IList<Rect>) arrayValue, enabled);

            if (elementType == typeof(Bounds))
                return DrawArray(label, (IList<Bounds>) arrayValue, enabled);

            if (elementType == typeof(RectInt))
                return DrawArray(label, (IList<RectInt>) arrayValue, enabled);

            if (elementType == typeof(BoundsInt))
                return DrawArray(label, (IList<BoundsInt>) arrayValue, enabled);

            if (elementType == typeof(Quaternion))
                return DrawArray(label, (IList<Quaternion>) arrayValue, enabled);

            if (elementType == typeof(LayerMask))
                return DrawArray(label, (IList<LayerMask>) arrayValue, enabled);

            if (elementType == typeof(Color))
                return DrawArray(label, (IList<Color>) arrayValue, enabled);

            if (elementType == typeof(AnimationCurve))
                return DrawArray(label, (IList<AnimationCurve>) arrayValue, enabled);

            if (elementType == typeof(Gradient))
                return DrawArray(label, (IList<Gradient>) arrayValue, enabled);

            if (typeof(Object).IsAssignableFrom(elementType))
                return DrawArray(label, (IList<Object>) arrayValue, enabled);

            return false;
        }

        public static object DrawArrayLayoutField(Object context, string fieldName, Type elementType, string label,
            object arrayValue)
        {
            if (elementType.IsEnum)
            {
                if (elementType.GetCustomAttribute<FlagsAttribute>() != null)
                    return DrawArrayField(context, fieldName, label, (IList<int>) arrayValue);
                return DrawArrayField(context, fieldName, label, (IList<int>) arrayValue, elementType);
            }

            if (elementType == typeof(bool))
                return DrawArrayField(context, fieldName, label, (IList<bool>) arrayValue);

            if (elementType == typeof(int) || elementType == typeof(short) || elementType == typeof(byte))
                return DrawArrayField(context, fieldName, label, (IList<int>) arrayValue);

            if (elementType == typeof(long))
                return DrawArrayField(context, fieldName, label, (IList<long>) arrayValue);

            if (elementType == typeof(float))
                return DrawArrayField(context, fieldName, label, (IList<float>) arrayValue);

            if (elementType == typeof(double))
                return DrawArrayField(context, fieldName, label, (IList<double>) arrayValue);

            if (elementType == typeof(string))
                return DrawArrayField(context, fieldName, label, (IList<string>) arrayValue);

            if (elementType == typeof(Vector2))
                return DrawArrayField(context, fieldName, label, (IList<Vector2>) arrayValue);

            if (elementType == typeof(Vector3))
                return DrawArrayField(context, fieldName, label, (IList<Vector3>) arrayValue);

            if (elementType == typeof(Vector4))
                return DrawArrayField(context, fieldName, label, (IList<Vector4>) arrayValue);

            if (elementType == typeof(Vector2Int))
                return DrawArrayField(context, fieldName, label, (IList<Vector2Int>) arrayValue);

            if (elementType == typeof(Vector3Int))
                return DrawArrayField(context, fieldName, label, (IList<Vector3Int>) arrayValue);

            if (elementType == typeof(Rect))
                return DrawArrayField(context, fieldName, label, (IList<Rect>) arrayValue);

            if (elementType == typeof(Bounds))
                return DrawArrayField(context, fieldName, label, (IList<Bounds>) arrayValue);

            if (elementType == typeof(RectInt))
                return DrawArrayField(context, fieldName, label, (IList<RectInt>) arrayValue);

            if (elementType == typeof(BoundsInt))
                return DrawArrayField(context, fieldName, label, (IList<BoundsInt>) arrayValue);

            if (elementType == typeof(Quaternion))
                return DrawArrayField(context, fieldName, label, (IList<Quaternion>) arrayValue);

            if (elementType == typeof(LayerMask))
                return DrawArrayField(context, fieldName, label, (IList<LayerMask>) arrayValue);

            if (elementType == typeof(Color))
                return DrawArrayField(context, fieldName, label, (IList<Color>) arrayValue);

            if (elementType == typeof(AnimationCurve))
                return DrawArrayField(context, fieldName, label, (IList<AnimationCurve>) arrayValue);

            if (elementType == typeof(Gradient))
                return DrawArrayField(context, fieldName, label, (IList<Gradient>) arrayValue);

            if (typeof(Object).IsAssignableFrom(elementType))
                return DrawArrayField(context, fieldName, label, (IList<Object>) arrayValue);

            return arrayValue;
        }

        public static IList<T> DrawArrayField<T>(Object context, string fieldName, string label, IList<T> array, Type drawerType = null)
        {
            var pathName = EditorSerializedFieldUtility.CreatePathName(context, fieldName);
            if (!EditorGUILayout.Foldout(GetFoldout(pathName), label, true))
            {
                SetFoldout(pathName, false);
                return array;
            }

            SetFoldout(pathName, true);

            EditorGUI.indentLevel++;
            var size = EditorGUILayout.DelayedIntField("Size", array.Count);
            MightyEditorUtility.ResizeIList(size, ref array);

            for (var i = 0; i < size; i++)
                array[i] = (T) DrawLayoutField(context, fieldName, drawerType ?? typeof(T), $"Element {i}", array[i]);
            EditorGUI.indentLevel--;

            return array;
        }

        public static bool DrawArray<T>(string label, IList<T> array, bool enabled)
        {
            GUI.enabled = true;
            if (!EditorGUILayout.Foldout(GetFoldout(label), label, true))
            {
                GUI.enabled = enabled;
                SetFoldout(label, false);
                return false;
            }

            GUI.enabled = enabled;
            SetFoldout(label, true);

            var isDrawn = true;
            var size = EditorGUILayout.DelayedIntField(label, array.Count);
            EditorGUI.indentLevel++;
            for (var i = 0; i < size; i++)
                isDrawn = DrawLayoutField($"Element {i}", array[i], enabled) && isDrawn;
            EditorGUI.indentLevel--;

            return isDrawn;
        }

        private static bool LabelHasContent(GUIContent label) => label == null || label.text != string.Empty || label.image != null;

        private static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns, GUIStyle style)
        {
            if (!LabelHasContent(label))
                return EditorGUI.IndentedRect(totalPosition);
            if (EditorGUIUtility.wideMode)
            {
                var labelPosition = new Rect(totalPosition.x + IndentSpace, totalPosition.y, EditorGUIUtility.labelWidth - IndentSpace, 16);
                var rect = totalPosition;
                rect.xMin += EditorGUIUtility.labelWidth;
                if (columns > 1)
                {
                    --labelPosition.width;
                    --rect.xMin;
                }

                if (columns == 2)
                {
                    var num = (float) (((double) rect.width - 4.0) / 3.0);
                    rect.xMax -= num + 2f;
                }

                EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, id, style);
                return rect;
            }

            var labelPosition1 = new Rect(totalPosition.x + IndentSpace, totalPosition.y, totalPosition.width - IndentSpace, 16f);
            var rect1 = totalPosition;
            rect1.xMin += IndentSpace + 15f;
            rect1.yMin += 16f;
            EditorGUI.HandlePrefixLabel(totalPosition, labelPosition1, label, id);
            return rect1;
        }

        public static void MultiFloatField(Rect rect, GUIContent[] subLabels, float[] values, float[] labelWidths, Orientation orientation)
        {
            var length = values.Length;
            var num = (rect.width - (length - 1) * 2f) / length;
            var position1 = new Rect(rect)
            {
                width = num
            };
            var labelWidth = EditorGUIUtility.labelWidth;
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            for (var index = 0; index < values.Length; ++index)
            {
                EditorGUIUtility.labelWidth = labelWidths[index];
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        values[index] = EditorGUI.FloatField(position1, subLabels[index], values[index]);
                        break;
                    case Orientation.Vertical:
                        EditorGUI.LabelField(position1, subLabels[index]);
                        position1.y += 16;
                        values[index] = EditorGUI.FloatField(position1, values[index]);
                        position1.y -= 16;
                        break;
                }

                position1.x += num + 2f;
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;
        }

        public static Quaternion DrawRotationEuler(Rect rect, string label, Quaternion rotation) =>
            Quaternion.Euler(EditorGUI.Vector3Field(rect, label, rotation.eulerAngles));

        public static Quaternion DrawRotationEuler(Rect rect, GUIContent label, Quaternion rotation) =>
            Quaternion.Euler(EditorGUI.Vector3Field(rect, label, rotation.eulerAngles));

        public static void DrawRotationEuler(Rect rect, string label, SerializedProperty property) =>
            property.quaternionValue = DrawRotationEuler(rect, label, property.quaternionValue);

        public static void DrawRotationEuler(Rect rect, GUIContent label, SerializedProperty property) =>
            property.quaternionValue = DrawRotationEuler(rect, label, property.quaternionValue);

        public static void DrawRotationEuler(Rect rect, SerializedProperty property) =>
            property.quaternionValue = DrawRotationEuler(rect, property.displayName, property.quaternionValue);

        public static Quaternion DrawRotationEuler(string label, Quaternion rotation) =>
            Quaternion.Euler(EditorGUILayout.Vector3Field(label, rotation.eulerAngles));

        public static Quaternion DrawRotationEuler(GUIContent label, Quaternion rotation) =>
            Quaternion.Euler(EditorGUILayout.Vector3Field(label, rotation.eulerAngles));

        public static void DrawRotationEuler(string label, SerializedProperty property) =>
            property.quaternionValue = DrawRotationEuler(label, property.quaternionValue);

        public static void DrawRotationEuler(SerializedProperty property) =>
            property.quaternionValue = DrawRotationEuler(property.displayName, property.quaternionValue);

        public static void DrawRotationEuler(GUIContent label, SerializedProperty property) =>
            property.quaternionValue = DrawRotationEuler(label, property.quaternionValue);

        public static Rect MultiFieldPrefixLabel(Rect position, GUIContent label, int columns, GUIStyle style = null)
        {
            var controlId = GUIUtility.GetControlID("Foldout".GetHashCode(), FocusType.Keyboard, position);
            position = MultiFieldPrefixLabel(position, controlId, label, columns, style ?? GUIStyle.none);
            position.height = 16f;
            return position;
        }

        public static void MultiFloatField(Rect position, GUIContent label, GUIContent[] subLabels, float[] values, float subLblWidth,
            Orientation orientation = Orientation.Horizontal, GUIStyle labelStyle = null) =>
            MultiFloatField(MultiFieldPrefixLabel(position, label, subLabels.Length, labelStyle), subLabels, values,
                Enumerable.Repeat(subLblWidth, values.Length).ToArray(), orientation);


        public static void MultiFloatField(Rect position, GUIContent label, GUIContent[] subLabels, float[] values, float[] subLblWidths,
            Orientation orientation = Orientation.Horizontal, GUIStyle labelStyle = null) =>
            MultiFloatField(MultiFieldPrefixLabel(position, label, subLabels.Length, labelStyle), subLabels, values, subLblWidths,
                orientation);

        #endregion /Fields
    }
}
#endif