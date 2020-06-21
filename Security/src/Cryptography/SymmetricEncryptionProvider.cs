using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using NerdyMishka.Text;
using NerdyMishka.Util.Arrays;
using NerdyMishka.Util.Buffers;
using NerdyMishka.Util.Streams;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// A configurable symmetric encryption engine that defaults to an encrypt and
    /// then MAC scheme using AES 256 in CBC mode with PKCS7 padding and
    /// uses message authentication with HMAC-SHA-256.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     The engine writes meta information to the storage mechanism which akin
    ///     to file headers at the beginning of the bytes. This is to ensure
    ///     the data can be decrypted even if the provider changes it's implementation.
    ///     </para>
    ///     <para>
    ///     The header information includes a version number, the nonce, hash,
    ///     and allows for additional decrypted information to be stored.
    ///     </para>
    ///     <para>
    ///     A symmetric key will be stored if an implementation <see cref="IEncryptionProvider" />
    ///     is provided. This is useful for leveraging certificates that encrypt and
    ///     the decrypt the symmetric key. The decrypted key is used by the symmetric
    ///     algorithm to decrypt/encrypt the data.
    ///     </para>
    /// </remarks>
    public partial class SymmetricEncryptionProvider : ISymmetricEncryptionProvider, IDisposable
    {
        private SymmetricAlgorithm algorithm;

        private KeyedHashAlgorithm signingAlgorithm;

        private ISymmetricEncryptionProviderOptions options;

        private bool isDisposed = false;

        private bool internallyControlled = false;

        public SymmetricEncryptionProvider(ISymmetricEncryptionProviderOptions options = null)
        {
            this.options = options;
            if (this.options == null)
            {
                this.internallyControlled = true;
                this.options = new SymmetricEncryptionProviderOptions();
            }
        }

        /// <summary>
        /// Decrypts encrypted data and returns the decrypted bytes.
        /// </summary>
        /// <param name="blob">The data to encrypt.</param>
        /// <param name="privateKey">
        /// A password or phrase used to generate the key for the symmetric algorithm. If the symmetric
        /// key is stored with the message, the key for the symmetric algorithm is used instead.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        /// <returns>Encrypted bytes.</returns>
        public ReadOnlySpan<byte> Decrypt(
            ReadOnlySpan<byte> blob,
            ReadOnlySpan<byte> privateKey = default,
            IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
            var pool = ArrayPool<byte>.Shared;
            byte[] rental = null;
            byte[] signerKeyRental = null;
            byte[] symmetricKeyRental = null;
            byte[] ivRental = null;

            try
            {
                rental = pool.Rent(blob.Length);
                blob.CopyTo(rental);
                using (var reader = new MemoryStream(rental))
                using (var header = this.ReadHeader(reader, this.options, privateKey, symmetricKeyEncryptionProvider))
                {
                    this.algorithm = this.algorithm ?? CreateSymmetricAlgorithm(this.options);
                    var messageSize = blob.Length - header.HeaderSize;
                    var message = new byte[messageSize];
                    Array.Copy(rental, header.HeaderSize, message, 0, messageSize);

                    if (header.Hash != null)
                    {
                        signerKeyRental = pool.Rent(header.SigningKey.Memory.Length);
                        using (var signer = this.signingAlgorithm ?? CreateSigningAlgorithm(this.options))
                        {
                            header.SigningKey.Memory.CopyTo(signerKeyRental);
                            signer.Key = signerKeyRental;
                            var h1 = header.Hash;
                            Span<byte> h2 = signer.ComputeHash(message);

                            if (!h1.Memory.Span.SlowEquals(h2))
                            {
                                h2.Clear();
                                message.Clear();
                                return null;
                            }

                            h2.Clear();
                        }
                    }

                    symmetricKeyRental = ArrayPool<byte>.Shared.Rent(header.SymmetricKey.Memory.Length);
                    ivRental = ArrayPool<byte>.Shared.Rent(header.IvSize);
                    header.SymmetricKey.Memory.CopyTo(symmetricKeyRental);
                    header.IV.Memory.CopyTo(ivRental);
                    using (var decryptor = this.algorithm.CreateDecryptor(symmetricKeyRental, ivRental))
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
#if NET461 || NET451
                        cs.Write(message, 0, message.Length);
#else
                        cs.Write(message);
#endif
                        message.Clear();
                        cs.Flush();
                        cs.FlushFinalBlock();
                        ms.Flush();
                        return ms.ToArray();
                    }
                }
            }
            finally
            {
                if (rental != null)
                    pool.Return(rental, true);

                if (symmetricKeyRental != null)
                    pool.Return(symmetricKeyRental, true);

                if (ivRental != null)
                    pool.Return(ivRental, true);

                if (signerKeyRental != null)
                    pool.Return(signerKeyRental, true);
            }
        }

        /// <summary>
        /// Decrypts encrypted data and returns the decrypted bytes.
        /// </summary>
        /// <param name="reader">The data stream to read from.</param>
        /// <param name="writer">The data stream to write to.</param>
        /// <param name="privateKey">
        /// A password or phrase used to generate the key for the symmetric algorithm. If the symmetric
        /// key is stored with the message, the key for the symmetric algorithm is used instead.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        [SuppressMessage(
            "Design",
            "CA2000: Use dispose",
            Justification = "CryptoStream's dispose will dispose of the underlying writer")]
        public void Decrypt(
            Stream reader,
            Stream writer,
            ReadOnlySpan<byte> privateKey = default,
            IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
            Check.NotNull(nameof(reader), reader);
            Check.NotNull(nameof(writer), writer);

            var pool = ArrayPool<byte>.Shared;
            byte[] rental = null;
            byte[] signerKeyRental = null;
            byte[] symmetricKeyRental = null;
            byte[] ivRental = null;
            byte[] buffer = null;
            try
            {
                buffer = pool.Rent(4096);
                using (var header = this.ReadHeader(reader, this.options, privateKey, symmetricKeyEncryptionProvider))
                {
                    long position = reader.Position;
                    this.algorithm = this.algorithm ?? CreateSymmetricAlgorithm(this.options);

                    if (header.Hash != null)
                    {
                        using (var signer = CreateSigningAlgorithm(this.options))
                        {
                            signerKeyRental = pool.Rent(header.SigningKey.Memory.Length);
                            header.SigningKey.CopyTo(signerKeyRental);
                            signer.Key = signerKeyRental;
                            var h1 = header.Hash;

                            long bytesRead = reader.Length - header.HeaderSize;
                            reader.Seek(header.HeaderSize, SeekOrigin.Begin);
                            var h2 = signer.ComputeHash(reader);

                            if (!h1.Memory.Span.SlowEquals(h2))
                                return;
                        }
                    }

                    reader.Seek(header.HeaderSize, SeekOrigin.Begin);

                    symmetricKeyRental = pool.Rent(header.SymmetricKey.Memory.Length);
                    ivRental = pool.Rent(header.IvSize);

                    header.SymmetricKey.CopyTo(symmetricKeyRental);
                    header.IV.CopyTo(ivRental);
                    using (var decryptor = this.algorithm.CreateDecryptor(symmetricKeyRental, ivRental))
                    {
                        // TODO: create a sudo stream that breaks a dispose call.
                        var cs = new CryptoStream(writer, decryptor, CryptoStreamMode.Write);

                        long bytesRead = reader.Length - header.HeaderSize;
                        while (bytesRead > 0)
                        {
                            int read = reader.Read(buffer, 0, buffer.Length);
                            bytesRead -= read;
                            cs.Write(buffer, 0, read);
                        }

                        cs.Flush();
                        cs.FlushFinalBlock();
                        writer.Flush();
                    }
                }
            }
            finally
            {
                if (buffer != null)
                    pool.Return(buffer, true);

                if (rental != null)
                    pool.Return(rental, true);

                if (symmetricKeyRental != null)
                    pool.Return(symmetricKeyRental, true);

                if (ivRental != null)
                    pool.Return(ivRental, true);

                if (signerKeyRental != null)
                    pool.Return(signerKeyRental, true);
            }
        }

        public byte[] DecryptBytes(
           byte[] blob,
           byte[] privateKey = default,
           IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
#if NET461 || NET451
            var span = Decrypt(
                blob.AsSpan(),
                privateKey.AsSpan(),
                symmetricKeyEncryptionProvider);

            return span.ToArray();
#else
            var span = this.Decrypt(blob, privateKey, symmetricKeyEncryptionProvider);
            return span.ToArray();
#endif
        }

        public byte[] EncryptBytes(
           byte[] blob,
           byte[] privateKey = default,
           byte[] symmetricKey = default,
           IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
#if NET461 || NET451
            var span = Encrypt(
                blob.AsSpan(),
                privateKey.AsSpan(),
                symmetricKey.AsSpan(),
                symmetricKeyEncryptionProvider);

            return span.ToArray();
#else
            var span = this.Encrypt(blob, privateKey, symmetricKey, symmetricKeyEncryptionProvider);
            return span.ToArray();
#endif
        }

        /// <summary>
        /// Encrypts the data and returns the encrypted bytes.
        /// </summary>
        /// <param name="blob">The data to encrypt.</param>
        /// <param name="privateKey">
        ///  A password or phrase used to generate the key for the symmetric algorithm.
        /// </param>
        /// <param name="symmetricKey">
        ///  The key for the symmetric algorithm. If used, the private key is ignored
        ///  and the symmetric key is stored with the message.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to encrypt/decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        /// <returns>Encrypted bytes.</returns>
        public ReadOnlySpan<byte> Encrypt(
            ReadOnlySpan<byte> blob,
            ReadOnlySpan<byte> privateKey = default,
            ReadOnlySpan<byte> symmetricKey = default,
            IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
            if (blob == null)
                throw new ArgumentNullException(nameof(blob));

            byte[] symmetricKeyRental = null;
            byte[] ivRental = null;
            byte[] headerRental = null;
            byte[] signingKeyRental = null;
            ArrayPool<byte> pool = ArrayPool<byte>.Shared;
            try
            {
                using (var header = this.GenerateHeader(this.options, symmetricKey, privateKey, null, symmetricKeyEncryptionProvider))
                {
                    byte[] encryptedBlob = null;
                    symmetricKeyRental = pool.Rent(header.SymmetricKey.Memory.Length);
                    ivRental = pool.Rent(header.IvSize);
                    header.SymmetricKey.Memory.CopyTo(symmetricKeyRental);
                    header.IV.Memory.CopyTo(ivRental);

                    this.algorithm = this.algorithm ?? CreateSymmetricAlgorithm(this.options);
                    using (var encryptor = this.algorithm.CreateEncryptor(symmetricKeyRental, ivRental))
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
#if NET461 || NET451
                        var next = ArrayPool<byte>.Shared.Rent(blob.Length);
                        blob.CopyTo(next);
                        cs.Write(next, 0, next.Length);
                        next.Clear();
                        ArrayPool<byte>.Shared.Return(next);
#else
                        cs.Write(blob);
#endif
                        cs.Flush();
                        cs.FlushFinalBlock();
                        ms.Flush();
                        encryptedBlob = ms.ToArray();
                    }

                    headerRental = pool.Rent(header.HeaderSize);
                    header.Bytes.Memory.CopyTo(headerRental);

                    if (!this.options.SkipSigning && header.SigningKey != null && !header.SigningKey.Memory.IsEmpty)
                    {
                        signingKeyRental = pool.Rent(header.SigningKey.Memory.Length);
                        this.signingAlgorithm = this.signingAlgorithm ?? CreateSigningAlgorithm(this.options);

                        header.SigningKey.Memory.CopyTo(signingKeyRental);
                        this.signingAlgorithm.Key = signingKeyRental;
                        var hash = this.signingAlgorithm.ComputeHash(encryptedBlob);

                        Array.Copy(hash, 0, headerRental, header.Position, hash.Length);

                        hash.Clear();
                        hash = null;
                    }

                    using (var ms = new MemoryStream())
                    {
                        using (var writer = new BinaryWriter(ms, Utf8Options.NoBom, true))
                        {
                            writer.Write(headerRental, 0, header.HeaderSize);
                        }

#if NET461 || NET451
                        ms.Write(encryptedBlob, 0, encryptedBlob.Length);
#else
                        ms.Write(encryptedBlob);
#endif
                        encryptedBlob.Clear();
                        ms.Flush();
                        return ms.ToArray();
                    }
                }
            }
            finally
            {
                if (symmetricKeyRental != null)
                    pool.Return(symmetricKeyRental, true);

                if (ivRental != null)
                    pool.Return(ivRental, true);

                if (headerRental != null)
                    pool.Return(headerRental, true);

                if (signingKeyRental != null)
                    pool.Return(signingKeyRental, true);
            }
        }

        /// <summary>
        /// Encrypts the data and returns the encrypted bytes.
        /// </summary>
        /// <param name="reader">The data stream to read from.</param>
        /// <param name="writer">The data stream to write to.</param>
        /// <param name="privateKey">
        ///  A password or phrase used to generate the key for the symmetric algorithm.
        /// </param>
        /// <param name="symmetricKey">
        ///  The key for the symmetric algorithm. If used, the private key is ignored
        ///  and the symmetric key is stored with the message.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to encrypt/decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        [SuppressMessage(
            "Design",
            "CA2000: Use dispose",
            Justification = "CryptoStream's dispose will dispose of the underlying writer")]
        public void Encrypt(
            Stream reader,
            Stream writer,
            ReadOnlySpan<byte> privateKey = default,
            ReadOnlySpan<byte> symmetricKey = default,
            IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
            Check.NotNull(nameof(reader), reader);
            Check.NotNull(nameof(writer), writer);

            byte[] symmetricKeyRental = null;
            byte[] ivRental = null;
            byte[] headerRental = null;
            byte[] signingKeyRental = null;
            byte[] buffer = null;
            ArrayPool<byte> pool = ArrayPool<byte>.Shared;
            try
            {
                buffer = pool.Rent(4096);
                using (var header = this.GenerateHeader(this.options, symmetricKey, privateKey, null, symmetricKeyEncryptionProvider))
                {
                    symmetricKeyRental = pool.Rent(header.SymmetricKey.Memory.Length);
                    ivRental = pool.Rent(header.IvSize);
                    header.SymmetricKey.Memory.CopyTo(symmetricKeyRental);
                    header.IV.Memory.CopyTo(ivRental);
                    this.algorithm = this.algorithm ?? CreateSymmetricAlgorithm(this.options);

                    using (var encryptor = this.algorithm.CreateEncryptor(symmetricKeyRental, ivRental))
                    {
                        var cs = new CryptoStream(writer, encryptor, CryptoStreamMode.Write);
                        using (var bw = new BinaryWriter(writer, Utf8Options.NoBom, true))
                        {
                            bw.Write(new byte[header.HeaderSize]);
                            bw.Flush();
                        }

                        long bytesLeft = reader.Length;

                        while (bytesLeft > 0)
                        {
                            int read = reader.Read(buffer, 0, buffer.Length);
                            bytesLeft -= read;
                            cs.Write(buffer, 0, read);
                        }

                        cs.Flush();
                        cs.FlushFinalBlock();
                        writer.Flush();
                    }

                    headerRental = pool.Rent(header.HeaderSize);
                    header.Bytes.Memory.CopyTo(headerRental);

                    if (!this.options.SkipSigning && header.SigningKey != null && !header.SigningKey.Memory.IsEmpty)
                    {
                        signingKeyRental = pool.Rent(header.SigningKey.Memory.Length);
                        header.SigningKey.CopyTo(signingKeyRental);
                        using (var signer = CreateSigningAlgorithm(this.options))
                        {
                            signer.Key = signingKeyRental;

                            writer.Seek(header.HeaderSize, SeekOrigin.Begin);
                            var hash = signer.ComputeHash(writer);

                            Array.Copy(hash, 0, headerRental, header.Position, hash.Length);
                            hash.Clear();
                        }
                    }

                    writer.Seek(0, SeekOrigin.Begin);
                    using (var bw = new BinaryWriter(writer, Utf8Options.NoBom, true))
                    {
                        bw.Write(headerRental, 0, header.HeaderSize);
                        writer.Flush();
                        writer.Seek(0, SeekOrigin.End);
                    }
                }
            }
            finally
            {
                if (buffer != null)
                    pool.Return(buffer, true);

                if (symmetricKeyRental != null)
                    pool.Return(symmetricKeyRental, true);

                if (ivRental != null)
                    pool.Return(ivRental, true);

                if (headerRental != null)
                    pool.Return(headerRental, true);

                if (signingKeyRental != null)
                    pool.Return(signingKeyRental, true);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                this.algorithm?.Dispose();
                this.signingAlgorithm?.Dispose();

                if (this.internallyControlled)
                    this.options?.Dispose();
            }

            this.isDisposed = true;
        }
    }
}