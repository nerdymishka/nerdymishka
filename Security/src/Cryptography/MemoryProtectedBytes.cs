using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using NerdyMishka.Util.Collections;

namespace NerdyMishka.Security.Cryptography
{
    [SuppressMessage(
        "Microsoft.Design",
        "CA1819: Properties should not return arrays",
        Justification = "Arrays are only publically ")]
    public class MemoryProtectedBytes : IEquatable<MemoryProtectedBytes>, IDisposable
    {
        private static MemoryProtectionAction s_defaultAction = ChaCha20Instance.Generate();

        private static int s_blockGrowth = -1;

        private byte[] data;

        private byte[] hash;

        private byte[] iv;

        private byte[] key;

        private int hashCode = 0;

        private bool isDisposed = false;

        public MemoryProtectedBytes()
        {
            this.data = Array.Empty<byte>();
            this.hash = Array.Empty<byte>();
            this.IV = NonceFactory.Generate();
        }

        public MemoryProtectedBytes(byte[] bytes, byte[] key, byte[] iv, bool encrypt = true)
        {
            Check.NotNull(nameof(bytes), bytes);
            Check.NotNull(nameof(key), key);
            Check.NotNull(nameof(iv), iv);

            this.key = new byte[key.Length];
            this.iv = new byte[iv.Length];
            Array.Copy(key, this.key, key.Length);
            Array.Copy(iv, this.iv, iv.Length);
            this.Init(bytes, encrypt);
        }

        public MemoryProtectedBytes(byte[] bytes, bool encrypt = true)
        {
            Check.NotNull(nameof(bytes), bytes);
            this.Init(bytes, encrypt);
        }

        ~MemoryProtectedBytes()
        {
            this.Dispose(false);
        }

        public bool IsReadOnly { get; protected set; }

        public bool IsProtected { get; protected set; }

        public int Length { get; protected set; }

        public byte[] Hash => this.hash;

        public byte[] IV
        {
            get => this.iv?.ToArray();
            set
            {
                this.iv = value;
            }
        }

        public byte[] Key
        {
            get => this.key?.ToArray();
            set
            {
                this.key = value;
            }
        }

        public static bool operator ==(MemoryProtectedBytes left, MemoryProtectedBytes right)
        {
            if (left == null)
            {
                if (right == null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(MemoryProtectedBytes left, MemoryProtectedBytes right)
        {
            if (left == null)
            {
                if (right == null)
                    return true;

                return false;
            }

            return !left.Equals(right);
        }

        public static void SetDefaultMemoryProtectionAction(MemoryProtectionAction action, int blockGrowth = -1)
        {
            ChaCha20Instance.Dispose();
            s_defaultAction = action;
            s_blockGrowth = blockGrowth;
        }

        public void CopyTo(byte[] array)
        {
            this.CheckDisposed();
            Check.NotNull(nameof(array), array);

            var decrypted = this.Decrypt(this.data);
            var l = Math.Min(this.Length, array.Length);

            Array.Copy(decrypted, array, l);
            Array.Clear(decrypted, 0, decrypted.Length);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object other)
        {
            this.CheckDisposed();

            if (other == null)
                return false;

            if (other is MemoryProtectedBytes bytes)
                return this.Equals(bytes);

            return false;
        }

        public bool Equals(MemoryProtectedBytes other)
        {
            this.CheckDisposed();

            if (other == null)
                return false;

            if (this.IsProtected != other.IsProtected)
                return false;

            if (this.Length != other.Length)
                return false;

            if (!this.IV.EqualTo(other.IV))
                return false;

            if (this.hash == null && other.hash == null)
                return true;

            return this.hash.EqualTo(other.hash);
        }

        public override int GetHashCode()
        {
            this.CheckDisposed();

            // TODO: replace with MurMurHash3
            if (this.hashCode > 0)
                return this.hashCode;

            this.hashCode = this.hash.GetHashCode() * 7;
            return this.hashCode;
        }

        public virtual byte[] ToArray()
        {
            this.CheckDisposed();

            var copy = new byte[this.Length];
            this.CopyTo(copy);
            return copy;
        }

        protected virtual byte[] Encrypt(byte[] bytes)
        {
            if (!this.IsProtected || bytes == null || bytes.Length == 0)
                return bytes;

            return s_defaultAction(bytes, this, MemoryProtectionActionType.Encrypt);
        }

        protected virtual byte[] Decrypt()
        {
            return this.Decrypt(this.data);
        }

        protected virtual byte[] Decrypt(byte[] bytes)
        {
            if (!this.IsProtected || bytes == null || bytes.Length == 0)
                return bytes;

            return s_defaultAction(bytes, this, MemoryProtectionActionType.Decrypt);
        }

        protected virtual void UpdateHash(byte[] bytes)
        {
            Check.NotNull(nameof(bytes), bytes);

            using (var sha = SHA256.Create())
            {
                this.hash = sha.ComputeHash(bytes);
            }
        }

        protected virtual void Update(byte[] decryptedBytes)
        {
            Check.NotNull(nameof(decryptedBytes), decryptedBytes);

            this.Length = decryptedBytes.Length;
            this.UpdateHash(decryptedBytes);

            // required for certain encryption types such as
            // DPAPI. DPAPI requires blocks of 16.
            if (s_blockGrowth > 0)
            {
                var bytes = Grow(decryptedBytes, s_blockGrowth);
                this.data = this.Encrypt(bytes);
                Array.Clear(bytes, 0, bytes.Length);
                return;
            }

            this.data = this.Encrypt(decryptedBytes);
        }

        protected void Init(byte[] bytes, bool encrypt = true)
        {
            if (this.iv == null)
                this.iv = NonceFactory.Generate();

            this.IsProtected = encrypt;
            this.Update(bytes);
        }

        protected virtual void CheckDisposed()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException($"{nameof(MemoryProtectedBytes)} - {this.IV}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                if (this.data != null)
                    Array.Clear(this.data, 0, this.data.Length);

                if (this.hash != null)
                    Array.Clear(this.hash, 0, this.hash.Length);

                if (this.Key != null)
                {
                    Array.Clear(this.Key, 0, this.Key.Length);
                    Array.Clear(this.IV, 0, this.IV.Length);
                }

                this.data = null;
                this.hash = null;
                this.Key = null;
                this.IV = null; // reference in NonceFactory
            }

            this.isDisposed = true;
        }

        private static byte[] Grow(byte[] binary, int blockSize = 16)
        {
            return Grow(binary, binary.Length, blockSize);
        }

        private static byte[] Grow(byte[] binary, int length, int blockSize = 16)
        {
            int blocks = binary.Length / blockSize;
            int size = blocks * blockSize;
            if (size <= length)
            {
                while (size < length)
                {
                    blocks++;
                    size = blocks * blockSize;
                }
            }

            byte[] result = new byte[blocks * blockSize];
            Array.Copy(binary, result, binary.Length);
            return result;
        }
    }
}