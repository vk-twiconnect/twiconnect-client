using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TWIConnect.Client.Utilities;
using System.Xml;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class ConfigurationTest
  {
    [TestMethod]
    public void LoadFromString()
    {
      Domain.Configuration configuration = Domain.Configuration.Load();
      Assert.IsNotNull(configuration);
      Assert.AreEqual("New Site Install", configuration.LocationKey);
    }

    [TestMethod]
    public void SaveToString()
    {
      Domain.Configuration configuration = Domain.Configuration.Load();
      string json = configuration.ToString();
      Assert.IsNotNull(json);
      Assert.IsTrue(json.Contains("New Site Install"));
    }
  }
}
