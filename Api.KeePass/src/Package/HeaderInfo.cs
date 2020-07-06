using System.Collections.Generic;

namespace NerdyMishka.Api.KeePass.Package
{
    public class HeaderInfo
    {
        public const uint Signature1 = 0x9AA2D903;

        public const uint Signature2 = 0xB54BFB67;

        public const uint Version = 0x00030001;

        public const uint Version4 = 0x0004000;

        internal const uint Mask = 0xFFFF0000;

        private byte[] randomByteGeneratorCryptoKey;

        private byte randomByteGeneratorCryptoType;

        public KeePassIdentifier DatabaseCipherId { get; set; }

        public byte DatabaseCompression { get; set; }

        public IReadOnlyList<byte> DatabaseCipherKeySeed { get; set; }

        public IReadOnlyList<byte> MasterKeyHashKey { get; set; }

        public long MasterKeyHashRounds { get; set; }

        public IReadOnlyList<byte> DatabaseCipherIV { get; set; }

        public IReadOnlyList<byte> RandomByteGeneratorCryptoKey
        {
            get
            {
                return this.randomByteGeneratorCryptoKey;
            }

            set
            {
                // this.randomByteGenerator = null;
                if (value == null)
                {
                    this.randomByteGeneratorCryptoKey = null;
                    return;
                }

                var set = new byte[value.Count];
                for (var i = 0; i < set.Length; i++)
                {
                    set[i] = value[i];
                }

                this.randomByteGeneratorCryptoKey = set;
            }
        }

        public IReadOnlyList<byte> HeaderByteMarks { get; set; }

        public byte RandomByteGeneratorCryptoType
        {
            get
            {
                return this.randomByteGeneratorCryptoType;
            }

            set
            {
                this.randomByteGeneratorCryptoType = value;
            }
        }

        public IReadOnlyList<byte> Hash { get; set; }
    }
}