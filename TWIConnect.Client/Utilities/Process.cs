using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TWIConnect.Client.Utilities
{
  public class Process
  {
    public static IDictionary<string, object> ExecuteCommand(CommandConfiguration config)
    {
      ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
      {
        UseShellExecute = false,
        //CreateNoWindow = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        FileName = config.CommandLine,
        Arguments = config.CommandArguments
      };

      System.Diagnostics.Process process = new System.Diagnostics.Process
      {
        StartInfo = startInfo
      };
      process.Start();

      string output = process.StandardOutput.ReadToEnd();
      string error = process.StandardError.ReadToEnd();

      process.WaitForExit();

      return new Dictionary<string, object>()
      {
        { Constants.Configuration.ObjectType, Constants.ObjectType.Command },
        { Constants.Configuration.CommandLine, config.CommandLine },
        { Constants.Configuration.CommandArguments, config.CommandArguments },
        { Constants.Configuration.CommandExitCode, process.ExitCode },
        { Constants.Configuration.CommandOutput, string.IsNullOrWhiteSpace(output)? error: output  }
      };
    }
  }
}
