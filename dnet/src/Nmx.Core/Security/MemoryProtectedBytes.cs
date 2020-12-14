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

        private static long s_counter;

        private readonly long id;

        private byte[] data;

        private int hashCode;

        private bool isDisposed;

        public MemoryProtectedBytes()
        {
            System.Threading.Interlocked.Increment(ref s_counter);
            this.id = s_counter;
            this.data = Array.Empty<byte>();
            this.Hash = Array.Empty<byte>();
        }

        public MemoryProtectedBytes(ReadOnlySpan<byte> bytes, bool encrypt = true)
            : this()
        {
            this.Init(bytes, encrypt);
        }

        ~MemoryProtectedBytes()
        {
            this.Dispose(false);
        }

        public long Id => this.id;

        public bool IsReadOnly { get; protected set; }

        public bool IsProtected { get; protected set; }

        public int Length { get; protected set; }

        public ReadOnlyMemory<byte> Hash { get; protected set; }

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
            Check.ArgNotNull(array, nameof(array));
            var l = Math.Min(this.Length, array.Length);

            var decrypted = this.Decrypt(this.data);
            if (l < decrypted.Length)
                throw new ArgumentOutOfRangeException(nameof(array));

            decrypted.CopyTo(array);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object other)
        {
            this.CheckDisposed();

            if (other is MemoryProtectedBytes bytes)
                return this.Equals(bytes);

            return false;
        }

        public bool Equals(ReadOnlySpan<byte> bytes)
        {
            if (bytes.IsEmpty && (this.data == null || this.data.Length == 0))
                return true;

            var hash = EncryptionUtil.ComputeChecksum(bytes, HashAlgorithmName.SHA256);
            return hash.SequenceEqual(this.Hash.Span);
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

            if (this.Hash.IsEmpty && other.Hash.IsEmpty)
                return true;

            return this.Hash.Equals(other.Hash);
        }

        public override int GetHashCode()
        {
            this.CheckDisposed();

            // TODO: replace with MurMurHash3
            if (this.hashCode > 0)
                return this.hashCode;

            this.hashCode = this.Hash.GetHashCode() * 7;
            return this.hashCode;
        }

        public virtual byte[] ToArray()
        {
            this.CheckDisposed();

            var copy = new byte[this.Length];
            this.CopyTo(copy);
            return copy;
        }

        protected virtual ReadOnlySpan<byte> Encrypt(ReadOnlySpan<byte> bytes)
        {
            if (!this.IsProtected || bytes.Length == 0)
                return bytes;

            return s_defaultAction(bytes, this, MemoryProtectionActionType.Encrypt);
        }

        protected virtual ReadOnlySpan<byte> Decrypt()
        {
            return this.Decrypt(this.data.AsSpan());
        }

        protected virtual ReadOnlySpan<byte> Decrypt(ReadOnlySpan<byte> bytes)
        {
            if (!this.IsProtected || bytes == null || bytes.Length == 0)
                return bytes;

            return s_defaultAction(bytes, this, MemoryProtectionActionType.Decrypt);
        }

        protected virtual void UpdateHash(ReadOnlySpan<byte> bytes)
        {
            EncryptionUtil.ComputeChecksum(bytes, HashAlgorithmName.SHA256);
        }

        protected virtual void Update(ReadOnlySpan<byte> decryptedBytes)
        {
            this.Length = decryptedBytes.Length;
            this.UpdateHash(decryptedBytes);

            // required for certain encryption types such as
            // DPAPI. DPAPI requires blocks of 16.
            if (s_blockGrowth > 0)
            {
                decryptedBytes = Grow(decryptedBytes, s_blockGrowth);
            }

            this.data = this.Encrypt(decryptedBytes)
                            .ToArray();
        }

        protected void Init(ReadOnlySpan<byte> bytes, bool encrypt = true)
        {
            this.IsProtected = encrypt;
            this.Update(bytes);
        }

        protected virtual void CheckDisposed()
        {
            if (this.isDisposed)
                throw new ObjectDisposedException($"{nameof(MemoryProtectedBytes)}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                if (this.data != null)
                    Array.Clear(this.data, 0, this.data.Length);

                this.data = null;
                this.Hash = null;
            }

            this.isDisposed = true;
        }

        private static ReadOnlySpan<byte> Grow(ReadOnlySpan<byte> binary, int blockSize = 16)
        {
            return Grow(binary, binary.Length, blockSize);
        }

        private static ReadOnlySpan<byte> Grow(ReadOnlySpan<byte> binary, int length, int blockSize = 16)
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

            Span<byte> span = new byte[blocks * blockSize];
            binary.CopyTo(span);
            return span;
        }
    }
}