using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TWIConnect.Client;
using System.Collections.Generic;
using System.Linq;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class FileSystemTest
  {
    [TestMethod]
    public void LoadFileInfo()
    {
      var config = Configuration.Load();
      const string path = @"C:\Windows\Write.exe";
      var info = Utilities.FileSystem.Load(config, path);
      Assert.AreEqual(path, info["Path"]);
      Assert.IsNotNull(info["FileContent"]);
    }

    [TestMethod]
    public void LoadFolderInfo()
    {
      var config = Configuration.Load();
      const string path = @"C:\Windows\Help";
      var info = Utilities.FileSystem.Load(config, path);
      Assert.AreEqual(path, info["Path"]);
      IEnumerable<IDictionary<string, object>> subFolders = info["SubFolders"] as IEnumerable<IDictionary<string, object>>;
      Assert.IsNotNull(subFolders.Where(sf => (string)sf["Path"] == path));
    }
  }
}
