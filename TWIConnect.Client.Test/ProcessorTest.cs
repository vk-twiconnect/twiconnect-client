using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class ProcessorTest
  {
    private void validateCommonFields(IDictionary<string, object> response)
    {
      //{,"Uri":"http:\/\/68.199.43.204:6002\/polling\/pollrequest_case_0.htm?ver=1538242412"}

      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.LocationKey].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.SequenceId].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.Uri].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.ScheduledIntervalSec].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.MaxThreads].ToString()));
      Assert.IsFalse(string.IsNullOrWhiteSpace(response[Constants.Configuration.ThreadTimeToLiveSec].ToString()));
    }

    [TestMethod]
    public void PostConfiguration()
    {
      var configuration = Configuration.FromFile("/../../Data/Configuration.json");
      var response = RestClient.PostJson<Dictionary<string, object>>(configuration.Uri, configuration);
      Assert.IsNotNull(response);
      Assert.AreEqual(Constants.ObjectType.None, response[Constants.Configuration.ObjectType].ToString());
      validateCommonFields(response);
    }

    [TestMethod]
    public void PostFileData()
    {
      var configuration = FileConfiguration.FromFile("/../../Data/FileConfiguration.json");
      var request = Utilities.FileSystem.LoadFile(configuration);
      request = Processor.AddCommonRequestFields(configuration, request);
      Assert.AreEqual(configuration.DerivedMachineHash, request[Constants.Configuration.DerivedMachineHash].ToString());
      Assert.AreEqual(configuration.LocationKey, request[Constants.Configuration.LocationKey].ToString());
      Assert.AreEqual(configuration.SequenceId, request[Constants.Configuration.SequenceId].ToString());
      var response = RestClient.PostJson<Dictionary<string, object>>(configuration.Uri, request);
      Assert.IsNotNull(response);
      Assert.AreEqual(Constants.ObjectType.File, response[Constants.Configuration.ObjectType].ToString());
    }

    [TestMethod]
    public void PostFolderMetaData()
    {
      var configuration = FolderConfiguration.FromFile("/../../Data/FolderConfiguration.json");
      var request = Utilities.FileSystem.LoadFolderMetaData(configuration);
      request = Processor.AddCommonRequestFields(configuration, request);
      Assert.AreEqual(configuration.DerivedMachineHash, request[Constants.Configuration.DerivedMachineHash].ToString());
      Assert.AreEqual(configuration.LocationKey, request[Constants.Configuration.LocationKey].ToString());
      Assert.AreEqual(configuration.SequenceId, request[Constants.Configuration.SequenceId].ToString());
      var response = RestClient.PostJson<Dictionary<string, object>>(configuration.Uri, request);
      Assert.IsNotNull(response);
      Assert.AreEqual(Constants.ObjectType.Folder, response[Constants.Configuration.ObjectType].ToString());
    }

    [TestMethod]
    public void PostCommandData()
    {
      var configuration = CommandConfiguration.FromFile("/../../Data/CommandConfiguration.json");
      var request = Utilities.Process.ExecuteCommand(configuration);
      request = Processor.AddCommonRequestFields(configuration, request);
      Assert.AreEqual(configuration.DerivedMachineHash, request[Constants.Configuration.DerivedMachineHash].ToString());
      Assert.AreEqual(configuration.LocationKey, request[Constants.Configuration.LocationKey].ToString());
      Assert.AreEqual(configuration.SequenceId, request[Constants.Configuration.SequenceId].ToString());
      var response = RestClient.PostJson<dynamic>(configuration.Uri, request);
      Assert.IsNotNull(response);
      Assert.AreEqual(Constants.ObjectType.Command, response[Constants.Configuration.ObjectType].ToString());
    }
  }
}
