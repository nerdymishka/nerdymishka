using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using NerdyMishka.Util.Arrays;

namespace NerdyMishka.Api.KeePass.Package
{
    public class AesCryptoStreamProvider : IKeePassCryptoStreamProvider
    {
        public KeePassIdentifier Id => new KeePassIdentifier(new byte[]
        {
            0x31, 0xC1, 0xF2, 0xE6, 0xBF, 0x71, 0x43, 0x50,
            0xBE, 0x58, 0x05, 0x21, 0x6A, 0xFC, 0x5A, 0xFF,
        });

        [SuppressMessage("Security", "SCS0011:Weak CBC Mode", Justification = "KeePass does an encrypt then mac scheme.")]
        public Stream CreateCryptoStream(Stream stream, bool encrypt, byte[] key, byte[] iv)
        {
            ICryptoTransform transform = null;
            byte[] localKey = new byte[32];
            byte[] localIV = new byte[16];
            Array.Copy(key, localKey, 32);
            Array.Copy(iv, localIV, 16);

            using (Aes aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                transform = encrypt ?
                    aes.CreateEncryptor(localKey, localIV)
                    : aes.CreateDecryptor(localKey, localIV);

                // The transform performs operations that mutates the key and iv
                // The key and iv can be disguarded after the AesCngCryptoTransform
                // is created.
                localKey.Clear();
                localIV.Clear();
            }

            if (transform == null)
                throw new Exception("AES transform failed");

            return new CryptoStream(stream, transform, encrypt ? CryptoStreamMode.Write : CryptoStreamMode.Read);
        }
    }
}