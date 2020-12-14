using System;
using System.Buffers;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// Options for <see cref="ISymmetricEncryptionProvider"/>.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="IMemoryOwner{byte}" /> is used instead of
    ///         <see cref="byte[]" /> to control lifetime, work with
    ///         async code, and hopefully minimize allocations.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="ISymmetricEncryptionProvider" />
    public interface ISymmetricEncryptionProviderOptions : IDisposable
    {
        int KeySize { get; set; }

        int BlockSize { get; set; }

        CipherMode Mode { get; set; }

        PaddingMode Padding { get; set; }

        SymmetricAlgorithmType SymmetricAlgorithm { get; set; }

        KeyedHashAlgorithmType KeyedHashedAlgorithm { get; set; }

        int SaltSize { get; set; }

        int Iterations { get; set; }

        int MinimumPrivateKeyLength { get; set; }

        bool SkipSigning { get; set; }

        IMemoryOwner<byte> Key { get; }

        IMemoryOwner<byte> SigningKey { get; }

        void SetKey(ReadOnlySpan<byte> key);

        void SetSigningKey(ReadOnlySpan<byte> signingKey);
    }
}