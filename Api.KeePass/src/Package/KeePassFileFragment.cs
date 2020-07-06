using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NerdyMishka.Util.Arrays;

namespace NerdyMishka.Api.KeePass.Package
{
    public class KeePassFileFragment : KeePassCompositeKeyFragment
    {
        public KeePassFileFragment(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            this.Path = path;

            if (!System.IO.File.Exists(path))
                throw new System.IO.FileNotFoundException(path);

            var data = ReadAndTransformKey(path);
            this.SetData(data);
        }

        public string Path { get; private set; }

        public static void GenerateFile(string path, byte[] entropy)
        {
            var generator = RandomByteGeneratorFactory.GetGenerator(2);
            var key = generator.NextBytes(32);

            using (var ms = new MemoryStream())
            {
                ms.Write(entropy);
                ms.Write(key);

                ReadOnlySpan<byte> span = ms.ToArray();
                var hash = span.ToSHA256Hash();
                CreateFile(path, hash);
            }
        }

        private static ReadOnlySpan<byte> ReadAndTransformKey(string path)
        {
            var bytes = ReadKeyFromFile(path);
            var hex = bytes.ToHexString();
            var key = Enumerable.Range(0, hex.Length / 2)
                                .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                                .ToArray();

            return key;
        }

        private static ReadOnlySpan<byte> ReadKeyFromFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var doc = new XmlDocument();
                doc.Load(fs);

                var el = doc.DocumentElement;
                if (el == null)
                    return null;

                if (!el.Name.Equals("KeyFile", StringComparison.OrdinalIgnoreCase))
                    return null;

                foreach (XmlNode child in el.ChildNodes)
                {
                    if (child.Name == "Key")
                    {
                        foreach (XmlNode subChild in child.ChildNodes)
                        {
                            if (subChild.Name == "Data")
                            {
                                return Convert.FromBase64String(subChild.InnerText);
                            }
                        }
                    }
                }
            }

            return Array.Empty<byte>();
        }

        private static void CreateFile(string path, ReadOnlySpan<byte> data)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                Encoding = new UTF8Encoding(false, false),
            };
            var key = data.ToArray();

            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                using (var xmlWriter = XmlWriter.Create(fs, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("KeyFile");
                    xmlWriter.WriteStartElement("Meta");
                    xmlWriter.WriteStartElement("Version");
                    xmlWriter.WriteString("1.00");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Data");
                    xmlWriter.WriteString(Convert.ToBase64String(key));
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                }
            }
            finally
            {
                key.Clear();
            }
        }
    }
}