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
    }

    [TestMethod]
    public void SaveToString()
    {
      Configuration configuration = Configuration.Load();
      string json = configuration.ToString();
      Assert.IsNotNull(json);
      Assert.IsTrue(json.Contains("Location"));
    }
  }
}
