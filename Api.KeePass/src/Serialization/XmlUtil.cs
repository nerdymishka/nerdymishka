using System;
using System.Globalization;
using System.Xml;

namespace NerdyMishka.Api.KeePass.Serialization
{
    internal static class XmlUtil
    {
        public static byte[] ReadContentAsBytes(this XmlReader reader)
        {
            var value = reader.ReadContentAsString();
            var bytes = Convert.FromBase64String(value);
            return bytes;
        }

        public static DateTime ReadContentAsStringToDateTime(this XmlReader reader)
        {
            var dateString = reader.ReadContentAsString();

            if (DateTime.TryParse(dateString, out DateTime datetime))
                return datetime;

            return DateTime.MinValue;
        }

        public static bool? ReadContentAsNullableBoolean(this XmlReader reader)
        {
            if (!reader.HasValue)
                return null;

            var boolString = reader.ReadContentAsString();
            if (boolString is null)
                return null;

            boolString = boolString.Trim().ToUpperInvariant();

            switch (boolString)
            {
                case "YES":
                case "TRUE":
                case "1":
                    return true;
            }

            return false;
        }

        public static bool ReadContentAsStringToBoolean(this XmlReader reader)
        {
            if (!reader.HasValue)
                return false;

            var boolString = reader.ReadContentAsString();
            if (boolString == null)
                return false;

            boolString = boolString.Trim().ToUpperInvariant();

            switch (boolString)
            {
                case "YES":
                case "TRUE":
                case "1":
                    return true;
            }

            return false;
        }

        public static void WriteElement(this XmlWriter writer, string name, byte[] bytes)
        {
            writer.WriteStartElement(name);
            writer.WriteBase64(bytes, 0, bytes.Length);
            writer.WriteEndElement();
        }

        public static void WriteElement(this XmlWriter writer, string name, int value)
        {
            WriteElement(writer, name, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void WriteElement(this XmlWriter writer, string name, bool? value)
        {
            if (!value.HasValue)
            {
                WriteElement(writer, name, "null");
                return;
            }

            WriteElement(writer, name, value.Value);
        }

        public static void WriteElement(this XmlWriter writer, string name, bool value)
        {
            WriteElement(writer, name, value ? "True" : "False");
        }

        internal static void WriteElement(this XmlWriter writer, string name, DateTime value)
        {
            var content = value.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture);
            if (!content.EndsWith("Z", StringComparison.Ordinal))
                content += "Z";

            WriteElement(writer, name, content);
        }

        internal static void WriteElement(this XmlWriter writer, string name, string value)
        {
            writer.WriteStartElement(name);
            if (value != null)
                writer.WriteString(value);
            writer.WriteEndElement();
        }
    }
}
