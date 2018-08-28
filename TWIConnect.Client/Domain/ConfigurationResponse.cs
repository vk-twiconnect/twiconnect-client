using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TWIConnect.Client.Utilities;

namespace TWIConnect.Client.Domain
{
    [XmlRoot("Response")]
    public class ConfigurationResponse
    {
        public string LocationKey { get; set; }
        public int? ScheduledIntervalSec { get; set; }
        public int? FileSizeLimitMb { get; set; }
        public int? ImmutabilityIntervalSec { get; set; }
        public int? MaxThreads { get; set; }
        public int? ThreadTimeToLiveSec { get; set; }
        public HashSet<FileSettings> Files { get; set; }
        public string SequenceId { get; set; }
        
        [XmlIgnore]
        public string UrlPostFile { get; set; }

        [XmlElement("UrlPostFile")]
        public XmlCDataSection CDataUrlPostFile
        {
            get
            {
                return (new XmlDocument()).CreateCDataSection(this.UrlPostFile.ToBase64String());
            }
            set
            {
                this.UrlPostFile = value.Value.FromBase64String();
            }
        }

        public string RunCommand { get; set; }
        public string RunCommandArgs { get; set; }
    }
}
