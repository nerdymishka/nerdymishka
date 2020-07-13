using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassAuditFieldsXmlSerializer : XmlSerializable<IKeePassAuditFields>, IXmlSerializable
    {
        internal const string Times = "Times";

        internal const string CreationTime = "CreationTime";

        internal const string LastModificationTime = "LastModificationTime";

        internal const string LastAccessTime = "LastAccessTime";

        internal const string ExpiryTime = "ExpiryTime";

        internal const string Expires = "Expires";

        internal const string UsageCount = "UsageCount";

        internal const string LocationChanged = "LocationChanged";

        public KeePassAuditFieldsXmlSerializer(IKeePassAuditFields time, SerializerContext context)
        {
            this.Model = time;
            this.Context = context;
        }

        public override string Name => Times;

        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var m = this.Model;
            switch (reader.Name)
            {
                case CreationTime:
                    reader.Read();
                    m.CreationTime = reader.ReadContentAsStringToDateTime();
                    break;

                case LastModificationTime:
                    reader.Read();
                    m.LastModificationTime = reader.ReadContentAsStringToDateTime();
                    break;

                case LastAccessTime:
                    reader.Read();
                    m.LastAccessTime = reader.ReadContentAsStringToDateTime();
                    break;

                case ExpiryTime:
                    reader.Read();
                    m.ExpiryTime = reader.ReadContentAsStringToDateTime();
                    break;

                case Expires:
                    reader.Read();
                    m.Expires = reader.ReadContentAsStringToBoolean();
                    break;

                case UsageCount:
                    reader.Read();
                    m.UsageCount = reader.ReadContentAsInt();
                    break;

                case LocationChanged:
                    reader.Read();
                    m.LocationChanged = reader.ReadContentAsStringToDateTime();
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement(CreationTime, m.CreationTime);
            writer.WriteElement(LastModificationTime, m.LastModificationTime);
            writer.WriteElement(LastAccessTime, m.LastAccessTime);
            writer.WriteElement(ExpiryTime, m.ExpiryTime);
            writer.WriteElement(Expires, m.Expires);
            writer.WriteElement(UsageCount, m.UsageCount);
            writer.WriteElement(LocationChanged, m.LocationChanged);
        }
    }
}
