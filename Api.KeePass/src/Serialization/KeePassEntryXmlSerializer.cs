using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassEntryXmlSerializer : XmlSerializable<IKeePassEntry>, IXmlSerializable
    {
        internal const string Entry = "Entry";

        internal const string Identifier = "UUID";

        internal const string CustomIconUuid = "CustomIconUUID";

        internal const string IconId = "IconID";

        internal const string ForegroundColor = "ForegroundColor";

        internal const string BackgroundColor = "BackgroundColor";

        internal const string OverrideUrl = "OverrideURL";

        internal const string Tags = "Tags";

        internal const string Times = "Times";

        internal const string AutoType = "AutoType";

        internal const string History = "History";

        internal const string CustomData = "CustomData";

        internal const string Item = "Item";

        internal const string Key = "Key";

        internal const string Ref = "ref";

        internal const string Value = "Value";

        internal const string String = "String";

        internal const string Binary = "Binary";

        private Type entryType;

        public KeePassEntryXmlSerializer(IKeePassEntry entry, SerializerContext context)
        {
            this.Model = entry ?? throw new ArgumentNullException(nameof(entry));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.entryType = context.Mappings[typeof(IKeePassEntry)];
        }

        public override string Name => Entry;

        protected override void WriteProperties(XmlWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var m = this.Model;

            writer.WriteElement(Identifier, m.Id.ToArray());
            writer.WriteElement(IconId, m.IconId);

            if (m.CustomIconUuid != null && m.CustomIconUuid != KeePassIdentifier.Empty)
                writer.WriteElement(CustomIconUuid, Convert.ToBase64String(m.CustomIconUuid.ToArray()));

            writer.WriteElement(ForegroundColor, m.ForegroundColor);
            writer.WriteElement(BackgroundColor, m.BackgroundColor);
            writer.WriteElement(OverrideUrl, m.OverrideUrl);

            var tags = string.Empty;
            if (m.Tags.Count > 0)
                tags = string.Join(";", m.Tags);

            writer.WriteElement(Tags, tags);

            var tx = new KeePassAuditFieldsXmlSerializer(m.AuditFields, this.Context);
            tx.WriteXml(writer);

            foreach (var kv in m.Strings)
            {
                writer.WriteStartElement(String);
                this.WriteMemoryProtectedString(writer, kv);
                writer.WriteEndElement();
            }

            if (m.Binaries != null && m.Binaries.Count > 0)
            {
                foreach (var kv in m.Binaries)
                {
                    writer.WriteStartElement(Binary);

                    writer.WriteStartElement(Key);
                    writer.WriteString(kv.Key);
                    writer.WriteEndElement();

                    writer.WriteStartElement(Value);
                    var index = this.Context.BinaryMap.IndexOf(kv.Value);
                    if (index == -1)
                        throw new IndexOutOfRangeException(
                            $"Entry {m.Name} with binary {kv.Key} does not exist in the BinaryMap ");
                    writer.WriteAttributeString(Ref, index.ToString(CultureInfo.InvariantCulture));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            if (m.AutoType != null)
            {
                var axs = new KeePassAutoTypeXmlSerializer(m.AutoType, this.Context);
                axs.WriteXml(writer);
            }

            if (m.CustomData != null && m.CustomData.Count > 0)
            {
                writer.WriteStartElement(CustomData);
                foreach (var kv in m.CustomData)
                {
                    writer.WriteStartElement(Item);

                    writer.WriteStartElement(Key);
                    writer.WriteString(kv.Key);
                    writer.WriteEndElement();

                    writer.WriteStartElement(Value);
                    writer.WriteString(kv.Value);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            if (m.History != null && m.History.Count > 0)
            {
                writer.WriteStartElement("History");

                foreach (var entry in m.History)
                {
                    var ex = new KeePassEntryXmlSerializer(entry, this.Context);
                    ex.WriteXml(writer);
                }

                writer.WriteEndElement();
            }
        }

        protected void WriteMemoryProtectedString(XmlWriter writer, KeyValuePair<string, MemoryProtectedText> kv)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteStartElement(Key);
            writer.WriteString(kv.Key);
            writer.WriteEndElement();

            writer.WriteStartElement(Value);
            if (kv.Value.Length == 0)
            {
                writer.WriteEndElement();
                return;
            }

            if (!this.IsProtected(kv))
            {
                var span = kv.Value.ToCharSpan().ToArray();
                writer.WriteChars(span, 0, span.Length);
                Array.Clear(span, 0, span.Length);
                writer.WriteEndElement();
                return;
            }

            writer.WriteAttributeString("Protected", "True");
            byte[] raw = kv.Value.ToArray();
            byte[] pad = this.Context.RandomByteGenerator.NextBytes(raw.Length);

            for (var i = 0; i < raw.Length; i++)
                raw[i] ^= pad[i];

            writer.WriteString(Convert.ToBase64String(raw));
            writer.WriteEndElement();
            Array.Clear(raw, 0, raw.Length);
            Array.Clear(pad, 0, pad.Length);
        }

        protected bool IsProtected(KeyValuePair<string, MemoryProtectedText> kv)
        {
            bool isProtected = kv.Value.IsProtected;
            switch (kv.Key)
            {
                case "Title":
                    isProtected = this.Context.MemoryProtection.ProtectTitle;
                    break;

                case "UserName":
                    isProtected = this.Context.MemoryProtection.ProtectUserName;
                    break;

                case "Notes":
                    isProtected = this.Context.MemoryProtection.ProtectNotes;
                    break;

                case "Password":
                    isProtected = this.Context.MemoryProtection.ProtectPassword;
                    break;

                case "URL":
                    isProtected = this.Context.MemoryProtection.ProtectUrl;
                    break;
            }

            return isProtected;
        }

        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000: Dispose objects before losing scope",
            Justification = "MemoryProtectionText should not be disposed")]
        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var m = this.Model;
            IXmlSerializable serializer;
            switch (reader.Name)
            {
                case Identifier:
                    reader.Read();
                    m.Id = reader.ReadContentAsBytes();
                    break;

                case IconId:
                    reader.Read();
                    m.IconId = reader.ReadContentAsInt();
                    break;

                case CustomIconUuid:
                    reader.Read();
                    m.CustomIconUuid = Convert.FromBase64String(reader.ReadContentAsString());
                    break;

                case ForegroundColor:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.ForegroundColor = reader.ReadContentAsString();
                    }

                    break;

                case BackgroundColor:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.BackgroundColor = reader.ReadContentAsString();
                    }

                    break;

                case OverrideUrl:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.OverrideUrl = reader.ReadContentAsString();
                    }

                    break;

                case Tags:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        var tags = reader.ReadContentAsString();
                        if (tags != null)
                        {
                            foreach (var tag in tags.Split(';'))
                            {
                                m.Tags.Add(tag);
                            }
                        }
                    }

                    break;

                case Times:
                    serializer = new KeePassAuditFieldsXmlSerializer(m.AuditFields, this.Context);
                    serializer.ReadXml(reader);

                    break;

                case String:
                    MemoryProtectedText text = null;
                    string stringKey = null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "Key":
                                    if (reader.IsEmptyElement)
                                        continue;

                                    stringKey = reader.ReadContentAsString();
                                    break;
                                case "Value":
                                    bool isProtected = false;
                                    if (reader.HasAttributes)
                                    {
                                        var pValue = reader.GetAttribute("Protected");
                                        if (pValue != null && pValue == "True")
                                            isProtected = true;
                                    }

                                    reader.Read();
                                    if (isProtected)
                                    {
                                        var content = reader.ReadContentAsString();
                                        if (content.Length > 0)
                                        {
                                            byte[] raw = Convert.FromBase64String(content);
                                            byte[] pad = this.Context.RandomByteGenerator.NextBytes(raw.Length);
                                            for (var i = 0; i < raw.Length; ++i)
                                                raw[i] ^= pad[i];

                                            text = new MemoryProtectedText(raw);
                                            Array.Clear(raw, 0, raw.Length);
                                            Array.Clear(pad, 0, pad.Length);
                                        }
                                        else
                                        {
                                            text = new MemoryProtectedText(Array.Empty<byte>());
                                        }
                                    }
                                    else
                                    {
                                        var content = reader.ReadContentAsString();
                                        text = new MemoryProtectedText(content);
                                    }

                                    break;
                            }
                        }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == String)
                            break;
                    }

                    if (stringKey != null && text != null)
                        m.Strings.Add(stringKey, text);

                    break;

                case Binary:
                    string binaryKey = null;
                    MemoryProtectedBytes bytes = null;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "Key":
                                    if (reader.IsEmptyElement)
                                        continue;
                                    reader.Read();
                                    binaryKey = reader.ReadContentAsString();

                                    break;
                                case "Value":

                                    if (reader.HasAttributes)
                                    {
                                        var refValue = reader.GetAttribute("Ref");
                                        if (int.TryParse(refValue, out int refInt))
                                        {
                                            bytes = this.Context.BinaryMap[refInt];
                                        }
                                    }

                                    break;
                            }
                        }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Binary")
                            break;
                    }

                    if (binaryKey != null && bytes != null)
                        m.Binaries.Add(binaryKey, bytes);

                    break;

                case AutoType:
                    serializer = new KeePassAutoTypeXmlSerializer(m.AutoType, this.Context);
                    serializer.ReadXml(reader);
                    break;

                case CustomData:
                    string customDataKey = null;
                    string customDataValue = null;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "Key":
                                    if (reader.IsEmptyElement)
                                        continue;

                                    customDataKey = reader.ReadContentAsString();
                                    break;

                                case "Value":
                                    if (reader.IsEmptyElement)
                                        continue;

                                    customDataValue = reader.ReadContentAsString();
                                    m.CustomData.Add(customDataKey, customDataValue);
                                    customDataKey = null;
                                    break;
                            }
                        }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CustomData)
                            break;
                    }

                    break;
                case History:
                    if (reader.IsEmptyElement)
                        return;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Entry")
                        {
                            if (reader.IsEmptyElement)
                                continue;

                            var entry = (IKeePassEntry)Activator.CreateInstance(this.entryType);
                            entry.IsHistorical = true;
                            serializer = new KeePassEntryXmlSerializer(entry, this.Context);
                            serializer.ReadXml(reader);
                            m.History.Add(entry);
                        }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "History")
                        {
                            return;
                        }
                    }

                    break;
            }
        }
    }
}
