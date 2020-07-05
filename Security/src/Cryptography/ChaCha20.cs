using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public class ChaCha20 : SymmetricAlgorithm
    {
        private static readonly KeySizes[] ChaChaLegalBlockSizes = new[] { new KeySizes(64, 64, 0) };
        private static readonly KeySizes[] ChaChaLegalKeySizes = new[] { new KeySizes(128, 256, 128) };
        private readonly NerdyRandomNumberGenerator rng;

        /// <summary>
        ///  Initializes a new instance of the <see cref="ChaCha20"/> class.
        /// </summary>
        protected ChaCha20()
        {
#if !NETCOREAPP
            this.LegalBlockSizesValue = ChaChaLegalBlockSizes;
            this.LegalKeySizesValue = ChaChaLegalKeySizes;
#endif
            this.BlockSize = 64;
            this.KeySize = 256;
            this.rng = new NerdyRandomNumberGenerator();
        }

        /// <summary>
        /// Gets the block sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                return ChaChaLegalBlockSizes;
            }
        }

        /// <summary>
        /// Gets the key sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalKeySizes
        {
            get
            {
                return ChaChaLegalKeySizes;
            }
        }

        public int Counter { get; set; } = 0;

        public ChaCha20Round Rounds { get; set; } = ChaCha20Round.Twenty;

#pragma warning disable CS0109

        /// <summary>
        /// Creates a new instance of <see cref="NerdyMishka.Security.Cryptography.ChaCha20" /> class.
        /// </summary>
        /// <returns>A new instance of <see cref="ChaCha20"/>.</returns>
        public static new ChaCha20 Create()
        {
            return new ChaCha20();
        }
#pragma warning restore CS0109

        /// <summary>
        /// Creates a symmetric decryptor object with the <paramref name="rgbKey"/> and initialization vector <paramref name="rgbIV"/>.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>A symmetric decryptor object.</returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new ChaCha20Transform(rgbKey, rgbIV, this.Rounds, this.Counter);
        }

        /// <summary>
        /// Creates a symmetric encryptor object with the <paramref name="rgbKey"/> and initialization vector <paramref name="rgbIV"/>.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>A symmetric encryptor object.</returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new ChaCha20Transform(rgbKey, rgbIV, this.Rounds, this.Counter);
        }

        public override void GenerateIV()
        {
            this.IV = GetRandomBytes(this.rng, this.BlockSize / 8);
        }

        public override void GenerateKey()
        {
            this.Key = GetRandomBytes(this.rng, this.KeySize / 8);
        }

        private static byte[] GetRandomBytes(NerdyRandomNumberGenerator rng, int byteCount)
        {
            byte[] bytes = new byte[byteCount];
            rng.NextBytes(bytes);
            return bytes;
        }
    }
}