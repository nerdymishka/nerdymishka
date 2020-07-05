using System;
using System.Collections.Concurrent;
using System.Windows;
using Microsoft.Win32;

namespace NerdyMishka.Windows.Registry
{
    public static class RegistryUtil
    {
        private static readonly ConcurrentDictionary<string, RegistryKey> Hives =
            new ConcurrentDictionary<string, RegistryKey>();

        public static object GetValue(
            string subKeyName,
            string propertyName,
            object defaultValue = null)
        {
            using (var key = OpenSubKey(subKeyName))
            {
                return key.GetValue(propertyName, defaultValue);
            }
        }

        public static string GetValueAsString(
            string keyName,
            string defaultValue = null)
        {
            return GetValue(keyName, defaultValue) as string;
        }

        public static int GetValueAsInt32(
            string keyName,
            int defaultValue = -1)
        {
            return (int)GetValue(keyName, defaultValue);
        }

        public static bool GetValueAsBoolean(
            string keyName,
            bool defaultValue = false)
        {
            var result = GetValueAsInt32(keyName, defaultValue == true ? 1 : 0);
            return result == 1;
        }

        public static long GetValueAsInt64(
          string keyName,
          long defaultValue = -1)
        {
            return (long)GetValue(keyName, defaultValue);
        }

        public static byte[] GetValueAsBinary(
          string keyName,
          byte[] defaultValue = null)
        {
            return (byte[])GetValue(keyName, defaultValue);
        }

        public static string[] GetValueAsStringArray(
          string keyName,
          string[] defaultValue = null)
        {
            return (string[])GetValue(keyName, defaultValue);
        }

        public static string GetValueAsString(string keyName)
        {
            return GetValue(keyName) as string;
        }

        public static int GetValueAsInt32(
            string subKeyName,
            string propertyName,
            int defaultValue = -1)
        {
            using (var key = OpenSubKey(subKeyName))
            {
                return (int)key.GetValue(propertyName, defaultValue);
            }
        }

        public static bool GetValueAsBoolean(
            string subKeyName,
            string propertyName,
            bool defaultValue = false)
        {
            var result = GetValueAsInt32(subKeyName, propertyName, defaultValue == true ? 1 : 0);
            return result == 1;
        }

        public static long GetValueAsInt64(
          string subKeyName,
          string propertyName,
          long defaultValue = -1)
        {
            return (long)GetValue(subKeyName, propertyName, defaultValue);
        }

        public static byte[] GetValueAsBinary(
          string subKeyName,
          string propertyName,
          byte[] defaultValue = null)
        {
            return (byte[])GetValue(subKeyName, propertyName, defaultValue);
        }

        public static string[] GetValueAsStringArray(
          string subKeyName,
          string propertyName,
          string[] defaultValue = null)
        {
            return (string[])GetValue(subKeyName, propertyName, defaultValue);
        }

        public static object GetValue(
            string keyName,
            object defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentNullException(nameof(keyName));

            var i = keyName.LastIndexOf('/');
            if (i == -1)
                i = keyName.LastIndexOf('\\');
            var propertyName = keyName.Substring(i + 1);
            var path = keyName.Substring(0, i);

            using (var key = OpenSubKey(path))
            {
                return key.GetValue(propertyName, defaultValue);
            }
        }

        public static RegistryKey CreateSubKey(string subKeyName)
        {
            if (string.IsNullOrWhiteSpace(subKeyName))
                throw new ArgumentNullException(nameof(subKeyName));

            var set = subKeyName.Split(':', '/');
            if (set.Length != 2)
                throw new ArgumentException("Invalid registry path");

            using (var hive = OpenHive(set[0]))
            {
                return hive.CreateSubKey(set[1].Replace('\\', '/'));
            }
        }

        public static void SetValue(
           string subKey,
           string propertyName,
           string[] propertyValue)
        {
            SetValue(
                subKey,
                propertyName,
                propertyValue,
                RegistryValueKind.MultiString);
        }

        public static void SetValue(
           string subKey,
           string propertyName,
           ReadOnlySpan<byte> propertyValue)
        {
            var bytes = new byte[propertyValue.Length];
            propertyValue.CopyTo(bytes);
            SetValue(subKey, propertyName, bytes);
            Array.Clear(bytes, 0, bytes.Length);
        }

        public static void SetValue(
           string subKey,
           string propertyName,
           byte[] propertyValue)
        {
            SetValue(
                subKey,
                propertyName,
                propertyValue,
                RegistryValueKind.Binary);
        }

        public static void SetValue(
            string keyName,
            byte[] propertyValue)
        {
            SetValue(
                keyName,
                propertyValue,
                RegistryValueKind.Binary);
        }

        public static void SetValue(
            string keyName,
            ReadOnlySpan<byte> propertyValue)
        {
            var bytes = new byte[propertyValue.Length];
            propertyValue.CopyTo(bytes);
            SetValue(keyName, bytes);
            Array.Clear(bytes, 0, bytes.Length);
        }

        public static void SetValue(
            string keyName,
            ReadOnlySpan<char> propertyValue)
        {
            SetValue(keyName, (object)propertyValue.ToString());
        }

        public static void SetValue(
            string keyName,
            int propertyValue)
        {
            SetValue(
                keyName,
                propertyValue,
                RegistryValueKind.DWord);
        }

        public static void SetValue(
            string keyName,
            long propertyValue)
        {
            SetValue(
                keyName,
                propertyValue,
                RegistryValueKind.QWord);
        }

        public static void SetValue(
            string keyName,
            string[] propertyValue)
        {
            SetValue(
                keyName,
                propertyValue,
                RegistryValueKind.MultiString);
        }

        public static void SetValue(
                string subKey,
                string propertyName,
                long propertyValue)
        {
            SetValue(
                subKey,
                propertyName,
                propertyValue,
                RegistryValueKind.QWord);
        }

        public static void SetValue(
            string subKey,
            string propertyName,
            int propertyValue)
        {
            SetValue(
                subKey,
                propertyName,
                propertyValue,
                RegistryValueKind.DWord);
        }

        public static void SetValue(
            string subKey,
            string propertyName,
            object propertyValue)
        {
            using (var key = CreateSubKey(subKey))
            {
                key.SetValue(propertyName, propertyValue);
            }
        }

        public static void SetValue(
            string keyName,
            object propertyValue)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentNullException(nameof(keyName));

            var i = keyName.LastIndexOf('/');
            if (i == -1)
                i = keyName.LastIndexOf('\\');
            var propertyName = keyName.Substring(i + 1);
            var path = keyName.Substring(0, i);

            using (var key = CreateSubKey(path))
            {
                if (propertyValue is string @string)
                {
                    key.SetValue(
                        propertyName,
                        @string,
                        RegistryValueKind.String);

                    return;
                }

                key.SetValue(propertyName, propertyValue);
            }
        }

        public static void SetValue(
            string subKey,
            string propertyName,
            object propertyValue,
            RegistryValueKind kind)
        {
            using (var key = CreateSubKey(subKey))
            {
                key.SetValue(propertyName, propertyValue, kind);
            }
        }

        public static void SetValue(
           string keyName,
           object propertyValue,
           RegistryValueKind kind)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentNullException(nameof(keyName));

            var i = keyName.LastIndexOf('/');
            if (i == -1)
                i = keyName.LastIndexOf('\\');
            var propertyName = keyName.Substring(i + 1);
            var path = keyName.Substring(0, i);

            using (var key = CreateSubKey(path))
            {
                if (propertyValue is string @string)
                {
                    key.SetValue(
                        propertyName,
                        @string,
                        RegistryValueKind.String);

                    return;
                }

                key.SetValue(propertyName, propertyValue, kind);
            }
        }

        public static RegistryKey OpenSubKey(RegistryKey hive, string value)
        {
            if (hive is null)
                throw new ArgumentNullException(nameof(hive));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

#if NETSTANDARD2_0
            return hive.OpenSubKey(value.Replace("\\", "/"));
        }
#else
            return hive.OpenSubKey(value.Replace("\\", "/", StringComparison.OrdinalIgnoreCase));
        }
#endif

        public static RegistryKey OpenSubKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            var set = value.Split(':', '/');
            if (set.Length != 2)
                throw new ArgumentException("Invalid registry path");

            using (var hive = OpenHive(set[0]))
            {
                return hive.OpenSubKey(set[1].Replace('\\', '/'));
            }
        }

        public static RegistryKey OpenHive(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            switch (value.ToUpperInvariant())
            {
                case "HKLM":
                    return Microsoft.Win32.Registry.LocalMachine;
                case "HKCU":
                    return Microsoft.Win32.Registry.CurrentUser;
                case "HKCR":
                    return Microsoft.Win32.Registry.ClassesRoot;
                case "HKCC":
                    return Microsoft.Win32.Registry.CurrentConfig;
                case "HKU":
                    return Microsoft.Win32.Registry.Users;
                default:
                    {
                        Hives.TryGetValue(value, out RegistryKey key);
                        return key;
                    }
            }
        }
    }
}