using System;
using System.Security.Cryptography;
using NerdyMishka.Security.Cryptography;
using NerdyMishka.Util.Arrays;

namespace NerdyMishka.Api.KeePass.Package
{
    public class ChaCha20RandomByteGenerator : IRandomByteGeneratorEngine
    {
        private ICryptoTransform transform;

        public int Id => 3;

        public void Initialize(ReadOnlySpan<byte> key)
        {
            using (var cipher = ChaCha20.Create())
            {
                cipher.SkipXor = true;
                cipher.Rounds = ChaCha20Round.Twelve;
                var iv = new byte[8] { 0xE8, 0x30, 0x09, 0x4B, 0x97, 0x20, 0x5D, 0x2A };
                var keyBytes = key.ToSHA256Hash().ToArray();
                this.transform = cipher.CreateDecryptor(keyBytes, iv);
                keyBytes.Clear();
                iv.Clear();
            }
        }

        public byte[] NextBytes(int count)
        {
            if (count < 1)
                return Array.Empty<byte>();

            byte[] bytes = new byte[count];
            this.transform.TransformBlock(bytes, 0, count, bytes, 0);

            return bytes;
        }
    }
}
