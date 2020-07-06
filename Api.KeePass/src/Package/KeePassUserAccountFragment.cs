using System;
using System.IO;
using NerdyMishka.Util.Arrays;

namespace NerdyMishka.Api.KeePass.Package
{
    public class KeePassUserAccountFragment : KeePassCompositeKeyFragment
    {
        private static readonly byte[] Entropy = new byte[]
        {
            0xDE, 0x13, 0x5B, 0x5F,
            0x18, 0xA3, 0x46, 0x70,
            0xB2, 0x57, 0x24, 0x29,
            0x69, 0x88, 0x98, 0xE6,
        };

        public KeePassUserAccountFragment(string keyLocation = null)
        {
            if (Provider == null)
                throw new InvalidOperationException("MasterKeyUserAccount.Provider must be set before creating a MasterKeyUserAccount instnace");

            this.UpdateKey(keyLocation);
        }

        public static string Bin { get; set; }

        public static IProtectedDataProvider Provider { get; set; }

        public void UpdateKey(string keyLocation)
        {
            var filePath = string.IsNullOrWhiteSpace(keyLocation) ? GetKeyFilePath() : keyLocation;
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(filePath))
            {
                var key = SetKey(filePath);
                this.SetData(key);
            }
            else
            {
                var key = GetKey(filePath);
                this.SetData(key);
            }
        }

        private static string GetKeyFilePath()
        {
            var roaming = Environment.GetEnvironmentVariable("APPDATA");
            if (string.IsNullOrWhiteSpace(roaming))
            {
                roaming = Environment.GetEnvironmentVariable("HOME");
            }

            if (string.IsNullOrWhiteSpace(roaming))
                throw new InvalidProgramException("Could not determine home directory");

            roaming = System.IO.Path.Combine(roaming, "KeePass", Bin);

            return roaming;
        }

        private static ReadOnlySpan<byte> GetKey(string filepath)
        {
            ReadOnlySpan<byte> bytes = File.ReadAllBytes(filepath);

            return Provider.UnprotectData(bytes, Entropy);
        }

        private static ReadOnlySpan<byte> SetKey(string filepath)
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var randomByteGenerator = RandomByteGeneratorFactory.GetGenerator(2);
                var initializer = new byte[32];
                randomByteGenerator.Initialize(initializer);
                var key = randomByteGenerator.NextBytes(64);
                var bytes = Provider.ProtectData(key, Entropy);
                var data = bytes.ToArray();
                File.WriteAllBytes(filepath, data);
                data.Clear();

                return key;
            }
        }
    }
}