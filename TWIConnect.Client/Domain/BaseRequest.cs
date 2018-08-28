using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TWIConnect.Client.Domain
{
    public abstract class BaseRequest
    {
        [XmlElement("DerivedMachineHash")]
        public string InstallationId
        {
            get
            {
                return Configuration.GetInstallationId();
            }
            set
            {
                //Ignore input
                //this._installationId = value;
            }
        }

        public string LocationKey { get; set; }
    }
}
