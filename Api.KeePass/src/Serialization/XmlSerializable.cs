﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyMishka.Api.KeePass.Serialization
{
    public abstract class XmlSerializable<T> : IXmlSerializable
    {
        private T model;

        private Type modelType;

        public abstract string Name { get; }

        public SerializerContext Context { get; set; }

        public T Model
        {
            get
            {
                if (this.model == null)
                {
                    if (this.modelType == null)
                    {
                        if (this.Context.Mappings.ContainsKey(typeof(T)))
                            this.modelType = this.Context.Mappings[typeof(T)];
                        else
                            this.modelType = typeof(T);
                    }

                    this.model = (T)Activator.CreateInstance(this.modelType);
                }

                return this.model;
            }

            protected set
            {
                this.model = value;
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.Name == this.Name)
                        {
                            return;
                        }

                        break;

                    case XmlNodeType.Element:

                        this.VisitElement(reader);
                        break;

                    case XmlNodeType.Attribute:
                        this.VisitAttribute(reader);
                        break;

                    default:
                        break;
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            writer.WriteStartElement(this.Name);

            this.WriteProperties(writer);

            writer.WriteEndElement();
        }

        protected abstract void WriteProperties(XmlWriter writer);

        protected virtual void VisitAttribute(XmlReader reader)
        {
        }

        protected abstract void VisitElement(XmlReader reader);
    }
}
