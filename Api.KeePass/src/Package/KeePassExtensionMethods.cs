using System;
using System.Security.Cryptography;

namespace NerdyMishka.Api.KeePass.Package
{
    public static class KeePassExtensionMethods
    {
        public static byte[] ToSHA256Hash(this byte[] bytes)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            using (var hash = SHA256.Create())
            {
                return hash.ComputeHash(bytes);
            }
        }
    }
}