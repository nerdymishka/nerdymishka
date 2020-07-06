using System;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Api.KeePass.Package
{
    public static class KeePassExtensionMethods
    {
        public static string ToHexString(this ReadOnlySpan<byte> bytes)
        {
            char[] chars = new char[bytes.Length * 2];
            int bit;
            for (int i = 0; i < bytes.Length; i++)
            {
                bit = bytes[i] >> 4;
                chars[i * 2] = (char)(55 + bit + (((bit - 10) >> 31) & -7));
                bit = bytes[i] & 0xF;
                chars[(i * 2) + 1] = (char)(55 + bit + (((bit - 10) >> 31) & -7));
            }

            return new string(chars);
        }

        public static ReadOnlySpan<byte> ToSHA256Hash(this ReadOnlySpan<byte> bytes)
        {
            using (var hash = SHA256.Create())
            {
                byte[] result = null;
#if NETSTANDARD2_0
                result = hash.ComputeHash(bytes.ToArray());
#else
                var size = hash.HashSize >> 3;
                Span<byte> uiSpan = stackalloc byte[64];
                uiSpan = uiSpan.Slice(0, size);

                if (!hash.TryComputeHash(bytes, uiSpan, out int bytesWritten) || bytesWritten != size)
                {
                    throw new CryptographicException();
                }

                result = uiSpan.ToArray();
#endif
                return result;
            }
        }

        internal static void Write(this Stream stream, ReadOnlySpan<byte> span)
        {
            stream.Write(span.ToArray(), 0, span.Length);
        }
    }
}