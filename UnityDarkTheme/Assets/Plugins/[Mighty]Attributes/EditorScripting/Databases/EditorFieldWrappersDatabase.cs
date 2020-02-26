#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MightyAttributes.Editor
{
    public static class EditorFieldWrappersDatabase
    {
        private static readonly Dictionary<Type, BaseEditorFieldWrapper> FieldWrappers;
        private static readonly Dictionary<Type, BaseEditorFieldWrapper> ArrayFieldWrappers;

        static EditorFieldWrappersDatabase()
        {
            FieldWrappers = new Dictionary<Type, BaseEditorFieldWrapper>();
            ArrayFieldWrappers = new Dictionary<Type, BaseEditorFieldWrapper>();

            foreach (var type in Assembly.GetAssembly(typeof(BaseEditorFieldWrapper)).GetTypes())
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(BaseEditorFieldWrapper))) continue;
                if (!(type.GetCustomAttributes(typeof(WrapTypeAttribute), true) is WrapTypeAttribute[] wrapTypeAttributes)) continue;

                foreach (var wrapTypeAttribute in wrapTypeAttributes)
                {
                    foreach (var wrappedType in wrapTypeAttribute.WrappedTypes)
                    {
                        if (type.IsSubclassOf(typeof(BaseArrayFieldWrapper)))
                            ArrayFieldWrappers.Add(wrappedType, (BaseEditorFieldWrapper) Activator.CreateInstance(type));
                        else
                            FieldWrappers.Add(wrappedType, (BaseEditorFieldWrapper) Activator.CreateInstance(type));
                    }
                }
            }
        }

        public static BaseEditorFieldWrapper GetWrapperForType(Type type, bool arrayWrapper)
        {
            if (type.IsEnum) type = typeof(int);
            if (typeof(Object).IsAssignableFrom(type)) type = typeof(Object);
            return arrayWrapper ? ArrayFieldWrappers.ContainsKey(type) ? ArrayFieldWrappers[type] : null
                : FieldWrappers.ContainsKey(type) ? FieldWrappers[type] : null;
        }
    }

    #region Wrappers

    [AttributeUsage(AttributeTargets.Class)]
    public class WrapTypeAttribute : Attribute
    {
        public Type[] WrappedTypes { get; }

        public WrapTypeAttribute(params Type[] types) => WrappedTypes = types;
    }

    public abstract class BaseEditorFieldWrapper
    {
        public abstract void SetValue(object value);
        public abstract void GetValue(out object value);
        public abstract void ResetValue();
    }

    #region Field Wrappers

    [Serializable, WrapType(typeof(bool))]
    public class BoolFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private bool _value;

        public override void SetValue(object value) => _value = (bool) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(int), typeof(Enum))]
    public class IntFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private int _value;

        public override void SetValue(object value) => _value = Convert.ToInt32(value);

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(long))]
    public class LongFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private long _value;

        public override void SetValue(object value) => _value = Convert.ToInt64(value);

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(float))]
    public class FloatFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private float _value;

        public override void SetValue(object value) => _value = (float) Convert.ToDouble(value.ToString());

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(double))]
    public class DoubleFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private double _value;

        public override void SetValue(object value) => _value = Convert.ToDouble(value.ToString());

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(char))]
    public class CharFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private char _value;

        public override void SetValue(object value) => _value = (char) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }    
    
    [Serializable, WrapType(typeof(string))]
    public class StringFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private string _value;

        public override void SetValue(object value) => _value = (string) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector2))]
    public class Vector2FieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Vector2 _value;

        public override void SetValue(object value) => _value = (Vector2) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector3))]
    public class Vector3FieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Vector3 _value;

        public override void SetValue(object value) => _value = (Vector3) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector4))]
    public class Vector4FieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Vector4 _value;

        public override void SetValue(object value) => _value = (Vector4) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Rect))]
    public class RectFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Rect _value;

        public override void SetValue(object value) => _value = (Rect) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Bounds))]
    public class BoundsFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Bounds _value;

        public override void SetValue(object value) => _value = (Bounds) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector2Int))]
    public class Vector2IntFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Vector2Int _value;

        public override void SetValue(object value) => _value = (Vector2Int) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector3Int))]
    public class Vector3IntFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Vector3Int _value;

        public override void SetValue(object value) => _value = (Vector3Int) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(RectInt))]
    public class RectIntFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private RectInt _value;

        public override void SetValue(object value) => _value = (RectInt) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(BoundsInt))]
    public class BoundsIntFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private BoundsInt _value;

        public override void SetValue(object value) => _value = (BoundsInt) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Quaternion))]
    public class QuaternionFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Quaternion _value;

        public override void SetValue(object value) => _value = (Quaternion) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(LayerMask))]
    public class LayerMaskFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private LayerMask _value;

        public override void SetValue(object value) => _value = (LayerMask) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Color))]
    public class ColorFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Color _value;

        public override void SetValue(object value) => _value = (Color) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Color32))]
    public class Color32FieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Color32 _value;

        public override void SetValue(object value) => _value = (Color32) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Gradient))]
    public class GradientCurveFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Gradient _value;

        public override void SetValue(object value) => _value = (Gradient) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(AnimationCurve))]
    public class AnimationCurveFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private AnimationCurve _value;

        public override void SetValue(object value) => _value = (AnimationCurve) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Object))]
    public class UnityObjectFieldWrapper : BaseEditorFieldWrapper
    {
        [SerializeField] private Object _value;

        public override void SetValue(object value) => _value = (Object) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    #endregion /Field Wrappers

    #region Array Field Wrappers

    public abstract class BaseArrayFieldWrapper : BaseEditorFieldWrapper
    {
        public bool foldout;
    }

    [Serializable, WrapType(typeof(bool))]
    public class BoolArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private bool[] _value;

        public override void SetValue(object value) => _value = (bool[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(int), typeof(Enum))]
    public class IntArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private int[] _value;

        public override void SetValue(object value) => _value = (int[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(long))]
    public class LongArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private long[] _value;

        public override void SetValue(object value) => _value = (long[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(float))]
    public class FloatArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private float[] _value;

        public override void SetValue(object value) => _value = (float[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(double))]
    public class DoubleArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private double[] _value;

        public override void SetValue(object value) => _value = (double[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }
    
    [Serializable, WrapType(typeof(char))]
    public class CharArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private char[] _value;

        public override void SetValue(object value) => _value = (char[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(string))]
    public class StringArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private string[] _value;

        public override void SetValue(object value) => _value = (string[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector2))]
    public class Vector2ArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Vector2[] _value;

        public override void SetValue(object value) => _value = (Vector2[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector3))]
    public class Vector3ArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Vector3[] _value;

        public override void SetValue(object value) => _value = (Vector3[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector4))]
    public class Vector4ArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Vector4[] _value;

        public override void SetValue(object value) => _value = (Vector4[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Rect))]
    public class RectArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Rect[] _value;

        public override void SetValue(object value) => _value = (Rect[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Bounds))]
    public class BoundsArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Bounds[] _value;

        public override void SetValue(object value) => _value = (Bounds[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector2Int))]
    public class Vector2IntArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Vector2Int[] _value;

        public override void SetValue(object value) => _value = (Vector2Int[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Vector3Int))]
    public class Vector3IntArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Vector3Int[] _value;

        public override void SetValue(object value) => _value = (Vector3Int[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(RectInt))]
    public class RectIntArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private RectInt[] _value;

        public override void SetValue(object value) => _value = (RectInt[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(BoundsInt))]
    public class BoundsIntArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private BoundsInt[] _value;

        public override void SetValue(object value) => _value = (BoundsInt[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Quaternion))]
    public class QuaternionArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Quaternion[] _value;

        public override void SetValue(object value) => _value = (Quaternion[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(LayerMask))]
    public class LayerMaskArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private LayerMask[] _value;

        public override void SetValue(object value) => _value = (LayerMask[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Color))]
    public class ColorArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Color[] _value;

        public override void SetValue(object value) => _value = (Color[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Color32))]
    public class Color32ArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Color32[] _value;

        public override void SetValue(object value) => _value = (Color32[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Gradient))]
    public class GradientArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Gradient[] _value;

        public override void SetValue(object value) => _value = (Gradient[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(AnimationCurve))]
    public class AnimationCurveArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private AnimationCurve[] _value;

        public override void SetValue(object value) => _value = (AnimationCurve[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    [Serializable, WrapType(typeof(Object))]
    public class UnityObjectArrayFieldWrapper : BaseArrayFieldWrapper
    {
        [SerializeField] private Object[] _value;

        public override void SetValue(object value) => _value = (Object[]) value;

        public override void GetValue(out object value) => value = _value;

        public override void ResetValue() => _value = default;
    }

    #endregion /Array Field Wrappers

    #endregion
}
#endif