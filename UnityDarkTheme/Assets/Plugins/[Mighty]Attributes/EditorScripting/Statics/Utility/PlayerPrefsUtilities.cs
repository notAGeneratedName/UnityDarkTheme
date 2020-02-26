#if UNITY_EDITOR
using System;
using UnityEngine;

namespace MightyAttributes.Editor
{
    public static class PlayerPrefsUtilities
    {
        public static T GetPlayerPref<T>(string name, T defaultValue)
        {
            if (typeof(bool) == typeof(T))
            {
                if (PlayerPrefs.HasKey(name)) return (T) Convert.ChangeType(PlayerPrefs.GetInt(name) == 1, typeof(T));
                PlayerPrefs.SetInt(name, Convert.ToBoolean(defaultValue) ? 1 : 0);
            }

            if (typeof(int) == typeof(T))
            {
                if (PlayerPrefs.HasKey(name)) return (T) Convert.ChangeType(PlayerPrefs.GetInt(name), typeof(T));
                PlayerPrefs.SetInt(name, Convert.ToInt32(defaultValue));
            }

            if (typeof(float) == typeof(T))
            {
                if (PlayerPrefs.HasKey(name)) return (T) Convert.ChangeType(PlayerPrefs.GetFloat(name), typeof(T));
                PlayerPrefs.SetFloat(name, Convert.ToSingle(defaultValue));
            }

            if (typeof(string) == typeof(T))
            {
                if (PlayerPrefs.HasKey(name)) return (T) Convert.ChangeType(PlayerPrefs.GetString(name), typeof(T));
                PlayerPrefs.SetString(name, Convert.ToString(defaultValue));
            }

            return defaultValue;
        }

        public static void SetPlayerPref<T>(string name, T value)
        {
            if (typeof(bool) == (typeof(T))) PlayerPrefs.SetInt(name, Convert.ToBoolean(value) ? 1 : 0);

            if (typeof(int) == (typeof(T))) PlayerPrefs.SetInt(name, Convert.ToInt32(value));

            if (typeof(float) == (typeof(T))) PlayerPrefs.SetFloat(name, Convert.ToSingle(value));

            if (typeof(string) == (typeof(T))) PlayerPrefs.SetString(name, Convert.ToString(value));
        }
    }
}
#endif