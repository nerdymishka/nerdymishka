using System;
using System.Collections.Generic;
using System.IO;
using NerdyMishka.Security.Cryptography;
using NerdyMishka.Util.Binary;
using static NerdyMishka.Util.Binary.LittleEndianBitConverter;

namespace NerdyMishka.Api.KeePass.Package
{
    public class HeaderInfo
    {
        public const uint Signature1 = 0x9AA2D903;

        public const uint Signature2 = 0xB54BFB67;

        public const uint Version3 = 0x00030001;

        public const uint Version4 = 0x0004000;

        public const uint Latest = Version4;

        internal const uint Mask = 0xFFFF0000;

        private byte[] randomByteGeneratorCryptoKey;

        private int randomByteGeneratorCryptoType;

        private IRandomByteGenerator randomByteGenerator;

        public HeaderInfo(int version)
        {
            this.GenerateValues(version);
        }

        public string Comment { get; set; }

        public KeePassIdentifier PackageCipherId { get; set; }

        public int PackageCompression { get; set; }

        public int FileVersion { get; set; } = (int)Version4;

        public ReadOnlyMemory<byte> PackageCipherKey { get; set; }

        public ReadOnlyMemory<byte> AesKdfPassword { get; set; }

        public long AesKdfIterations { get; set; }

        public ReadOnlyMemory<byte> PackageCipherIV { get; set; }

        public ReadOnlyMemory<byte> RandomByteGeneratorCryptoKey
        {
            get
            {
                return this.randomByteGeneratorCryptoKey;
            }

            set
            {
                this.randomByteGenerator = null;
                if (value.IsEmpty)
                {
                    this.randomByteGeneratorCryptoKey = null;
                    return;
                }

                var set = new byte[value.Length];
                for (var i = 0; i < set.Length; i++)
                {
                    set[i] = value.Span[i];
                }

                this.randomByteGeneratorCryptoKey = set;
            }
        }

        public ReadOnlyMemory<byte> StreamStartByteMarker { get; set; }

        public int RandomByteGeneratorCryptoType
        {
            get
            {
                return this.randomByteGeneratorCryptoType;
            }

            set
            {
                this.randomByteGeneratorCryptoType = value;
                this.randomByteGenerator = null;
            }
        }

        public IRandomByteGenerator RandomByteGenerator
        {
            get
            {
                if (this.randomByteGenerator == null)
                {
                    this.randomByteGenerator =
                        RandomByteGeneratorFactory
                            .GetGenerator(this.randomByteGeneratorCryptoType);
                }

                return this.randomByteGenerator;
            }
        }

        public ReadOnlyMemory<byte> Hash { get; set; }

        public VariantDictionary CustomData { get; set; }

        public KdfParameters KdfParameters { get; set; }

        public void ClearGeneratorEngine()
        {
            this.randomByteGenerator = null;
        }

        public void GenerateValues(int version)
        {
            this.FileVersion = version;
            this.PackageCompression = (byte)1;

            using (var rng = new NerdyRandomNumberGenerator())
            {
                if (this.FileVersion < Version4)
                {
                    if (this.AesKdfIterations == 0)
                        this.AesKdfIterations = 60000;

                    this.PackageCipherId = AesCryptoStreamProvider.KdfId;
                    this.AesKdfPassword = rng.NextBytes(32);
                    this.StreamStartByteMarker = rng.NextBytes(32);
                    this.RandomByteGeneratorCryptoKey = rng.NextBytes(32);
                    this.RandomByteGeneratorCryptoType = (byte)2;
                }

                this.PackageCipherKey = rng.NextBytes(32);
                this.PackageCipherIV = rng.NextBytes(16);
            }
        }

        public void ReadFromStream(Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            using (var ms = new MemoryStream())
            {
                byte[] signature1Bytes = stream.ReadBytes(4);
                byte[] signature2Bytes = stream.ReadBytes(4);

                uint signature1 = signature1Bytes.ToUInt();
                uint signature2 = signature2Bytes.ToUInt();

                bool pass = signature1 == Signature1 && signature2 == Signature2;
                if (!pass)
                    throw new FormatException("Database has incorrect file signature");

                byte[] versionBytes = stream.ReadBytes(4);
                uint version = versionBytes.ToUInt();

                if ((version & Mask) > (Latest & Mask))
                    throw new FormatException($"The file version {version} is unsupported");

                ms.Write(signature1Bytes);
                ms.Write(signature2Bytes);
                ms.Write(versionBytes);

                while (this.ReadNext(stream, ms))
                {
                    // Add Logging Statement
                }

                this.Hash = EncryptionUtil.ComputeChecksumAsMemory(
                    ms.ToArray(),
                    HashAlgorithmName.SHA256);
            }
        }

        public void WriteToStream(Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            stream.Write(Signature1);
            stream.Write(Signature2);
            stream.Write((uint)this.FileVersion);

            var r = (byte)'\r';
            var n = (byte)'\n';
            var marker = new byte[] { r, n, r, n };

            stream.WriteHeader(HeaderField.PackageCipherId,
                this.PackageCipherId);

            stream.WriteHeader(HeaderField.PackageCompression,
                ToBytes((uint)this.PackageCompression));

            if (this.FileVersion < Version4)
            {
                stream.WriteHeader(HeaderField.AesKdfPassword, this.AesKdfPassword);
                stream.WriteHeader(HeaderField.AesKdfIterations, ToBytes(this.AesKdfIterations));
            }
            else
            {
                stream.WriteHeader(HeaderField.KdfParameters, this.KdfParameters.Serialize());
            }

            if (this.PackageCipherIV.Length > 0)
                stream.WriteHeader(HeaderField.PackageCipherIV, this.PackageCipherIV);

            if (this.FileVersion < Version4)
            {
                stream.WriteHeader(HeaderField.RandomBytesCryptoKey, this.RandomByteGeneratorCryptoKey);
                stream.WriteHeader(HeaderField.StreamStartByteMarker, this.StreamStartByteMarker);
                stream.WriteHeader(HeaderField.RandomBytesCryptoType, ToBytes(this.RandomByteGeneratorCryptoType));
            }

            if (this.CustomData != null && this.CustomData.Count > 0)
                stream.WriteHeader(HeaderField.PackageCustomData, this.CustomData.Serialize());

            stream.WriteHeader(HeaderField.EndOfHeader, marker);
        }

        protected bool ReadNext(Stream input, Stream output)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (output is null)
                throw new ArgumentNullException(nameof(output));

            int nextByte = input.ReadByte();
            if (nextByte == -1)
                throw new EndOfStreamException();

            byte fieldIdByte = (byte)nextByte;
            output.WriteByte(fieldIdByte);

            HeaderField field = (HeaderField)fieldIdByte;

            int size;
            byte[] sizeBytes;
            if ((uint)this.FileVersion < Version4)
            {
                sizeBytes = input.ReadBytes(2);
                size = LittleEndianBitConverter.ToUInt16(sizeBytes);
            }
            else
            {
                sizeBytes = input.ReadBytes(4);
                size = LittleEndianBitConverter.ToInt32(sizeBytes);
            }

            byte[] data = null;

            if (size > 0)
                data = input.ReadBytes(size);

            output.Write(sizeBytes);
            output.Write(data);

            switch (field)
            {
                case HeaderField.EndOfHeader:
                    return false;

                case HeaderField.PackageCipherId:
                    this.PackageCipherId = data;
                    break;

                case HeaderField.PackageCompression:
                    if (data.Length < 1)
                        throw new Exception("Invalid Header");

                    this.PackageCompression = (int)ToUInt32(data);
                    break;
                case HeaderField.PackageCipherKey:
                    this.PackageCipherKey = data;
                    break;

                case HeaderField.AesKdfPassword:
                    this.AesKdfPassword = data;
                    this.KdfParameters = this.KdfParameters ?? new KdfParameters(AesKdf.Id);
                    this.KdfParameters.Add(AesKdf.KeyParameterName, data);
                    break;

                case HeaderField.AesKdfIterations:
                    this.AesKdfIterations = (long)ToUInt64(data);
                    this.KdfParameters = this.KdfParameters ?? new KdfParameters(AesKdf.Id);
                    this.KdfParameters.Add(AesKdf.IterationsParameterName, this.AesKdfIterations);
                    break;

                case HeaderField.PackageCipherIV:
                    this.PackageCipherIV = data;
                    break;

                case HeaderField.RandomBytesCryptoKey:
                    this.RandomByteGeneratorCryptoKey = data;
                    break;

                case HeaderField.StreamStartByteMarker:
                    this.StreamStartByteMarker = data;
                    break;

                case HeaderField.RandomBytesCryptoType:
                    if (data.Length < 1)
                        throw new Exception("Invalid Header");
                    this.RandomByteGeneratorCryptoType = (int)ToUInt32(data);
                    break;

                case HeaderField.KdfParameters:
                    this.KdfParameters = this.KdfParameters ?? new KdfParameters();
                    this.KdfParameters.Deserialize(data);
                    break;

                case HeaderField.PackageCustomData:
                    this.CustomData = this.CustomData ?? new VariantDictionary();
                    this.CustomData.Deserialize(data);
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}