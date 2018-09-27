using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class RestClientTest
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
    public void PostFile()
    {
      var configuration = FileConfiguration.FromFile("/../../Data/FileConfiguration.json");
      var request = Utilities.FileSystem.LoadFile(configuration);
      var response = RestClient.PostJson<dynamic>(configuration.Uri, request);
      Assert.IsNotNull(response);

    }
  }
}
