using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TWIConnect.Client
{
  public static class Processor
  {
    public static void Run()
    {
      Utilities.Logger.Log(Resources.Messages.ProcessStarted);
      System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();

      try
      {
        var configuration = Configuration.Load();

        #region Send Request & Load Response
        JObject response = SendReqesut(configuration, configuration);
        string objectType = response.Property(Constants.Configuration.ObjectType).Value.ToString();
        #endregion

        while (string.Compare(objectType, Constants.ObjectType.None, StringComparison.InvariantCultureIgnoreCase) == 0)
        {
          IDictionary<string, object> request = null;

          switch (objectType)
          {
            case Constants.ObjectType.Command:
              var commandConfig = CommandConfiguration.FromJObject(response);
              request = Utilities.Threading.AsyncCallWithTimeout<IDictionary<string, object>>
              (
                () => Utilities.Process.ExecuteCommand(commandConfig),
                (int)(configuration.ThreadTimeToLiveSec * 1000)
              );
              break;

            case Constants.ObjectType.File:
              var fileConfig = FileConfiguration.FromJObject(response);
              request = Utilities.Threading.AsyncCallWithTimeout<IDictionary<string, object>>
              (
                () => Utilities.FileSystem.LoadFile(fileConfig),
                (int)(configuration.ThreadTimeToLiveSec * 1000)
              );
              break;

            case Constants.ObjectType.Folder:
              var folderConfig = FolderConfiguration.FromJObject(response);
              request = Utilities.Threading.AsyncCallWithTimeout<IDictionary<string, object>>
              (
                () => Utilities.FileSystem.LoadFolderMetaData(folderConfig),
                (int)(configuration.ThreadTimeToLiveSec * 1000)
              );
              break;

            default:
              throw new InvalidOperationException("Invalid ObjectType found in the response from the server: '" + objectType + "'");
          }

          request = AddCommonRequestFields(configuration, request);
          response = SendReqesut(configuration, configuration);
          objectType = response.Property(Constants.Configuration.ObjectType).Value.ToString();
        }

        var responseConfig = Configuration.FromJObject(response);
        Utilities.Threading.AsyncCallWithTimeout
        (
          () => responseConfig.Save(),
          (int)(responseConfig.ThreadTimeToLiveSec * 1000)
        );

        Utilities.Logger.Log(Resources.Messages.ProcessSucceeded, Utilities.Logger.GetTimeElapsed(stopWatch));
      }
      catch (Exception ex)
      {
        Utilities.Logger.Log(Resources.Messages.ProcessFailed);
        //No action - just quit
      }
    }

    public static IDictionary<string, object> AddCommonRequestFields(Configuration configuration, IDictionary<string, object> request)
    {
      //Clone original object
      request = new Dictionary<string, object>(request);
      request.Add(Constants.Configuration.LocationKey, configuration.LocationKey);
      request.Add(Constants.Configuration.DerivedMachineHash, configuration.DerivedMachineHash);
      request.Add(Constants.Configuration.SequenceId, configuration.SequenceId);
      return request;
    }

    public static JObject SendReqesut(Configuration configuration, object request)
    {
      var response = Utilities.Threading.AsyncCallWithTimeout<JObject>
              (
                () => RestClient.PostJson<JObject>(configuration.Uri, configuration),
                (int)(configuration.ThreadTimeToLiveSec * 1000)
              );
      return response;
    }
  }
}