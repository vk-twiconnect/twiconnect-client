using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TWIConnect.Client
{
  public class FileConfiguration : Configuration
  {
    public static FileConfiguration FromJObject(JObject jObject)
    {
      return jObject.ToObject<FileConfiguration>();
    }

    public static FileConfiguration FromFile(string path)
    {
      return Utilities.FileSystem.LoadObjectFromFile<FileConfiguration>(path);
    }

    public string ObjectType { get { return Constants.ObjectType.File; } }
    public int FileSizeLimitMb { get; set; }
    public int ImmutabilityIntervalSec { get; set; }
    public bool IgnoreSizeLimit { get; set; }
    public bool IgnoreImmutabilityInterval { get; set; }
    public DateTime? SendVersionAfterTimeStampUtc { get; set; }
    public string Path { get; set; }
  }
}
