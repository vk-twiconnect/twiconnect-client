using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TWIConnect.Client;
using System.Collections.Generic;
using System.Linq;

namespace TWIConnect.Client.Test
{
  [TestClass]
  public class ProcessTest
  {
    [TestMethod]
    public void ExecuteCommand()
    {
      var config = CommandConfiguration.FromFile("/../../Data/CommandConfiguration.json");
      var info = Utilities.Process.ExecuteCommand(config);
      Assert.AreEqual(config.CommandLine, info[Constants.Configuration.CommandLine]);
      Assert.AreEqual(config.CommandArguments, info[Constants.Configuration.CommandArguments]);
      Assert.IsTrue(info[Constants.Configuration.CommandOutput].ToString().Length > 0);
      Assert.IsNotNull(info[Constants.Configuration.CommandExitCode]);
    }
  }
}
