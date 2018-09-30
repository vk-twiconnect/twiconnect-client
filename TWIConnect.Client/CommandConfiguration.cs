using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TWIConnect.Client
{
  public class CommandConfiguration : Configuration
  {
    public static CommandConfiguration FromJObject(JObject jObject)
    {
      return jObject.ToObject<CommandConfiguration>();
    }

    public static CommandConfiguration FromFile(string path)
    {
      return Utilities.FileSystem.LoadObjectFromFile<CommandConfiguration>(path);
    }

    public string ObjectType { get { return Constants.ObjectType.Command; } }
    public string CommandLine { get; set; }
    public string CommandArguments { get; set; }
  }
}
