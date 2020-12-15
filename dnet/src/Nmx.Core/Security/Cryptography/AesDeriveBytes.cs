using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// Generates bytes using <see cref="Aes" /> over multiple iterations to tranform
    /// the initial password. The initial state is designed to conform to KeePass's AES-KDF.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///      The implementation of <see cref="AesDervieBytes" /> is based on <see cref="PasswordDeriveBytes" />
    ///      and KeePass's AES-KDF.
    ///     </para>
    /// </remarks>
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1819: Properties should not return arrays",
        Justification = "Arrays return copies")]
    public class AesDeriveBytes : DeriveBytes
    {
        private const int MinimumKeySize = 8;

        private readonly int blockSize;

        private readonly byte[] password;

        private byte[] key;

        private byte[] iv;

        private uint iterations;

        private HMAC hmac;

        private byte[] buffer;

        private byte[] block;

        private int startIndex;

        private int endIndex;

        private Aes aes;

        private ICryptoTransform transform;

        public AesDeriveBytes(byte[] password)
            : this(password, null, null, 100000, HashAlgorithmName.SHA256)
        {
        }

        public AesDeriveBytes(byte[] password, byte[] key, long iterations = 10000)
            : this(password, key, null, iterations, HashAlgorithmName.SHA256)
        {
        }

        public AesDeriveBytes(byte[] password, byte[] key, byte[] iv, long iterations, HashAlgorithmName hashAlgorithm)
        {
            if (key != null && key.Length != 0 && key.Length < MinimumKeySize)
                throw new ArgumentException($"salt minimum length is {MinimumKeySize}", nameof(key));

            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations), "iterations must be a positive number");

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            this.key = new byte[32];
            if (this.key == null || this.key.Length == 0)
            {
                using (var rng = new NerdyRandomNumberGenerator())
                {
                    rng.NextBytes(this.key);
                }
            }
            else
            {
                Array.Copy(key, this.key, this.key.Length);
            }

            this.iv = new byte[16];
            Array.Clear(this.iv, 0, this.iv.Length);

            if (iv != null && iv.Length > 0)
                Array.Copy(iv, this.iv, iv.Length);

            this.password = (byte[])password.Clone();
            this.HashAlgorithm = hashAlgorithm;
            this.hmac = this.CreateHmac();
            this.blockSize = this.hmac.HashSize >> 3;
            this.Initialize();
        }

        public AesDeriveBytes(byte[] password, long iterations, HashAlgorithmName hashAlgorithm)
        {
            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations), "iterations must be a positive number");

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            this.key = new byte[MinimumKeySize];
            using (var rng = new NerdyRandomNumberGenerator())
            {
                rng.NextBytes(this.key);
            }

            this.iv = new byte[16];
            Array.Clear(this.iv, 0, this.iv.Length);

            this.password = (byte[])password.Clone();
            this.HashAlgorithm = hashAlgorithm;
            this.hmac = this.CreateHmac();
            this.blockSize = this.hmac.HashSize >> 3;
            this.Initialize();
        }

        public HashAlgorithmName HashAlgorithm { get; }

        public byte[] Key
        {
            get
            {
                return this.key.ToArray();
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length < MinimumKeySize)
                    throw new ArgumentException($"key size must be {MinimumKeySize}");

                this.key = new byte[32];
                Array.Copy(value, this.key, value.Length);

                this.Initialize();
            }
        }

        public byte[] IV
        {
            get
            {
                return this.iv.ToArray();
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                this.iv = new byte[16];
                Array.Copy(value, this.iv, value.Length);
            }
        }

        public long Iterations
        {
            get => (int)this.iterations;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value must a positive number");
                this.iterations = (uint)value;

                this.Initialize();
            }
        }

        public byte[] GetBytes()
        {
            return this.GetBytes(32);
        }

        public override byte[] GetBytes(int cb)
        {
            if (cb <= 0)
                throw new ArgumentOutOfRangeException(nameof(cb), "cb must be positive number");

            byte[] password = new byte[cb];
            int offset = 0;
            int size = this.endIndex - this.startIndex;

            if (size > 0)
            {
                if (cb >= size)
                {
                    Buffer.BlockCopy(this.buffer, this.startIndex, password, 0, size);
                    this.startIndex = this.endIndex = 0;
                    offset += size;
                }
                else
                {
                    Buffer.BlockCopy(this.buffer, this.startIndex, password, 0, cb);
                    this.startIndex += cb;
                    return password;
                }
            }

            Debug.Assert(this.startIndex == 0 && this.endIndex == 0, "Invalid start or end index in the internal buffer.");

            while (offset < cb)
            {
                this.Func();
                int remainder = cb - offset;
                if (remainder >= this.blockSize)
                {
                    Buffer.BlockCopy(this.buffer, 0, password, offset, this.blockSize);
                    offset += this.blockSize;
                }
                else
                {
                    Buffer.BlockCopy(this.buffer, 0, password, offset, remainder);
                    this.startIndex = remainder;
                    this.endIndex = this.buffer.Length;
                    return password;
                }
            }

            return password;
        }

        public override void Reset()
        {
            this.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.hmac != null)
                {
                    this.hmac.Dispose();
                    this.hmac = null;
                }

                if (this.transform != null)
                {
                    this.transform.Dispose();
                    this.transform = null;
                }

                if (this.aes != null)
                {
                    this.aes.Dispose();
                    this.aes = null;
                }

                if (this.block != null)
                    Array.Clear(this.block, 0, this.block.Length);

                if (this.buffer != null)
                    Array.Clear(this.buffer, 0, this.buffer.Length);

                if (this.password != null)
                    Array.Clear(this.password, 0, this.password.Length);

                if (this.key != null)
                    Array.Clear(this.key, 0, this.key.Length);

                if (this.iv != null)
                    Array.Clear(this.iv, 0, this.iv.Length);
            }

            base.Dispose(disposing);
        }

        private void Func()
        {
            for (var i = 0; i < this.Iterations; ++i)
            {
                this.transform.TransformBlock(this.block, 0, 16, this.block, 0);
                this.transform.TransformBlock(this.block, 16, 16, this.block, 16);
            }

            var hash = this.hmac.ComputeHash(this.block);
            Array.Copy(hash, this.buffer, this.blockSize);
            Array.Clear(hash, 0, hash.Length);
        }

        [SuppressMessage("Microsoft.Security", "SCS0012:", Justification = "Required")]
        private void Initialize()
        {
            if (this.block != null)
                Array.Clear(this.block, 0, this.block.Length);

            if (this.buffer != null)
                Array.Clear(this.buffer, 0, this.buffer.Length);

            this.block = new byte[32];
            this.buffer = new byte[this.blockSize];
            this.startIndex = this.endIndex = 0;

            var l = Math.Min(this.password.Length, this.block.Length);
            Array.Copy(this.password, this.block, l);

            if (this.aes == null)
            {
                var aes = this.aes = Aes.Create();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = this.iv;
                aes.Key = this.key;
                aes.Mode = CipherMode.ECB;
            }

            if (this.transform != null)
                this.transform.Dispose();

            this.transform = this.aes.CreateEncryptor();
        }

        private HMAC CreateHmac()
        {
            HashAlgorithmName hashAlgorithm = this.HashAlgorithm;

            if (string.IsNullOrEmpty(hashAlgorithm.Name))
                throw new CryptographicException("HashAlgorithm is null or empty");

            if (hashAlgorithm == HashAlgorithmName.SHA256)
                return new HMACSHA256();
            if (hashAlgorithm == HashAlgorithmName.SHA384)
                return new HMACSHA384();
            if (hashAlgorithm == HashAlgorithmName.SHA512)
                return new HMACSHA512();

            throw new CryptographicException($"Unkown Algorithm {hashAlgorithm.Name}");
        }
    }
}