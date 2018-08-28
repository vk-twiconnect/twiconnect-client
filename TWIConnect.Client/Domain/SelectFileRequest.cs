using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TWIConnect.Client.Utilities;

namespace TWIConnect.Client.Domain
{
    [XmlRoot("request")]
    public class SelectFileRequest : BaseRequest
    {
        public SelectFileRequest() { }

        public SelectFileRequest(Domain.Configuration configuration, Domain.FileSettings fileSettings, IEnumerable<Domain.File> fileInfos)
        {
            this.InstallationId = configuration.InstallationId;
            this.LocationKey = configuration.LocationKey;
            this.FolderName = fileSettings.Name;
            this.Files = new HashSet<File>(fileInfos);
        }

        public string FolderName { get; set; }
        public HashSet<Domain.File> Files { get; set; }

        public int FileCount
        {
            get {  return (this.Files != null)? this.Files.Count(): 0;}
            set { /*ignore*/ }
        }
    }
}
