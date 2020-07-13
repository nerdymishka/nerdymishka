using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class MemoryProtectionOptionsXmlSerializer : XmlSerializable<MemoryProtectionOptions>, IXmlSerializable
    {
        internal const string ProtectTitle = "ProtectTitle";

        internal const string ProtectUserName = "ProtectUserName";

        internal const string ProtectPassword = "ProtectPassword";

        internal const string ProtectUrl = "ProtectURL";

        internal const string ProtectNotes = "ProtectNotes";

        internal const string Element = "MemoryProtection";

        public MemoryProtectionOptionsXmlSerializer(MemoryProtectionOptions protection, SerializerContext context)
        {
            this.Model = protection;
            this.Context = context;
        }

        public override string Name => Element;

        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var m = this.Model;

            if (reader.NodeType != XmlNodeType.Element)
                return;

            switch (reader.Name)
            {
                case ProtectTitle:
                    reader.Read();
                    m.ProtectTitle = reader.ReadContentAsStringToBoolean();
                    break;

                case ProtectUserName:
                    reader.Read();
                    m.ProtectPassword = reader.ReadContentAsStringToBoolean();
                    break;

                case ProtectPassword:
                    reader.Read();
                    m.ProtectPassword = reader.ReadContentAsStringToBoolean();
                    break;

                case ProtectUrl:
                    reader.Read();
                    m.ProtectUrl = reader.ReadContentAsStringToBoolean();
                    break;

                case ProtectNotes:
                    reader.Read();
                    m.ProtectNotes = reader.ReadContentAsStringToBoolean();
                    break;

                default:
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement(ProtectTitle, m.ProtectTitle);
            writer.WriteElement(ProtectUserName, m.ProtectUserName);
            writer.WriteElement(ProtectPassword, m.ProtectPassword);
            writer.WriteElement(ProtectUrl, m.ProtectUrl);
            writer.WriteElement(ProtectNotes, m.ProtectNotes);
        }
    }
}
