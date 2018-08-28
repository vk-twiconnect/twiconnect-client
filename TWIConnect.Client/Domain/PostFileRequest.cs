using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TWIConnect.Client.Utilities;

namespace TWIConnect.Client.Domain
{
    /*
    <?xml version="1.0" encoding="utf-8"?>
    <request>
      <!-- mac address of the first connected network adapter ->
      <mac>0060ef217868</mac>
      <!-- string identifier for client installation. -->
      <locationkey>123</locationkey>
      <filecontent>
        <![CDATA[base64string]]>
      </filecontent>
      <filesize>1234578</filesize>
      <file>
        <name>
            <![CDATA[C:\Temp\Configuration.xml]]>
        </name>
      </file>
      <filestamp>2013-01-01-T00:00:00</filestamp>
    </request>
    */
    [XmlRoot("request")]
    public class PostFileRequest : BaseRequest
    {
        public PostFileRequest()
        {
        }

        public PostFileRequest(Domain.Configuration configuration, Domain.File file)
        {
            LocationKey = configuration.LocationKey;
            SequenceId = configuration.SequenceId;
            FileContent = file.Content;
            FileSizeBytes = file.SizeBytes;
            Name = file.Name;
            FileTimeStamp = file.TimeStampUtc;
            SequenceId = configuration.SequenceId;
        }

        [XmlIgnore]
        public string FileContent { get; set; }

        [XmlElement("FileContent")]
        public XmlCDataSection CDataFileContent
        {
            get
            {
                return (new XmlDocument()).CreateCDataSection(this.FileContent);
            }
            set
            {
                this.FileContent = value.Value;
            }
        }

        public long FileSizeBytes { get; set; }

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

        public DateTime FileTimeStamp { get; set; }

        public string SequenceId { get; set; }

    }
}
