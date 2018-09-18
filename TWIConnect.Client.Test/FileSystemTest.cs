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
      Assert.AreEqual(path, info[Constants.Configuration.Path]);
      Assert.IsNotNull(info[Constants.Configuration.FileContent]);
      //Assert.AreEqual(config.LocationKey, info[Constants.Configuration.LocationKey]);
      //Assert.AreEqual(config.DerivedMachineHash, info[Constants.Configuration.DerivedMachineHash]);
      //Assert.AreEqual(config.SequenceId, info[Constants.Configuration.SequenceId]);
      Assert.AreEqual(Constants.ObjectType.File, info[Constants.Configuration.ObjectType]);
      Assert.IsTrue(((string)info[Constants.Configuration.FileContent]).Length > 0);
      Assert.IsTrue(((long)info[Constants.Configuration.FileSize]) > 0);
      Assert.IsTrue(((DateTime)info[Constants.Configuration.Modified]) < DateTime.UtcNow);
    }

    [TestMethod]
    public void LoadFolderInfo()
    {
    //"ObjectType": "Folder",
    //"FolderSize": "1234578",
    //"SubFoldersCount": "35",
    //"FilesCount": "3",
    //"SubFolders": [
    //  {"Path": "C:\\Temp\\_1"},
    //  {"Path": "C:\\Temp\\_2}"
    //],
    //"Files": [
    //  { "Path": "C:\\Temp\\_1\\aa.txt" },
    //  { "Path": "C:\\Temp\\_2\\bb.tmp" }
    //],
    //"Path": "C:\\Temp",
    //"Modified": "2013-01-01-T00:00:00"
      var config = Configuration.Load();
      const string path = @"C:\Windows\Help";
      var info = Utilities.FileSystem.Load(config, path);

      Assert.AreEqual(Constants.ObjectType.Folder, info[Constants.Configuration.ObjectType]);
      Assert.IsTrue(((double)info[Constants.Configuration.FolderSize]) > 0);
      Assert.IsTrue(((int)info[Constants.Configuration.SubFoldersCount]) > 0);
      Assert.IsTrue(((int)info[Constants.Configuration.FilesCount]) > 0);
      Assert.IsTrue(((IEnumerable<IDictionary<string, object>>)info[Constants.Configuration.Files]).Count() > 0);
      Assert.AreEqual(path, info[Constants.Configuration.Path]);
      IEnumerable<IDictionary<string, object>> subFolders = info[Constants.Configuration.SubFolders] as IEnumerable<IDictionary<string, object>>;
      Assert.IsNotNull(subFolders.Where(sf => (string)sf[Constants.Configuration.Path] == path + @"\Help"));
      Assert.IsTrue(((DateTime)info[Constants.Configuration.Modified]) > DateTime.MinValue);

    }
  }
}
