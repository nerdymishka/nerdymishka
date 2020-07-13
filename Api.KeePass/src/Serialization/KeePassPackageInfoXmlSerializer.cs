using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NerdyMishka.Api.KeePass.Package;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassPackageInfoXmlSerializer : XmlSerializable<PackageInfo>, IXmlSerializable
    {
        internal const string Generator = "Generator";

        internal const string DatabaseName = "DatabaseName";

        internal const string DatabaseNameChanged = "DatabaseNameChanged";

        internal const string DatabaseDescription = "DatabaseDescription";

        internal const string DatabaseDescriptionChanged = "DatabaseDescriptionChanged";

        internal const string DefaultUserName = "DefaultUserName";

        internal const string DefaultUserNameChanged = "DefaultUserNameChanged";

        internal const string MaintenanceHistoryDays = "MaintenanceHistoryDays";

        internal const string Color = "Color";

        internal const string MasterKeyChanged = "MasterKeyChanged";

        internal const string MasterKeyChangeRec = "MasterKeyChangeRec";

        internal const string MasterKeyChangeForce = "MasterKeyChangeForce";

        internal const string MemoryProtection = "MemoryProtection";

        internal const string RecycleBinEnabled = "RecycleBinEnabled";

        internal const string RecycleBinIdentifier = "RecycleBinUUID";

        internal const string RecycleBinChanged = "RecycleBinChanged";

        internal const string EntryTemplatesGroup = "EntryTemplatesGroup";

        internal const string EntryTemplatesGroupChanged = "EntryTemplatesGroupChanged";

        internal const string HistoryMaxItems = "HistoryMaxItems";

        internal const string HistoryMaxSize = "HistoryMaxSize";

        internal const string LastSelectedGroup = "LastSelectedGroup";

        internal const string LastTopVisibleGroup = "LastTopVisibleGroup";

        internal const string Binaries = "Binaries";

        internal const string CustomData = "CustomData";

        internal const string CustomIcons = "CustomIcons";

        internal const string Icon = "Icon";

        internal const string Data = "Data";

        internal const string Identifier = "UUID";

        internal const string Item = "Item";

        internal const string Binary = "Binary";

        internal const string Protected = "Protected";

        internal const string Key = "Key";

        internal const string Value = "Value";

        internal const string Element = "Meta";

        public KeePassPackageInfoXmlSerializer(PackageInfo info, SerializerContext context)
        {
            this.Model = info ?? throw new ArgumentNullException(nameof(info));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override string Name => Element;

        protected override void WriteProperties(XmlWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var m = this.Model;
            writer.WriteElement(Generator, m.Generator);
            writer.WriteElement(DatabaseName, m.DatabaseName);
            writer.WriteElement(DatabaseNameChanged, m.DatabaseNameChanged);
            writer.WriteElement(DatabaseDescription, m.DatabaseDescription);
            writer.WriteElement(DatabaseDescriptionChanged, m.DatabaseDescriptionChanged);
            writer.WriteElement(DefaultUserName, m.DefaultUserName);
            writer.WriteElement(DefaultUserNameChanged, m.DefaultUserNameChanged);
            writer.WriteElement(MaintenanceHistoryDays, m.MaintenanceHistoryDays);
            writer.WriteElement(Color, m.Color);
            writer.WriteElement(MasterKeyChanged, m.MasterKeyChanged);
            writer.WriteElement(MasterKeyChangeRec, m.MasterKeyChangeRec);
            writer.WriteElement(MasterKeyChangeForce, m.MasterKeyChangeForce);

            var serializer = new MemoryProtectionOptionsXmlSerializer(this.Model.MemoryProtection, this.Context);
            serializer.WriteXml(writer);

            var customIcons = this.Model.CustomIcons;

            if (customIcons != null && customIcons.Count > 0)
            {
                writer.WriteStartElement(CustomIcons);

                foreach (var icon in customIcons)
                {
                    writer.WriteStartElement(Icon);

                    writer.WriteStartElement(Identifier);
                    writer.WriteString(Convert.ToBase64String(icon.Id.ToArray()));
                    writer.WriteEndElement();

                    writer.WriteStartElement(Data);
                    writer.WriteString(Convert.ToBase64String(icon.ToBytes()));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteElement(RecycleBinEnabled, m.RecycleBinEnabled);
            writer.WriteElement(RecycleBinIdentifier, m.RecycleBinId.ToArray());
            writer.WriteElement(RecycleBinChanged, m.RecycleBinChanged);
            writer.WriteElement(EntryTemplatesGroup, m.EntryTemplatesGroupId.ToArray());
            writer.WriteElement(EntryTemplatesGroupChanged, m.EntryTemplatesGroupChanged);
            writer.WriteElement(HistoryMaxItems, m.HistoryMaxItems);
            writer.WriteElement(HistoryMaxSize, m.HistoryMaxSize);
            writer.WriteElement(LastSelectedGroup, this.Model.LastSelectedGroupId.ToArray());
            writer.WriteElement(LastTopVisibleGroup, this.Model.LastTopVisibleGroupId.ToArray());

            var binaries = this.Context.BinaryMap;
            var customData = this.Model.CustomData;
            writer.WriteStartElement(Binaries);

            if (binaries != null && binaries.Count > 0)
            {
                foreach (var kv in binaries)
                {
                    writer.WriteStartElement(Binary);
                    writer.WriteAttributeString("ID", kv.Key.ToString(CultureInfo.InvariantCulture));

                    if (kv.Value.IsProtected)
                    {
                        writer.WriteAttributeString(Protected, "True");
                        var raw = kv.Value.ToArray();
                        var pad = this.Context.RandomByteGenerator.NextBytes(raw.Length);
                        var bytes = new byte[raw.Length];

                        for (var i = 0; i < raw.Length; i++)
                            bytes[i] = (byte)(raw[i] ^ pad[i]);

                        Array.Clear(raw, 0, raw.Length);
                        Array.Clear(pad, 0, pad.Length);

                        writer.WriteString(Convert.ToBase64String(bytes));
                        Array.Clear(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        var bytes = kv.Value.ToArray();
                        if (this.Context.DatabaseCompression == (byte)1)
                        {
                            writer.WriteAttributeString("Compressed", "True");
                            bytes = this.Compress(bytes);
                        }

                        writer.WriteString(Convert.ToBase64String(bytes));
                        Array.Clear(bytes, 0, bytes.Length);
                    }

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteStartElement(CustomData);

            if (customData != null && customData.Count > 0)
            {
                foreach (var kp in customData)
                {
                    writer.WriteStartElement(Item);

                    writer.WriteStartElement(Key);
                    writer.WriteString(kp.Key);
                    writer.WriteEndElement();

                    writer.WriteStartElement(Data);
                    writer.WriteString(kp.Value);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            switch (reader.Name)
            {
                case Generator:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.Generator = reader.ReadContentAsString();
                    }

                    break;

                case DatabaseName:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.DatabaseName = reader.ReadContentAsString();
                    }

                    break;

                case DatabaseNameChanged:
                    reader.Read();
                    this.Model.DatabaseNameChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case DatabaseDescription:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.DatabaseDescription = reader.ReadContentAsString();
                    }

                    break;

                case DefaultUserName:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.DefaultUserName = reader.ReadContentAsString();
                    }

                    break;

                case DefaultUserNameChanged:
                    reader.Read();
                    this.Model.DefaultUserNameChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case MaintenanceHistoryDays:
                    reader.Read();
                    this.Model.MaintenanceHistoryDays = reader.ReadContentAsInt();
                    break;

                case Color:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.Color = reader.ReadContentAsString();
                    }

                    break;

                case MasterKeyChanged:
                    reader.Read();
                    this.Model.MasterKeyChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case MasterKeyChangeRec:
                    reader.Read();
                    this.Model.MasterKeyChangeRec = reader.ReadContentAsInt();
                    break;

                case MasterKeyChangeForce:
                    reader.Read();
                    this.Model.MasterKeyChangeForce = reader.ReadContentAsInt();
                    break;

                case MemoryProtection:
                    var serializer = new MemoryProtectionOptionsXmlSerializer(this.Model.MemoryProtection, this.Context);
                    serializer.ReadXml(reader);
                    this.Model.MemoryProtection = serializer.Model;
                    break;

                case RecycleBinEnabled:
                    reader.Read();
                    this.Model.RecycleBinEnabled = reader.ReadContentAsStringToBoolean();
                    break;

                case RecycleBinIdentifier:
                    reader.Read();
                    this.Model.RecycleBinId = reader.ReadContentAsBytes();
                    break;

                case RecycleBinChanged:
                    reader.Read();
                    this.Model.RecycleBinChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case EntryTemplatesGroup:
                    reader.Read();
                    this.Model.EntryTemplatesGroupId = reader.ReadContentAsBytes();
                    break;

                case EntryTemplatesGroupChanged:
                    reader.Read();
                    this.Model.EntryTemplatesGroupChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case HistoryMaxItems:
                    reader.Read();
                    this.Model.HistoryMaxItems = reader.ReadContentAsInt();
                    break;

                case HistoryMaxSize:
                    reader.Read();
                    this.Model.HistoryMaxSize = reader.ReadContentAsInt();
                    break;

                case LastSelectedGroup:
                    reader.Read();
                    this.Model.LastSelectedGroupId = reader.ReadContentAsBytes();
                    break;

                case LastTopVisibleGroup:
                    reader.Read();
                    this.Model.LastSelectedGroupId = reader.ReadContentAsBytes();

                    break;

                case Binaries:
                    if (reader.IsEmptyElement)
                        return;

                    this.ReadBinaries(reader);
                    break;
                case CustomIcons:
                    if (reader.IsEmptyElement)
                        return;

                    this.ReadCustomIcons(reader);

                    break;
                case CustomData:
                    if (reader.IsEmptyElement)
                        return;

                    this.ReadCustomData(reader);

                    break;
                default:
                    break;
            }
        }

        private void ReadCustomIcons(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Icon")
                    this.ReadCustomIcon(reader);

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "CustomIcons")
                {
                    return;
                }
            }
        }

        private void ReadCustomIcon(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            string name = null;
            byte[] uuid = null;
            byte[] data = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "UUID":
                            reader.Read();
                            uuid = Convert.FromBase64String(reader.ReadContentAsString());
                            break;

                        case "Name":
                            reader.Read();
                            name = reader.ReadContentAsString();
                            break;

                        case "Data":
                            reader.Read();
                            data = Convert.FromBase64String(reader.ReadContentAsString());
                            break;
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Icon")
                {
                    var icon = new KeePassIcon()
                    {
                        Id = uuid,
                        Name = name,
                    };
                    icon.SetIcon(data);
                    this.Model.CustomIcons.Add(icon);

                    return;
                }
            }
        }

        private void ReadCustomData(XmlReader reader)
        {
            string key = null;
            string value = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "Key":
                            key = reader.ReadContentAsString();
                            break;

                        case "Value":
                            value = reader.ReadContentAsString();
                            this.Model.CustomData.Add(key, value);
                            break;
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CustomData)
                {
                    return;
                }
            }
        }

        private void ReadBinaries(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == Binary)
                    this.ReadBinary(reader);

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == Binaries)
                {
                    return;
                }
            }
        }

        private byte[] Compress(byte[] bytes)
        {
            using (var source = new MemoryStream(bytes, false))
            using (var dest = new MemoryStream())
            using (var gz = new GZipStream(dest, CompressionMode.Compress))
            {
                source.CopyTo(gz);
                return dest.ToArray();
            }
        }

        private byte[] Decompress(byte[] bytes)
        {
            using (var source = new MemoryStream(bytes, false))
            using (var gz = new GZipStream(source, CompressionMode.Decompress))
            using (var dest = new MemoryStream())
            {
                gz.CopyTo(dest);
                return dest.ToArray();
            }
        }

        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000: Dispose objects before losing scope",
            Justification = "MemoryProtectionText should not be disposed")]
        private void ReadBinary(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            bool compressed = false,
                 isProtected = false;

            string id = null;

            if (reader.HasAttributes)
            {
                id = reader.GetAttribute("ID");
                var compressedString = reader.GetAttribute("Compressed");
                var isProtectedString = reader.GetAttribute(Protected);

                if (compressedString != null && compressedString == "True")
                    compressed = true;

                if (isProtectedString != null & isProtectedString == "True")
                    isProtected = true;
            }

            reader.Read();
            var content = reader.ReadContentAsString();
            var bytes = Convert.FromBase64String(content);

            MemoryProtectedBytes binary = null;

            if (isProtected)
            {
                var pad = this.Context.RandomByteGenerator.NextBytes(bytes.Length);
                var raw = new byte[bytes.Length];

                for (var i = 0; i < bytes.Length; i++)
                    raw[i] = (byte)(bytes[i] ^ pad[i]);

                binary = new MemoryProtectedBytes(raw, true);
                Array.Clear(raw, 0, raw.Length);
                Array.Clear(pad, 0, pad.Length);
            }
            else
            {
                if (compressed)
                {
                    bytes = this.Decompress(bytes);
                }

                binary = new MemoryProtectedBytes(bytes.AsSpan(), encrypt: false);
            }

            Array.Clear(bytes, 0, bytes.Length);

            if (int.TryParse(id, out int identity))
            {
                this.Context.BinaryMap.Add(identity, binary);
            }
            else
            {
                this.Context.BinaryMap.Add(binary);
            }
        }
    }
}