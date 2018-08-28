using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TWIConnect.Client.Utilities
{
    public static class Serialization
    {
        public static string Serialize(this object graph)
        {
            XmlSerializer serializer = new XmlSerializer(graph.GetType());
            System.Text.StringBuilder stream = new System.Text.StringBuilder();
            using (XmlWriter xmlWriter = XmlTextWriter.Create(stream, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                serializer.Serialize(xmlWriter, graph, new XmlSerializerNamespaces(new[] { new System.Xml.XmlQualifiedName(string.Empty) }));
                return stream.ToString();
            }
        }

        public static T Deserialize<T>(this string xml)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                Type type = typeof(T);
                XmlSerializer serializer = new XmlSerializer(type);

                byte[] buffer = new UTF8Encoding().GetBytes(xml);
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0;

                return (T)Convert.ChangeType(serializer.Deserialize(stream), type);
            }
        }

        public static string FromBase64String(this string value)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(value));
        }

        public static string ToBase64String(this string value)
        {
            return (string.IsNullOrWhiteSpace(value))? string.Empty: System.Text.Encoding.UTF8.GetBytes(value).ToBase64String();
        }

        public static string ToBase64String(this byte[] buffer)
        {
            string output = System.Convert.ToBase64String(buffer);
            foreach (string stringToRemove in Constants.Protocol.Base64EncodingRemoveStrings)
            {
                output = output.Replace(stringToRemove, string.Empty);
            }
            return output;
        }
    }
}
