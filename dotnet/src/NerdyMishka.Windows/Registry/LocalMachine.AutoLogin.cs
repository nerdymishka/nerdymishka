using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;

namespace NerdyMishka.Windows.Registry
{
    [SuppressMessage("", "SA1601:", Justification = "Comments in another file")]
    public static partial class LocalMachine
    {
        private const string AutoAdminLogin = "AutoAdminLogin";
        private const string DefaultUserName = "DefaultUserName";
        private const string DefaultPassword = "DefaultPassword";
        private const string SubKey = "HKLM:/SOFTWARE/Microsoft/Windows NT/CurrentVersion/Winlogon";

        public static void EnableAutoLogin(NetworkCredential credential)
        {
            if (credential is null)
                throw new ArgumentNullException(nameof(credential));

            RegistryUtil.SetValue(SubKey, AutoAdminLogin, 1);
            RegistryUtil.SetValue(SubKey, DefaultUserName, credential.UserName);
            RegistryUtil.SetValue(SubKey, DefaultPassword, credential.Password);
        }

        public static void EnableAutoLogin(string userName, string password)
        {
            const string SubKey = "HKLM:/SOFTWARE/Microsoft/Windows NT/CurrentVersion/Winlogon";
            RegistryUtil.SetValue(SubKey, AutoAdminLogin, 1);
            RegistryUtil.SetValue(SubKey, DefaultUserName, userName);
            RegistryUtil.SetValue(SubKey, DefaultPassword, password);
        }

        public static void DisableAutoLogin()
        {
            using (var subKey = RegistryUtil.CreateSubKey(SubKey))
            {
                subKey.DeleteValue(AutoAdminLogin);
                subKey.DeleteValue(DefaultUserName);
                subKey.DeleteValue(DefaultPassword);
            }
        }
    }
}