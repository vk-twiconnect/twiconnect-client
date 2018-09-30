using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TWIConnect.Client
{
  public class FolderConfiguration : Configuration
  {
    public static FolderConfiguration FromFile(string path)
    {
      return Utilities.FileSystem.LoadObjectFromFile<FolderConfiguration>(path);
    }

    public string Path { get; set; }
    public string ObjectType { get { return Constants.ObjectType.Folder; } }

    internal static FolderConfiguration FromJObject(JObject jObject)
    {
      return jObject.ToObject<FolderConfiguration>();
    }
  }
}
