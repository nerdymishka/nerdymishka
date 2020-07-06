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
                var mpd = (MemoryProtectedBytes)state;
                var key = mpd.Key ?? defaultKey;
                var iv = mpd.IV;
                var transform = action == MemoryProtectionActionType.Encrypt ?
                    s_instance.CreateEncryptor(key, iv) :
                    s_instance.CreateEncryptor(key, iv);

                return transform.TransformFinalBlock(bytes.ToArray(), 0, bytes.Length);
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