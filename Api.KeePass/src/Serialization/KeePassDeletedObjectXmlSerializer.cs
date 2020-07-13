using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassDeletedObjectXmlSerializer : XmlSerializable<DeletedObjectInfo>, IXmlSerializable
    {
        internal const string Element = "DeletedObject";

        internal const string Identifier = "UUID";

        internal const string DeletionTime = "DeletionTime";

        public KeePassDeletedObjectXmlSerializer(DeletedObjectInfo model, SerializerContext context)
        {
            this.Model = model;
            this.Context = context;
        }

        public override string Name => Element;

        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var m = this.Model;
            switch (reader.Name)
            {
                case Identifier:
                    reader.Read();
                    m.Id = reader.ReadContentAsBytes();
                    break;

                case DeletionTime:
                    reader.Read();
                    m.DeletionTime = reader.ReadContentAsStringToDateTime();
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var m = this.Model;
            writer.WriteElement(Identifier, m.Id.ToArray());
            writer.WriteElement(DeletionTime, m.DeletionTime);
        }
    }
}
