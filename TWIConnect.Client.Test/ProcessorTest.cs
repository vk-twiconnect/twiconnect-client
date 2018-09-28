using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class ProcessorTest
  {
    [TestMethod]
    public void PostConfiguration()
    {
      var configuration = Configuration.FromFile("/../../Data/Configuration.json");
      var response = RestClient.PostJson<dynamic>(configuration.Uri, configuration);
      Assert.IsNotNull(response);
      Assert.AreEqual(Constants.ObjectType.None, response.ObjectType.ToString());
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
      var response = RestClient.PostJson<dynamic>(configuration.Uri, request);
      Assert.IsNotNull(response);
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
      var response = RestClient.PostJson<dynamic>(configuration.Uri, request);
      Assert.IsNotNull(response);
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
    }
  }
}
