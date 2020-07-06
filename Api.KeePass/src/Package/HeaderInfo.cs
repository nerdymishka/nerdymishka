using System;
using System.Collections.Generic;
using System.IO;

namespace NerdyMishka.Api.KeePass.Package
{
    public class HeaderInfo
    {
        public const uint Signature1 = 0x9AA2D903;

        public const uint Signature2 = 0xB54BFB67;

        public const uint Version = 0x00030001;

        public const uint Version4 = 0x0004000;

        internal const uint Mask = 0xFFFF0000;

        private byte[] randomByteGeneratorCryptoKey;

        private byte randomByteGeneratorCryptoType;

        private IRandomByteGenerator randomByteGenerator;

        public KeePassIdentifier DatabaseCipherId { get; set; }

        public byte DatabaseCompression { get; set; }

        public IReadOnlyList<byte> DatabaseCipherKeySeed { get; set; }

        public IReadOnlyList<byte> MasterKeyHashKey { get; set; }

        public long MasterKeyHashRounds { get; set; }

        public IReadOnlyList<byte> DatabaseCipherIV { get; set; }

        public IReadOnlyList<byte> RandomByteGeneratorCryptoKey
        {
            get
            {
                return this.randomByteGeneratorCryptoKey;
            }

            set
            {
                this.randomByteGenerator = null;
                if (value == null)
                {
                    this.randomByteGeneratorCryptoKey = null;
                    return;
                }

                var set = new byte[value.Count];
                for (var i = 0; i < set.Length; i++)
                {
                    set[i] = value[i];
                }

                this.randomByteGeneratorCryptoKey = set;
            }
        }

        public IReadOnlyList<byte> HeaderByteMarks { get; set; }

        public byte RandomByteGeneratorCryptoType
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

        public IReadOnlyList<byte> Hash { get; set; }

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

                if ((version & Mask) > (Version & Mask))
                    throw new FormatException("The file version is unsupported");

                ms.Write(signature1Bytes);
                ms.Write(signature2Bytes);
                ms.Write(versionBytes);

                while (this.ReadNext(stream, ms))
                {
                    // Add Logging Statement
                }

                ReadOnlySpan<byte> data = ms.ToArray();
                this.Hash = data.ToSHA256Hash()
                    .ToArray();
            }
        }

        public void WriteToStream(Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            stream.Write(Signature1);
            stream.Write(Signature2);
            stream.Write(Version);

            var r = (byte)'\r';
            var n = (byte)'\n';
            var marker = new byte[] { r, n, r, n };

            stream.WriteHeader(HeaderField.DatabaseCipherId, this.DatabaseCipherId);
            stream.WriteHeader(HeaderField.DatabaseCipherKeySeed, this.DatabaseCipherKeySeed);
            stream.WriteHeader(HeaderField.MasterKeyHashSeed, this.MasterKeyHashKey);
            stream.WriteHeader(HeaderField.MasterKeyHashRounds, BitConverter.GetBytes((ulong)this.MasterKeyHashRounds));
            stream.WriteHeader(HeaderField.DatabaseCipherIV, this.DatabaseCipherIV);
            stream.WriteHeader(HeaderField.RandomBytesCryptoKey, this.RandomByteGeneratorCryptoKey);
            stream.WriteHeader(HeaderField.HeaderByteMark, this.HeaderByteMarks);
            stream.WriteHeader(HeaderField.RandomBytesCryptoType, BitConverter.GetBytes(this.RandomByteGeneratorCryptoType));
            stream.WriteHeader(HeaderField.DatabaseCompression, BitConverter.GetBytes(this.DatabaseCompression));
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

            byte[] sizeBytes = input.ReadBytes(2);
            ushort size = BitConverter.ToUInt16(sizeBytes, 0);
            byte[] data = null;

            if (size > 0)
                data = input.ReadBytes(size);

            output.Write(sizeBytes);
            output.Write(data);

            switch (field)
            {
                case HeaderField.EndOfHeader:
                    return false;

                case HeaderField.DatabaseCipherId:
                    this.DatabaseCipherId = data;
                    break;
                case HeaderField.DatabaseCompression:
                    if (data.Length < 1)
                        throw new Exception("Invalid Header");

                    this.DatabaseCompression = data[0];
                    break;
                case HeaderField.DatabaseCipherKeySeed:
                    this.DatabaseCipherKeySeed = data;
                    break;
                case HeaderField.MasterKeyHashSeed:
                    this.MasterKeyHashKey = data;
                    break;
                case HeaderField.MasterKeyHashRounds:
                    this.MasterKeyHashRounds = BitConverter.ToInt32(data, 0);
                    break;
                case HeaderField.DatabaseCipherIV:
                    this.DatabaseCipherIV = data;
                    break;
                case HeaderField.RandomBytesCryptoKey:
                    this.RandomByteGeneratorCryptoKey = data;

                    break;
                case HeaderField.HeaderByteMark:
                    this.HeaderByteMarks = data;
                    break;

                case HeaderField.RandomBytesCryptoType:
                    if (data.Length < 1)
                        throw new Exception("Invalid Header");
                    this.RandomByteGeneratorCryptoType = data[0];
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}