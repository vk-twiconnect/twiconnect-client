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
      var config = FileConfiguration.FromFile("/../../Data/FileConfiguration.json");
      var info = Utilities.FileSystem.LoadFile(config);
      Assert.AreEqual(config.Path, info[Constants.Configuration.Path]);
      Assert.IsNotNull(info[Constants.Configuration.FileContent]);
      Assert.AreEqual(Constants.ObjectType.File, info[Constants.Configuration.ObjectType]);
      Assert.IsTrue(((string)info[Constants.Configuration.FileContent]).Length > 0);
      Assert.IsTrue(((long)info[Constants.Configuration.FileSize]) > 0);
      Assert.IsTrue(((DateTime)info[Constants.Configuration.Modified]) < DateTime.UtcNow);
      Assert.AreEqual((int)info[Constants.Configuration.ExitCode], 0);
      Assert.IsTrue(((string)info[Constants.Configuration.ErrorMessage]).Length == 0);
    }

    [TestMethod]
    public void LoadFileNotFound()
    {
      var config = FileConfiguration.FromFile("/../../Data/FileConfiguration.json");
      config.Path = "Z:\\Dummy.zzz";
      var info = Utilities.FileSystem.LoadFile(config);
      Assert.AreEqual(config.Path, info[Constants.Configuration.Path]);
      Assert.IsTrue(string.IsNullOrEmpty(info[Constants.Configuration.FileContent].ToString()));
      Assert.AreEqual(Constants.ObjectType.File, info[Constants.Configuration.ObjectType]);
      Assert.IsTrue(((string)info[Constants.Configuration.FileContent]).Length == 0);
      Assert.IsTrue((long)info[Constants.Configuration.FileSize] == 0L);
      Assert.IsNull((info[Constants.Configuration.Modified]));
      Assert.AreEqual((int)info[Constants.Configuration.ExitCode], 1);
      Assert.IsTrue(((string)info[Constants.Configuration.ErrorMessage]).Length > 0);
    }

    [TestMethod]
    public void LoadFolderInfo()
    {
      var config = FolderConfiguration.FromFile("/../../Data/FolderConfiguration.json");
      var info = Utilities.FileSystem.LoadFolderMetaData(config);

      Assert.AreEqual(Constants.ObjectType.Folder, info[Constants.Configuration.ObjectType]);
      Assert.IsTrue(((double)info[Constants.Configuration.FolderSize]) > 0);
      Assert.IsTrue(((int)info[Constants.Configuration.SubFoldersCount]) > 0);
      Assert.IsTrue(((int)info[Constants.Configuration.FilesCount]) > 0);
      Assert.IsTrue(((IEnumerable<IDictionary<string, object>>)info[Constants.Configuration.Files]).Count() > 0);
      Assert.AreEqual(config.Path, info[Constants.Configuration.Path]);
      IEnumerable<IDictionary<string, object>> subFolders = info[Constants.Configuration.SubFolders] as IEnumerable<IDictionary<string, object>>;
      Assert.IsNotNull(subFolders.Where(sf => (string)sf[Constants.Configuration.Path] == config.Path + @"\Help"));
      Assert.IsTrue(((DateTime?)info[Constants.Configuration.Modified]) > DateTime.MinValue);
      Assert.AreEqual((int)info[Constants.Configuration.ExitCode], 0);
      Assert.IsTrue(((string)info[Constants.Configuration.ErrorMessage]).Length == 0);
    }

    [TestMethod]
    public void LoadFolderInfoNotFound()
    {
      var config = FolderConfiguration.FromFile("/../../Data/FolderConfiguration.json");
      config.Path = "Z:\\Dummy";
      var info = Utilities.FileSystem.LoadFolderMetaData(config);

      Assert.AreEqual(Constants.ObjectType.Folder, info[Constants.Configuration.ObjectType]);
      Assert.IsTrue(((double)info[Constants.Configuration.FolderSize]) == 0);
      Assert.IsTrue(((int)info[Constants.Configuration.SubFoldersCount]) == 0);
      Assert.IsTrue(((int)info[Constants.Configuration.FilesCount]) == 0);
      Assert.IsNull(info[Constants.Configuration.Files]);
      Assert.AreEqual(config.Path, info[Constants.Configuration.Path]);
      Assert.IsNull(info[Constants.Configuration.SubFolders]);
      Assert.IsNull(info[Constants.Configuration.Modified]);
      Assert.AreEqual((int)info[Constants.Configuration.ExitCode], 1);
      Assert.IsTrue(((string)info[Constants.Configuration.ErrorMessage]).Length > 0);
    }
  }
}
