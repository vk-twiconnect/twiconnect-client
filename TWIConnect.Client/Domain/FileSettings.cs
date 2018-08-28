using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TWIConnect.Client.Utilities;

namespace TWIConnect.Client.Domain
{
    [XmlType("file")]
    public class FileSettings
    {
        [XmlIgnore]
        public string Name { get; set; }

        [XmlElement("name")]
        public XmlCDataSection CDataName
        {
            get
            {
                return (new XmlDocument()).CreateCDataSection(this.Name.ToBase64String());
            }
            set
            {
                this.Name = value.Value.FromBase64String();
            }
        }

        public bool IgnoreSizeLimit { get; set; }
        public bool IgnoreImmutabilityInterval { get; set; }

        public override string ToString()
        {
            string template = "Name: '{0}', SendVersionAfterTimeStampUtc: '{1}'";
            return string.Format(template, this.Name, this.SendVersionAfterTimeStampUtc);
        }

        public DateTime? SendVersionAfterTimeStampUtc { get; set; }
    }
}
