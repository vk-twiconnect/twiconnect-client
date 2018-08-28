using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TWIConnect.Client.Domain
{
    /// <summary>
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <request>
    ///  <!-- mac address of the first connected network adapter -->
    ///  <mac>0060ef217868</mac> 
    ///  <!-- string identifier for client installation, can be empty on first request from the client to the server -->
    ///  <locationkey>123</locationkey>
    /// </request>    
    /// </summary>
    [XmlRoot("request")]
    public class ConfigurationRequest : BaseRequest
    {
        public ConfigurationRequest()
        {
        }

        public ConfigurationRequest(Configuration localConfiguration)
        {
            LocationKey = localConfiguration.LocationKey;
        }
    }
}
