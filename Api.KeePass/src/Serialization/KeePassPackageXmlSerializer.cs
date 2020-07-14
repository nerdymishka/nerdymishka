using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassPackageXmlSerializer
    {
        public KeePassPackageXmlSerializer()
        {
            this.BinaryMap = new MemoryProtectedBytesMap();
            this.Mappings = new Dictionary<Type, Type>()
            {
                { typeof(IKeePassDocument), typeof(KeePassDocument) },
                { typeof(IKeePassAssociation), typeof(KeePassAssociation) },
                { typeof(IKeePassAutoType), typeof(KeePassAutoType) },
                { typeof(IKeePassAuditFields), typeof(KeePassAuditFields) },
                { typeof(IKeePassEntry), typeof(KeePassEntry) },
                { typeof(IKeePassGroup), typeof(KeePassGroup) },
            };
        }

        public string Extension => ".kdbx";

        public MemoryProtectedBytesMap BinaryMap { get; protected set; }

        public IDictionary<Type, Type> Mappings { get; }

        public void Read(IKeePassPackage package, Stream stream)
        {
            if (package is null)
                throw new ArgumentNullException(nameof(package));

            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var settings = new XmlReaderSettings()
            {
                CloseInput = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Prohibit,
            };

            settings.ValidationType = ValidationType.None;
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                reader.Read();

                var serializerContext = new SerializerContext()
                {
                    RandomByteGenerator = package.HeaderInfo.RandomByteGenerator,
                    DatabaseCompression = (byte)package.HeaderInfo.PackageCompression,
                    Mappings = this.Mappings,
                };

                var metaSerializer = new KeePassPackageInfoXmlSerializer(package.MetaInfo, serializerContext);
                metaSerializer.ReadXml(reader);

                // set memory protection options.
                serializerContext.MemoryProtection = package.MetaInfo.MemoryProtection;

                var rootSerializer = new KeePassDocumentXmlSerializer(package.Document, serializerContext);
                rootSerializer.ReadXml(reader);

                package.BinaryMap = serializerContext.BinaryMap;
            }
        }

        public void Write(IKeePassPackage package, Stream stream)
        {
            if (package is null)
                throw new ArgumentNullException(nameof(package));

            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                Encoding = NerdyMishka.Text.Utf8Options.NoBom,
            };

            var map = new MemoryProtectedBytesMap
            {
                package.Document.RootGroup,
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartElement("KeePassFile");

                // TODO: clear database genartor
                // package.HeaderInfo.
                var serializerContext = new SerializerContext()
                {
                    RandomByteGenerator = package.HeaderInfo.RandomByteGenerator,
                    MemoryProtection = package.MetaInfo.MemoryProtection,
                    DatabaseCompression = (byte)package.HeaderInfo.PackageCompression,
                    BinaryMap = map,
                    Mappings = this.Mappings,
                };

                var metaSerializer = new KeePassPackageInfoXmlSerializer(package.MetaInfo, serializerContext);
                metaSerializer.WriteXml(writer);

                var rootSerializer = new KeePassDocumentXmlSerializer(package.Document, serializerContext);
                rootSerializer.WriteXml(writer);

                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
