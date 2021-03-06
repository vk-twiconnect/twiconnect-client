﻿using Newtonsoft.Json.Linq;
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
        Processor.ClientServerLoop(Configuration.Load());
        Utilities.Logger.Log(Resources.Messages.ProcessSucceeded, Utilities.Logger.GetTimeElapsed(stopWatch));
      }
      catch (Exception ex)
      {
        Utilities.Logger.Log(NLog.LogLevel.Error, ex);
        //No action - just quit
      }
    }

    public static void ClientServerLoop(Configuration configuration)
    {
      JObject response = SendReqesut(configuration, configuration);
      string objectType = string.Empty;
      IDictionary<string, object> request = null;
      Configuration newConfiguration = null;

      while (true)
      {
        objectType = response.Property(Constants.Configuration.ObjectType).Value.ToString();

        switch (objectType)
        {
          case Constants.ObjectType.None:
            //Update configuration
            newConfiguration = Configuration.FromJObject(response);

            // Replace machine hash with previous value
            newConfiguration.DerivedMachineHash = configuration.DerivedMachineHash;

            //Save new configuration to disk
            Utilities.Threading.AsyncCallWithTimeout
            (
              () => newConfiguration.Save(),
              (int)(newConfiguration.ThreadTimeToLiveSec * 1000)
            );

            //Exit loop
            return;

          case Constants.ObjectType.Command:
            var commandConfig = CommandConfiguration.FromJObject(response);
            SelectiveUpdateLocalConfiguration(configuration, commandConfig);
            request = Processor.ExecuteCommand(commandConfig);
            newConfiguration = commandConfig;
            break;

          case Constants.ObjectType.File:
            var fileConfig = FileConfiguration.FromJObject(response);
            SelectiveUpdateLocalConfiguration(configuration, fileConfig);
            request = Processor.LoadFile(fileConfig);
            newConfiguration = fileConfig;
            break;

          case Constants.ObjectType.Folder:
            var folderConfig = FolderConfiguration.FromJObject(response);
            SelectiveUpdateLocalConfiguration(configuration, folderConfig);
            request = Processor.LoadFolderMetaData(folderConfig);
            newConfiguration = folderConfig;
            break;

          default:
            throw new InvalidOperationException("Invalid ObjectType found in the response from server: '" + objectType + "'");
        }

        //Pause before sending next request
        System.Threading.Thread.Sleep(newConfiguration.ScheduledIntervalSec);

        //Send next request for command/file/folderMetaData cases
        response = SendReqesut(newConfiguration, request);
      }
    }

    private static void SelectiveUpdateLocalConfiguration(Configuration configuration, Configuration newConfiguration)
    {
      //Update ScheduledIntervalSec
      configuration.ScheduledIntervalSec = newConfiguration.ScheduledIntervalSec;

      //Update SequenceId
      configuration.SequenceId = newConfiguration.SequenceId;

      //Save local configuration
      Utilities.Threading.AsyncCallWithTimeout
      (
        () => configuration.Save(),
        (int)(newConfiguration.ThreadTimeToLiveSec * 1000)
      );
    }

    public static IDictionary<string, object> AddCommonRequestFields(Configuration configuration, IDictionary<string, object> request)
    {
      //Clone original object
      request = new Dictionary<string, object>(request)
      {
        { Constants.Configuration.LocationKey, configuration.LocationKey },
        { Constants.Configuration.DerivedMachineHash, configuration.DerivedMachineHash },
        { Constants.Configuration.SequenceId, configuration.SequenceId }
      };
      return request;
    }

    public static IDictionary<string, object> LoadFile(FileConfiguration configuration)
    {
      var fileContent = Utilities.Threading.AsyncCallWithTimeout<IDictionary<string, object>>
      (
        () => Utilities.FileSystem.LoadFile(configuration),
        (int)(configuration.ThreadTimeToLiveSec * 1000)
      );

      fileContent = AddCommonRequestFields(configuration, fileContent);
      return fileContent;
    }

    public static IDictionary<string, object> LoadFolderMetaData(FolderConfiguration configuration)
    {
      var folderMetaData = Utilities.Threading.AsyncCallWithTimeout<IDictionary<string, object>>
      (
        () => Utilities.FileSystem.LoadFolderMetaData(configuration),
        (int)(configuration.ThreadTimeToLiveSec * 1000)
      );

      folderMetaData = AddCommonRequestFields(configuration, folderMetaData);
      return folderMetaData;
    }

    public static IDictionary<string, object> ExecuteCommand(CommandConfiguration configuration)
    {
      var commandResult = Utilities.Threading.AsyncCallWithTimeout<IDictionary<string, object>>
      (
        () => Utilities.Process.ExecuteCommand(configuration),
        (int)(configuration.ThreadTimeToLiveSec * 1000)
      );

      commandResult = AddCommonRequestFields(configuration, commandResult);
      return commandResult;
    }

    public static JObject SendReqesut(Configuration configuration, object request)
    {
      var response = Utilities.Threading.AsyncCallWithTimeout<JObject>
              (
                () => Utilities.RestClient.PostJson<JObject>(configuration.Uri, request),
                (int)(configuration.ThreadTimeToLiveSec * 1000)
              );
      return response;
    }
  }
}