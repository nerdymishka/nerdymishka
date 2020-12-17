using System;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public class PasswordAuthenticator : IPasswordAuthenticator
    {
        private readonly PasswordAuthenticatorOptions options;

        public PasswordAuthenticator(PasswordAuthenticatorOptions options = null)
        {
            this.options = options ?? new PasswordAuthenticatorOptions();
            if (this.options.SaltSize < 8)
                throw new ArgumentOutOfRangeException(nameof(this.options.SaltSize), "SaltSize must be 8 or greater");
        }

        public int HashSize => 8 + (this.options.SaltSize)  + this.options.OutputSize;

        public byte[] ComputeHash(byte[] value)
        {
            var o = this.options;
            Span<byte> salt = stackalloc byte[o.SaltSize];
            using var csprng = new RNGCryptoServiceProvider();
            csprng.GetBytes(salt);
            
            var hash = Pbkdf2(value, salt, o.Iterations, o.OutputSize);
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            var size = (short) salt.Length;

            writer.Write(this.options.HashType); // 2
            writer.Write(size); // 2
            writer.Write(this.options.Iterations); // 4
            writer.Write(salt); // var 1 (8 by default)
            writer.Write(hash); // var 2 (32 by default) 
            writer.Flush();
            ms.Flush();

            return ms.ToArray();
        }

        public bool TryComputeHash(ReadOnlySpan<byte> input, Span<byte> output, out int bytesWritten)
        {
            bytesWritten = 0;
            if (output.Length != this.HashSize)
                return false;

            var o = this.options;
            Span<byte> salt = stackalloc byte[o.SaltSize];
            
            using var csprng = new RNGCryptoServiceProvider();
            csprng.GetBytes(salt);
           

            try
            {
                var hash = Pbkdf2(input, salt, o.Iterations, o.OutputSize);
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                var size = (short) salt.Length;

                writer.Write(this.options.HashType); // 2
                writer.Write(size); // 2
                writer.Write(this.options.Iterations); // 4
                writer.Write(salt); // var 1 (8 by default)
                writer.Write(hash); // var 2 (32 by default)
                writer.Flush();
                ms.Flush();

                ms.ToArray().CopyTo(output);
                bytesWritten = this.HashSize;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Verify(ReadOnlySpan<byte> rawValue, ReadOnlySpan<byte> hash)
        {
            var o = this.options;
            using var ms = new MemoryStream();
            ms.Write(hash);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            using var reader = new BinaryReader(ms);

            // in case one wants to switch to bcrypt or another 
            // implementation later, save the hash type.
            var hashType = reader.ReadInt16(); // 2
            var size = reader.ReadInt16(); // 2
            var iterations = reader.ReadInt32(); // 4
            var salt = reader.ReadBytes(size); // variable

            var actualHash = reader.ReadBytes(hash.Length - (size + 8));
            var attemptedHash = Pbkdf2(rawValue, salt, o.Iterations, o.OutputSize);

            return EncryptionUtil.SlowEquals(attemptedHash, actualHash);
        }

        public static ReadOnlySpan<byte> Pbkdf2(ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterations = 64000, int outputBytes = 32)
        {
            using var pbkdf2 = new NerdyRfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(outputBytes);
        }

        public class PasswordAuthenticatorOptions
        {
            public int Iterations { get; set; } = 64000;

            public short HashType { get; set; } = 1;

            public short SaltSize { get; set; } = 8;

            public int OutputSize { get; set; } = 32;
        }
    }
}
