#if UNITY_EDITOR
using System.Linq;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public static class MightyEditorUtility
    {
        public static readonly ObjectIDGenerator IDGenerator = new ObjectIDGenerator();

        public static long GetUniqueID(this object obj) => IDGenerator.GetId(obj, out _);

        public static IEnumerable<MightyEditor> GetMightyEditors()
        {
            foreach (var script in SerializedPropertyUtility.FindAllObjects<MonoBehaviour>())
                if (script.CreateEditor(out var mightyEditor))
                    yield return mightyEditor;
        }

        public static bool IsMightyScript(this MonoBehaviour script)
        {
            var type = script.GetType();

            if (ReflectionUtility.GetAllFields(type,
                    f => f.GetCustomAttributes(typeof(BaseMightyAttribute), true).Length > 0).ToArray().Length > 0) return true;

            if (ReflectionUtility.GetAllProperties(type,
                    p => p.GetCustomAttributes(typeof(BaseMightyAttribute), true).Length > 0).ToArray().Length > 0) return true;

            if (ReflectionUtility.GetAllMethods(type,
                    m => m.GetCustomAttributes(typeof(BaseMightyAttribute), true).Length > 0).ToArray().Length > 0) return true;

            return false;
        }

        public static bool CreateEditor(this MonoBehaviour script, out MightyEditor mightyEditor)
        {
            var isMightyScript = script.IsMightyScript();
            mightyEditor = isMightyScript ? (MightyEditor) UnityEditor.Editor.CreateEditor(script, typeof(MightyEditor)) : null;
            return isMightyScript;
        }

        public static void ResizeArray(ref Array array, Type elementType, int length)
        {
            if (array == null)
            {
                array = Array.CreateInstance(elementType, length);
                return;
            }

            if (length == array.Length) return;

            while (length > array.Length) array = AddArrayElement(array, elementType);
            while (length < array.Length) array = RemoveArrayElement(array, elementType, length - 1);
        }

        public static Array AddArrayElement(Array array, Type elementType)
        {
            var length = array.Length;
            var newArray = Array.CreateInstance(elementType, length + 1);
            var i = 0;
            for (; i < length; i++) newArray.SetValue(array.GetValue(i), i);
            try
            {
                newArray.SetValue(Activator.CreateInstance(elementType), i);
            }
            catch
            {
                // ignored
            }

            return newArray;
        }

        public static Array RemoveArrayElement(Array array, Type elementType, int index)
        {
            var length = array.Length;
            if (length == 0) return array;

            var newArray = Array.CreateInstance(elementType, length + 1);
            var offset = 0;
            for (var i = 0; i < length - 1; i++)
            {
                if (i == index) offset = 1;
                newArray.SetValue(array.GetValue(i + offset), i);
            }

            return newArray;
        }

        public static void ResizeArray<T>(int length, ref T[] array)
        {
            if (array == null)
            {
                array = new T[length];
                return;
            }

            if (length == array.Length) return;

            while (length > array.Length) array = AddArrayElement(array);
            while (length < array.Length) array = RemoveArrayElement(array, length - 1);
        }

        public static T[] AddArrayElement<T>(this T[] array, T element = default)
        {
            var length = array.Length;
            var newArray = new T[length + 1];
            var i = 0;
            for (; i < length; i++) newArray[i] = array[i];
            try
            {
                newArray[i] = element;
            }
            catch
            {
                // ignored
            }

            return newArray;
        }

        public static T[] AddArrayElement<T>(this T[] array, int index, T element = default)
        {
            var length = array.Length;
            var newArray = new T[length + 1];
            var offset = 0;
            for (var i = 0; i < length + 1; i++)
            {
                if (i == index)
                {
                    offset = 1;
                    try
                    {
                        newArray[i] = element;
                    }
                    catch
                    {
                        // ignored
                    }

                    continue;
                }

                newArray[i] = array[i - offset];
            }

            return newArray;
        }

        public static T[] RemoveArrayElement<T>(this T[] array, int index)
        {
            var length = array.Length;
            if (length == 0) return array;
            var newArray = new T[length - 1];
            var offset = 0;
            for (var i = 0; i < length - 1; i++)
            {
                if (i == index)
                    offset = 1;

                newArray[i] = array[i + offset];
            }

            return newArray;
        }

        public static void ResizeIList<T>(int length, ref IList<T> list)
        {
            if (list == null) return;

            if (length == list.Count) return;

            while (length > list.Count) list = AddIListElement(list);
            while (length < list.Count) list = RemoveIListElement(list, length - 1);
        }

        public static IList<T> AddIListElement<T>(this IList<T> list, T element = default)
        {
            var length = list.Count;
            var newArray = new T[length];
            var i = 0;
            for (; i < length; i++) newArray[i] = list[i];
            try
            {
                newArray[i] = element;
            }
            catch
            {
                // ignored
            }

            return newArray;
        }

        public static IList<T> RemoveIListElement<T>(this IList<T> list, int index)
        {
            var length = list.Count;
            if (length == 0) return list;
            var newArray = new T[length - 1];
            var offset = 0;
            for (var i = 0; i < length - 1; i++)
            {
                if (i == index)
                    offset = 1;

                newArray[i] = list[i + offset];
            }

            return newArray;
        }
        
        public static int ToBitMask(this int value) => 1 << value;

        public static byte GetBitIndex(this uint value, bool offsetOnce = false)
        {
            if (value == 0)
            {
                if (!offsetOnce)
                    throw new ArgumentOutOfRangeException();
                return 0;
            }

            byte index = 0;
            while (value != 1UL << index)
                if (++index > 31)
                    throw new ArgumentOutOfRangeException();

            return offsetOnce ? (byte) (index + 1) : index;
        }

        public static bool MaskContains(this int mask, int flag) => (mask & flag) != 0;

        public static int AddFlag(this int mask, int flag) => mask | flag;

        public static float[] Vector2ToArray(this Vector2 source) => new[] {source.x, source.y};

        public static Vector2 ArrayToVector2(this float[] source) => new Vector2(source[0], source[1]);

        public static float[] Vector3ToArray(this Vector3 source) => new[] {source.x, source.y, source.z};

        public static Vector3 ArrayToVector3(this float[] source) => new Vector3(source[0], source[1], source[2]);

        public static float[] Vector4ToArray(this Vector4 source) => new[] {source.x, source.y, source.z, source.w};

        public static Vector4 ArrayToVector4(this float[] source) => new Vector4(source[0], source[1], source[2], source[3]);

        public static bool IsBuildTargetSupported(this BuildTarget target)
        {
            var moduleManager = Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            var isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            return (bool) isPlatformSupportLoaded.Invoke(null,
                new object[] {(string) getTargetStringFromBuildTarget.Invoke(null, new object[] {target})});
        }
    }
}
#endif