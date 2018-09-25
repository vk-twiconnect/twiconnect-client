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
    //public static FileConfiguration FromJToken(JToken jToken)
    //{

    //  return jToken.ToObject<FileConfiguration>(jToken);
    //}

    public static FileConfiguration FromFile(string path)
    {
      return Utilities.FileSystem.LoadObjectFromFile<FileConfiguration>(path);
    }

    public const string ObjectType = Constants.ObjectType.File;
    public int FileSizeLimitMb { get; set; }
    public int ImmutabilityIntervalSec { get; set; }
    public bool IgnoreSizeLimit { get; set; }
    public bool IgnoreImmutabilityInterval { get; set; }
    public DateTime? SendVersionAfterTimeStampUtc { get; set; }
    public string Path { get; set; }
  }
}
