using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class ConfigurationTest
  {
    [TestMethod]
    public void LoadFromString()
    {
      Configuration configuration = Configuration.Load();
      Assert.IsNotNull(configuration);
      Assert.AreEqual("New Site Install", configuration.LocationKey);
    }

    [TestMethod]
    public void SaveToString()
    {
      Configuration configuration = Configuration.Load();
      string json = configuration.ToString();
      Assert.IsNotNull(json);
      Assert.IsTrue(json.Contains("New Site Install"));
    }

    [TestMethod]
    public void PostConfiguration()
    {
      var configuration = Configuration.FromFile("/../../Data/Configuration.json");
      var response = RestClient.PostJson<dynamic>(configuration.Uri, configuration);
      Assert.IsNotNull(response);
      Assert.AreEqual(Constants.ObjectType.None, response.ObjectType.ToString());
    }
  }
}
