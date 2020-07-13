using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassDocumentXmlSerializer : XmlSerializable<IKeePassDocument>, IXmlSerializable
    {
        internal const string Group = "Group";

        internal const string Element = "Root";

        internal const string DeletedObjects = "DeletedObjects";

        private Type groupType;

        public KeePassDocumentXmlSerializer(IKeePassDocument model, SerializerContext context)
        {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.groupType = context.Mappings[typeof(IKeePassGroup)];
        }

        public override string Name
        {
            get
            {
                return "Root";
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var m = this.Model;
            IXmlSerializable serializer = null;
            switch (reader.Name)
            {
                case Group:
                    var group = m.RootGroup ?? new KeePassGroup()
                    {
                        Package = m.Package,
                    };
                    serializer = new KeePassGroupXmlSerializer(group, this.Context);
                    serializer.ReadXml(reader);
                    break;

                case DeletedObjects:
                    var deletedObject = new DeletedObjectInfo()
                    {
                        Package = m.Package,
                        Parent = m.RootGroup,
                    };
                    m.DeletedObjects.Add(deletedObject);
                    serializer = new KeePassDeletedObjectXmlSerializer(deletedObject, this.Context);
                    serializer.ReadXml(reader);
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var m = this.Model;

            if (m.RootGroup != null)
            {
                var gx = new KeePassGroupXmlSerializer(m.RootGroup, this.Context);
                gx.WriteXml(writer);
            }

            if (m.DeletedObjects != null && m.DeletedObjects.Count > 0)
            {
                writer.WriteStartElement(DeletedObjects);
                foreach (var deletedObject in m.DeletedObjects)
                {
                    var dox = new KeePassDeletedObjectXmlSerializer(deletedObject, this.Context);
                    dox.WriteXml(writer);
                }

                writer.WriteEndElement();
            }
        }
    }
}
