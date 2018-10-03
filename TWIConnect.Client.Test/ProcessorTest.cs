using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class ProcessorTest
  {
    private void validateCommonRequestFields(IDictionary<string, object> dict)
    {
      Assert.IsFalse(string.IsNullOrWhiteSpace(dict[Constants.Configuration.LocationKey].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(dict[Constants.Configuration.SequenceId].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(dict[Constants.Configuration.DerivedMachineHash].ToString()));
    }

    private void validateCommonResponseFields(IDictionary<string, object> dict)
    {
      Assert.IsFalse(string.IsNullOrWhiteSpace(dict[Constants.Configuration.LocationKey].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(dict[Constants.Configuration.SequenceId].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(dict[Constants.Configuration.Uri].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(dict[Constants.Configuration.ScheduledIntervalSec].ToString()));
    }

    [TestMethod]
    public void PostConfiguration()
    {
      var configuration = Configuration.FromFile("./Data/Configuration.json");
      var response = Utilities.RestClient.PostJson<Dictionary<string, object>>(configuration.Uri, configuration);

      validateCommonResponseFields(response);
      Assert.AreEqual(Constants.ObjectType.None, response[Constants.Configuration.ObjectType].ToString());
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.Uri].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.ScheduledIntervalSec].ToString()));
    }

    [TestMethod]
    public void PostFileData()
    {
      var configuration = FileConfiguration.FromFile("./Data/FileConfiguration.json");
      var request = Processor.LoadFile(configuration);

      #region Validate Request
      validateCommonRequestFields(request);
      Assert.AreEqual(Constants.ObjectType.File, request[Constants.Configuration.ObjectType].ToString());
      Assert.IsFalse(string.IsNullOrWhiteSpace(request[Constants.Configuration.FileContent].ToString()));
      Assert.IsTrue((int.Parse(request[Constants.Configuration.FileSize].ToString())) > 0);
      Assert.IsTrue(
        string.Compare(
          request[Constants.Configuration.Path].ToString(),
          configuration.Path, System.StringComparison.CurrentCultureIgnoreCase) == 0
      );
      Assert.IsTrue(((DateTime)request[Constants.Configuration.Modified]) > DateTime.MinValue);
      #endregion

      var response = Utilities.RestClient.PostJson<Dictionary<string, object>>(configuration.Uri, request);

      #region Validate Response
      Assert.AreEqual(Constants.ObjectType.File, response[Constants.Configuration.ObjectType].ToString());
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.ThreadTimeToLiveSec].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.ImmutabilityIntervalSec].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.FileSizeLimitMb].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.IgnoreSizeLimit].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.IgnoreImmutabilityInterval].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.SendVersionAfterTimeStampUtc].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.Path].ToString()));
      validateCommonResponseFields(response);
      #endregion
    }

    [TestMethod]
    public void PostFolderMetaData()
    {
      var configuration = FolderConfiguration.FromFile("./Data/FolderConfiguration.json");
      var request = Processor.LoadFolderMetaData(configuration);
      #region Validate Request
      validateCommonRequestFields(request);
      Assert.AreEqual(Constants.ObjectType.Folder, request[Constants.Configuration.ObjectType].ToString());
      Assert.IsTrue(
        string.Compare(
          request[Constants.Configuration.Path].ToString(),
          configuration.Path, System.StringComparison.CurrentCultureIgnoreCase) == 0
      );
      Assert.IsTrue((int.Parse(request[Constants.Configuration.FolderSize].ToString())) > 0);
      Assert.IsTrue((int.Parse(request[Constants.Configuration.SubFoldersCount].ToString())) > 0);
      Assert.IsTrue((int.Parse(request[Constants.Configuration.FilesCount].ToString())) > 0);
      Assert.IsTrue(
        string.Compare(
          request[Constants.Configuration.Path].ToString(),
          configuration.Path, System.StringComparison.CurrentCultureIgnoreCase) == 0
      );
      Assert.IsTrue(((DateTime)request[Constants.Configuration.Modified]) > DateTime.MinValue);
      Assert.IsTrue(
        ((IEnumerable<IDictionary<string, object>>)request[Constants.Configuration.SubFolders]).Count() > 0
      );
      Assert.IsTrue(
        ((IEnumerable<IDictionary<string, object>>)request[Constants.Configuration.Files]).Count() > 0
      );
      #endregion

      var response = Utilities.RestClient.PostJson<Dictionary<string, object>>(configuration.Uri, request);

      #region Validate Response
      Assert.AreEqual(Constants.ObjectType.Folder, response[Constants.Configuration.ObjectType].ToString());
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.ThreadTimeToLiveSec].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.Path].ToString()));
      validateCommonResponseFields(response);
      #endregion
    }

    [TestMethod]
    public void PostCommandData()
    {
      var configuration = CommandConfiguration.FromFile("./Data/CommandConfiguration.json");
      var request = Processor.ExecuteCommand(configuration);

      #region Validate Request
      validateCommonRequestFields(request);
      Assert.AreEqual(configuration.CommandLine, request[Constants.Configuration.CommandLine]);
      Assert.AreEqual(configuration.CommandArguments, request[Constants.Configuration.CommandArguments]);
      Assert.IsTrue(request[Constants.Configuration.CommandOutput].ToString().Length > 0);
      Assert.IsNotNull(request[Constants.Configuration.CommandExitCode]);
      #endregion

      var response = Utilities.RestClient.PostJson<Dictionary<string, object>>(configuration.Uri, request);

      #region Validate Response
      Assert.AreEqual(Constants.ObjectType.Command, response[Constants.Configuration.ObjectType].ToString());
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.ThreadTimeToLiveSec].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.CommandLine].ToString()));
      validateCommonResponseFields(response);
      #endregion
    }

    [TestMethod]
    public void ObjectTypeFile()
    {
      var path = Utilities.FileSystem.GetCurrentFolderName() + "./Data/FileConfiguration.json";
      var configuration = FileConfiguration.FromFile(path);
      Processor.ClientServerLoop(configuration);
    }

    [TestMethod]
    public void ObjectTypeFolder()
    {
      var configuration = FolderConfiguration.FromFile("./Data/FolderConfiguration.json");
      Processor.ClientServerLoop(configuration);
    }

    [TestMethod]
    public void ObjectTypeCommand()
    {
      var configuration = CommandConfiguration.FromFile("./Data/CommandConfiguration.json");
      Processor.ClientServerLoop(configuration);
    }

    [TestMethod]
    public void ObjectTypeNone()
    {
      var configuration = Configuration.FromFile("./Data/Configuration.json");
      Processor.ClientServerLoop(configuration);
    }
  }
}
