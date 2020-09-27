using System;
using System.Buffers;
using NerdyMishka.Util.Binary;

namespace NerdyMishka.Security.Cryptography
{
    internal static class ChaCha20Instance
    {
        private static ChaCha20 s_instance;

        public static MemoryProtectionAction Generate()
        {
            if (s_instance == null)
                s_instance = ChaCha20.Create();

            s_instance.GenerateKey();
            var defaultKey = s_instance.Key;

            return (bytes, state, action) =>
            {
                var mpb = (MemoryProtectedBytes)state;
                var key = defaultKey;

                // TODO: add ToBytes method that incorporates offset
                var id = LittleEndianBitConverter.ToBytes(mpb.Id);
                var iv = ArrayPool<byte>.Shared.Rent(12);
                Array.Copy(id, 0, iv, 4, id.Length);

                try
                {
                    var transform = action == MemoryProtectionActionType.Encrypt ?
                        s_instance.CreateEncryptor(key, iv) :
                        s_instance.CreateEncryptor(key, iv);

                    var data = bytes.ToArray();
                    var result = transform.TransformFinalBlock(bytes.ToArray(), 0, bytes.Length);
                    Array.Clear(data, 0, data.Length);

                    return result;
                }
                finally
                {
                    Array.Clear(id, 0, id.Length);
                    ArrayPool<byte>.Shared.Return(iv);
                }
            };
        }

        public static void Dispose()
        {
            if (s_instance != null)
            {
                s_instance.Dispose();
                s_instance = null;
            }
        }
    }
}