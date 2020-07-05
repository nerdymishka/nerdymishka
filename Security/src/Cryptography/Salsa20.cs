using System;
using System.Security.Cryptography;
using NerdyMishka.Util.Binary;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// An implementation of Salsa20, a stream cipher proposed by Daniel J. Bernstein available for
    /// use in the public domain.
    /// </summary>
    public class Salsa20 : SymmetricAlgorithm
    {
        private static readonly KeySizes[] SalsaLegalBlockSizes = new[] { new KeySizes(64, 64, 0) };
        private static readonly KeySizes[] SalsaLegalKeySizes = new[] { new KeySizes(128, 256, 128) };
        private readonly NerdyRandomNumberGenerator rng;

        /// <summary>
        /// Initializes a new instance of the <see cref="Salsa20"/> class.
        /// </summary>
        protected Salsa20()
        {
#if !NETCOREAPP
            this.LegalBlockSizesValue = SalsaLegalBlockSizes;
            this.LegalKeySizesValue = SalsaLegalKeySizes;
#endif
            this.BlockSize = 64;
            this.KeySize = 256;
            this.Rounds = Salsa20Round.Twenty;
            this.rng = new NerdyRandomNumberGenerator();
        }

        /// <summary>
        /// Gets or sets the number of rounds that should be used.
        /// </summary>
        public Salsa20Round Rounds { get; set; }

        /// <summary>
        /// Gets the block sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                return SalsaLegalBlockSizes;
            }
        }

        /// <summary>
        /// Gets the key sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalKeySizes
        {
            get
            {
                return SalsaLegalKeySizes;
            }
        }
#pragma warning disable CS0109

        /// <summary>
        /// Gets or sets a value indicating whether or skip a XOR operation during the transform block.
        /// </summary>
        internal bool SkipXor { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="Salsa20" /> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="Salsa20"/> class.</returns>
        public static new Salsa20 Create()
        {
            return new Salsa20();
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
            return new Salsa20Transform(rgbKey, rgbIV, (int)this.Rounds, this.SkipXor);
        }

        /// <summary>
        /// Creates a symmetric encryptor object with the <paramref name="rgbKey"/> and initialization vector <paramref name="rgbIV"/>.
        /// </summary>
        /// <param name="rgbKey">The secret key to use for the symmetric algorithm.</param>
        /// <param name="rgbIV">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>A symmetric encryptor object.</returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new Salsa20Transform(rgbKey, rgbIV, (int)this.Rounds, this.SkipXor);
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
