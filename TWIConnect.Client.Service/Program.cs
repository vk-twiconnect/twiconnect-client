using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TWIConnect.Client.Service
{
  static class Program
  {
    static void Main(string[] args)
    {
      try
      {
        if ((args == null) || (args.Length == 0))
        {
          ServiceBase[] ServicesToRun;
          ServicesToRun = new ServiceBase[]
          {
            new Service()
          };
          ServiceBase.Run(ServicesToRun);
        }
        else if (args.Any(a => a.Equals("install", StringComparison.CurrentCultureIgnoreCase)))
        {
          ServiceInstaller.Install(args);
        }
        else if (args.Any(a => a.Equals("uninstall", StringComparison.CurrentCultureIgnoreCase)))
        {
          ServiceInstaller.Uninstall(args);
        }
        else if (args.Any(a => a.Equals("console", StringComparison.CurrentCultureIgnoreCase)))
        {
          (new Service()).Process(null, null);
        }
        else if (args.Any(a => a.Equals("help", StringComparison.CurrentCultureIgnoreCase)))
        {
          Console.WriteLine
          (
            "Options: \n" +
            "install - installs the Windows Service\n" +
            "uninstall - uninstalls the Windows service\n" +
            "help - prints out this message\n" +
            "console - triggers the processing." +
            "{none} - used by Windows Service only."
          );
        }
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(ex.Message);
      }
    }
  }
}
