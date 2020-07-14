using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using NerdyMishka.Api.KeePass.Package;
using NerdyMishka.Security.Cryptography;
using NerdyMishka.Util.Collections;
using NerdyMishka.Util.Streams;

namespace NerdyMishka.Api.KeePass
{
    public class KeePassPackage : IKeePassPackage
    {
        private bool isDisposed = false;

        private KeePassCompositeKey compositeKey;

        public KeePassPackage()
        {
            this.compositeKey = new KeePassCompositeKey();
            this.CompositeKey = this.compositeKey;
            this.CustomData = new CustomDataDictionary();
        }

        ~KeePassPackage()
        {
            this.Dispose(false);
        }

        public CompositeKey CompositeKey
        {
            get => this.compositeKey;
            internal protected set
            {
                if (value is KeePassCompositeKey key)
                    this.compositeKey = key;
                else
                    throw new NotSupportedException();
            }
        }

        public HeaderInfo HeaderInfo { get; internal protected set; }

        public PackageInfo MetaInfo { get; internal protected set; }

        public IKeePassDocument Document { get; internal protected set; }

        public CustomDataDictionary CustomData { get; }

        public MemoryProtectedBytesMap BinaryMap { get; protected internal set; }

        MemoryProtectedBytesMap IKeePassPackage.BinaryMap
        {
            get => this.BinaryMap;
            set => this.BinaryMap = value;
        }

        public static void Save(KeePassPackage package, KeePassCompositeKey key, Stream stream)
        {
            if (package is null)
                throw new ArgumentNullException(nameof(package));

            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var actualByteMarks = package.HeaderInfo.StreamStartByteMarker;
            var h = package.HeaderInfo;
            h.WriteToStream(stream);

            stream.Write(actualByteMarks);

            var symmetricKey = GetSymmetricKey(
                key,
                h.PackageCipherKey.Span,
                h.AesKdfPassword.Span,
                h.AesKdfIterations)
                .ToArray();
            try
            {
                var factory = new Package.CryptoStreamFactory();
                var provider = factory.Find(h.PackageCipherId);

                var iv = h.PackageCipherIV.ToArray();
                using (var outerStream = provider.CreateCryptoStream(stream, false, symmetricKey, iv))
                using (var blockStream = new HMACBlockStream(outerStream, true))
                {
                    var serializer = new Serialization.KeePassPackageXmlSerializer();
                    if (h.PackageCompression == 1)
                    {
                        using (var compressedStream = new GZipStream(blockStream, CompressionMode.Compress))
                        {
                            serializer.Write(package, compressedStream);
                        }
                    }
                    else
                    {
                        serializer.Write(package, blockStream);
                    }
                }
            }
            finally
            {
                Array.Clear(symmetricKey, 0, symmetricKey.Length);
            }
        }

        public static void Open(
            KeePassPackage package,
            KeePassCompositeKey key,
            Stream stream)
        {
            if (package is null)
                throw new ArgumentNullException(nameof(package));

            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var h = package.HeaderInfo;
            h.ReadFromStream(stream);

            var symmetricKey = GetSymmetricKey(
                key,
                h.PackageCipherKey.Span,
                h.AesKdfPassword.Span,
                h.AesKdfIterations)
                .ToArray();
            try
            {
                var factory = new Package.CryptoStreamFactory();
                var provider = factory.Find(h.PackageCipherId);

                var iv = h.PackageCipherIV.ToArray();
                using (var outerStream = provider.CreateCryptoStream(stream, false, symmetricKey, iv))
                {
                    byte[] expectedHeaderByteMark = h.StreamStartByteMarker.ToArray();
                    byte[] actualHeaderByteMark = outerStream.ReadBytes(32);
                    bool equal = expectedHeaderByteMark.EqualTo(actualHeaderByteMark);
                    if (!equal)
                        throw new Exception("Invalid File Format");

                    using (var blockStream = new HMACBlockStream(outerStream, false))
                    {
                        var serializer = new Serialization.KeePassPackageXmlSerializer();
                        if (h.PackageCompression == 1)
                        {
                            using (var compressedStream = new GZipStream(blockStream, CompressionMode.Decompress))
                            {
                                serializer.Read(package, compressedStream);
                            }
                        }
                        else
                        {
                            serializer.Read(package, blockStream);
                        }
                    }
                }
            }
            finally
            {
                Array.Clear(symmetricKey, 0, symmetricKey.Length);
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
                if (this.compositeKey != null)
                    this.compositeKey.Dispose();
            }

            this.isDisposed = true;
        }

        private static ReadOnlySpan<byte> GetSymmetricKey(
            KeePassCompositeKey key,
            ReadOnlySpan<byte> cipherKeySeed,
            ReadOnlySpan<byte> masterKeyHash,
            long iterations)
        {
            using (var ms = new MemoryStream())
            {
                var bytes = CreateSymmetricKey(key, masterKeyHash, iterations);
                if (bytes.Length != 32)
                    throw new Exception("Invalid ComposteKey or derived bytes");

#if NETSTANDARD2_0
                StreamExtensions.Write(ms, cipherKeySeed);
                StreamExtensions.Write(ms, bytes);
#else
                ms.Write(cipherKeySeed);
                ms.Write(bytes);
#endif
                using (var sha = SHA256.Create())
                {
                    var symmetricKey = ms.ToArray();
                    var result = sha.ComputeHash(symmetricKey);
                    Array.Clear(symmetricKey, 0, symmetricKey.Length);

                    return result;
                }
            }
        }

        private static ReadOnlySpan<byte> CreateSymmetricKey(
            KeePassCompositeKey key,
            ReadOnlySpan<byte> symmetricKey,
            long iterations)
        {
            const int size = 32;
            var raw = key.AssembleKey().ToArray();
            if (raw == null || raw.Length != size)
                return Array.Empty<byte>();

            try
            {
                // TODO: add argon2 support
                var pdbKey = symmetricKey.ToArray();
                using (AesDeriveBytes pdb = new AesDeriveBytes(raw)
                {
                    Iterations = iterations,
                    Key = pdbKey,
                    IV = new byte[16],
                })
                {
                    using (var sha = SHA256.Create())
                    {
                        var bytes = pdb.GetBytes();
                        var result = sha.ComputeHash(bytes);

                        Array.Clear(bytes, 0, bytes.Length);
                        Array.Clear(pdbKey, 0, pdbKey.Length);
                        return result;
                    }
                }
            }
            finally
            {
                Array.Clear(raw, 0, raw.Length);
            }
        }
    }
}