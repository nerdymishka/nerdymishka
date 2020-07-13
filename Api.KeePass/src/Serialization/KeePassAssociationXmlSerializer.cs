using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public class KeePassAssociationXmlSerializer : XmlSerializable<IKeePassAssociation>, IXmlSerializable
    {
        internal const string Association = "Association";

        internal const string Window = "Window";

        internal const string KeystrokeSequence = "KeystrokeSequence";

        public KeePassAssociationXmlSerializer(IKeePassAssociation association, SerializerContext context)
        {
            this.Model = association;
            this.Context = context;
        }

        public override string Name => Association;

        protected override void VisitElement(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var m = this.Model;
            switch (reader.Name)
            {
                case Window:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.Window = reader.ReadContentAsString();
                    }

                    break;

                case KeystrokeSequence:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.KeystrokeSequence = reader.ReadContentAsString();
                    }

                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var m = this.Model;
            writer.WriteElement(Window, m.Window);
            writer.WriteElement(KeystrokeSequence, m.KeystrokeSequence);
        }
    }
}