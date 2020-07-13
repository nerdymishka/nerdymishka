using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassGroupXmlSerializer : XmlSerializable<IKeePassGroup>, IXmlSerializable
    {
        internal const string CustomIconUuid = "CustomIconUUID";

        internal const string Identifier = "UUID";

        internal const string GroupName = "Name";

        internal const string Notes = "Notes";

        internal const string IconId = "IconID";

        internal const string Times = "Times";

        internal const string IsExpanded = "IsExpanded";

        internal const string DefaultAutoTypeSequence = "DefaultAutoTypeSequence";

        internal const string EnableAutoType = "EnableAutoType";

        internal const string EnableSearching = "EnableSearching";

        internal const string LastTopVisibleEntry = "LastTopVisibleEntry";

        internal const string CustomData = "CustomData";

        internal const string Item = "Item";

        internal const string Key = "Key";

        internal const string Value = "Value";

        internal const string Entry = "Entry";

        internal const string Element = "Group";

        private Type entryType;

        private Type groupType;

        public KeePassGroupXmlSerializer(IKeePassGroup group, SerializerContext context)
        {
            this.Model = group ?? throw new ArgumentNullException(nameof(group));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.entryType = context.Mappings[typeof(IKeePassEntry)];
            this.groupType = context.Mappings[typeof(IKeePassGroup)];
        }

        public override string Name => Element;

        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var m = this.Model;
            IXmlSerializable serializer = null;
            switch (reader.Name)
            {
                case Identifier:
                    reader.Read();
                    m.Id = reader.ReadContentAsBytes();

                    break;

                case GroupName:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.Name = reader.ReadContentAsString();
                    }

                    break;

                case Notes:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.Notes = reader.ReadContentAsString();
                    }

                    break;

                case IconId:
                    reader.Read();
                    m.IconId = reader.ReadContentAsInt();
                    break;

                case CustomIconUuid:
                    reader.Read();
                    m.CustomIconUuid = Convert.FromBase64String(reader.ReadContentAsString());
                    break;

                case Times:
                    serializer = new KeePassAuditFieldsXmlSerializer(m.AuditFields, this.Context);
                    serializer.ReadXml(reader);
                    break;

                case IsExpanded:
                    reader.Read();
                    m.IsExpanded = reader.ReadContentAsStringToBoolean();
                    break;

                case DefaultAutoTypeSequence:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.DefaultAutoTypeSequence = reader.ReadContentAsString();
                    }

                    break;

                case EnableAutoType:
                    reader.Read();
                    m.EnableAutoType = reader.ReadContentAsNullableBoolean();
                    break;

                case EnableSearching:
                    reader.Read();
                    m.EnableSearching = reader.ReadContentAsNullableBoolean();
                    break;

                case LastTopVisibleEntry:
                    reader.Read();
                    m.LastTopVisibleEntryId = reader.ReadContentAsBytes();
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
                                    customDataValue = null;
                                    break;
                            }
                        }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CustomData)
                            break;
                    }

                    break;

                case Entry:
                    var entry = (IKeePassEntry)Activator.CreateInstance(this.entryType);
                    m.Entries.Add(entry);
                    serializer = new KeePassEntryXmlSerializer(entry, this.Context);
                    serializer.ReadXml(reader);
                    break;

                case Element:
                    var group = (IKeePassGroup)Activator.CreateInstance(this.groupType);
                    m.Groups.Add(group);
                    serializer = new KeePassGroupXmlSerializer(group, this.Context);
                    serializer.ReadXml(reader);
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var m = this.Model;
            writer.WriteElement(Identifier, m.Id.ToArray());

            writer.WriteElement(GroupName, m.Name);
            writer.WriteElement(Notes, m.Notes);
            writer.WriteElement(IconId, m.IconId);

            if (!m.CustomIconUuid.IsEmpty)
                writer.WriteElement(CustomIconUuid, Convert.ToBase64String(m.CustomIconUuid.ToArray()));

            var tx = new KeePassAuditFieldsXmlSerializer(m.AuditFields, this.Context);
            tx.WriteXml(writer);

            writer.WriteElement(IsExpanded, m.IsExpanded);
            writer.WriteElement(DefaultAutoTypeSequence, m.DefaultAutoTypeSequence);
            writer.WriteElement(EnableAutoType, m.EnableAutoType);
            writer.WriteElement(EnableSearching, m.EnableSearching);
            writer.WriteElement(LastTopVisibleEntry, m.LastTopVisibleEntryId.ToArray());

            if (m.CustomData != null && m.CustomData.Count > 0)
            {
                writer.WriteStartElement(CustomData);

                // TODO: escape string values;
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

            if (m.Entries != null)
            {
                foreach (var entry in m.Entries)
                {
                    var ex = new KeePassEntryXmlSerializer(entry, this.Context);
                    ex.WriteXml(writer);
                }
            }

            if (m.Groups != null)
            {
                foreach (var group in m.Groups)
                {
                    var gx = new KeePassGroupXmlSerializer(group, this.Context);
                    gx.WriteXml(writer);
                }
            }
        }
    }
}