//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TWIConnect.Client
//{
//    public class Processor
//    {
//        private static volatile int _filesProcessed = 0;
//        private static volatile int _filesUploaded = 0;

//        public static void Run(Configuration configuration)
//        {
//            try
//            {
//                if (configuration == null)
//                    throw new ArgumentNullException("configuration");

//                Utilities.Logger.Log(Resources.Messages.ProcessStarted);
//                System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
//                Processor processor = new Processor();
//                Processor._filesProcessed = Processor._filesUploaded = 0;

//                //Start on separate thread to ensure process does not gets stack but rather thread is aborted
//                Utilities.Threading.AsyncCallWithTimeout
//                        (
//                            () => {
//                                processor.ProcessFiles(configuration);
//                                processor.RunCommand(configuration);
//                            },
//                            (int)(configuration.Files.Count() * configuration.ThreadTimeToLiveSec * 1000 * 1.1)
//                        );

//                Utilities.Logger.Log(Resources.Messages.ProcessSucceeded, Utilities.Logger.GetTimeElapsed(stopWatch), Processor._filesProcessed, Processor._filesUploaded);
//            }
//            catch
//            {
//                Utilities.Logger.Log(Resources.Messages.ProcessFailed);
//            }
//        }

//        private void RunCommand(Configuration configuration)
//        {
//            try
//            {
//                if (!string.IsNullOrWhiteSpace(configuration.RunCommand))
//                {
//                    Utilities.Logger.Log("Running commands has been disabled for security reasons: " + configuration.RunCommand);
//                    //Process.Start(configuration.RunCommand, configuration.RunCommandArgs);
//                }
//            }
//            catch (Exception ex)
//            {
//                Utilities.Logger.Log(ex.Message);
//            }
//        }

//        private void ProcessFiles(Configuration configuration)
//        {
//            try
//            {
//                //Single threaded processing to support continuous firing of one file at the time
//                Configuration config = configuration;

//                while ((config != null) && (config.Files.Any()))
//                {
//                    config = this.ProcessFileAsync(configuration, config.Files.First());
//                }
//            }
//            catch (Exception ex)
//            {
//                Utilities.Logger.Log(ex.Message);
//            }
//        }

//        private Configuration ProcessFileAsync(Configuration configuration, FileSettings fileSettings)
//        {
//            try
//            {
//                Func<Configuration> operation = new Func<Configuration>(() => ProcessFile(configuration, fileSettings));
//                return Utilities.Threading.AsyncCallWithTimeout(operation, configuration.ThreadTimeToLiveSec * 1000);
//            }
//            catch (Exception ex)
//            {
//                Utilities.Logger.Log(ex);
//                return null;
//            }
//        }

//        private Configuration ProcessFile(Configuration configuration, FileSettings fileSettings)
//        {
//            try
//            {
//                var restClient = new RestClient(configuration);
//                ConfigurationResponse configurationResponse = null;

//                if (Utilities.FileSystem.IsDirectory(fileSettings.Name))
//                {
//                    var filesNames = Utilities.FileSystem.GetFilesList(fileSettings.Name);
//                    var files = filesNames.Select
//                                        (
//                                            fi =>
//                                                new File
//                                                {
//                                                    Content = null,
//                                                    Name = fi.FullName,
//                                                    SizeBytes = fi.Length,
//                                                    TimeStampUtc = fi.LastWriteTimeUtc,
//                                                }

//                                        );

//                    configurationResponse = restClient.SelectFile(new SelectFileRequest(configuration, fileSettings, files));
//                }
//                else
//                {

//                    Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartProcessFile, fileSettings.Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
//                    System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
//                    Processor._filesProcessed++;
//                    File file = File.Load(configuration, fileSettings);
//                    PostFileRequest postFileRequest = new PostFileRequest(configuration, file);
//                    configurationResponse = restClient.UploadFile(postFileRequest);
//                    Processor._filesUploaded++;
//                    Utilities.Logger.Log(NLog.LogLevel.Info, Resources.Messages.FileUploaded, fileSettings.Name, Utilities.Logger.GetTimeElapsed(stopWatch));
//                }

//                configuration.UpdateLocalConfiguration(configurationResponse);
//                return configuration;

//            }
//            catch (Exception ex)
//            {
//                Utilities.Logger.Log(ex);
//                return null;
//            }
//        }
//    }
//}

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
      try
      {
        Utilities.Logger.Log(Resources.Messages.ProcessStarted);
        System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();

        var configuration = Configuration.Load();

        //Start on separate thread to ensure process does not gets stack but rather thread is aborted
        Utilities.Threading.AsyncCallWithTimeout
        (
          () => Processor.Run(configuration),
          (int)(configuration.ThreadTimeToLiveSec * 1000)
        );
        Utilities.Logger.Log(Resources.Messages.ProcessSucceeded, Utilities.Logger.GetTimeElapsed(stopWatch));
      }
      catch
      {
        Utilities.Logger.Log(Resources.Messages.ProcessFailed);
        //No action, just quit
      }
    }

    public static void Run(Configuration configuration)
    {
      var response = RestClient.PostJson<JObject>(configuration.Uri, configuration);
      var responseDictionary = response.ToObject<IDictionary<string, object>>();
      string objectType = (string)responseDictionary[Constants.Configuration.ObjectType];

      while (string.Compare(objectType, Constants.ObjectType.None, StringComparison.InvariantCultureIgnoreCase) == 0)
      {
        switch(objectType)
        {
          case Constants.ObjectType.Command:
            throw new NotImplementedException();

          case Constants.ObjectType.File:
          case Constants.ObjectType.Folder:
            var info = Processor.LoadFileFolderInfo(configuration, responseDictionary);
            responseDictionary = Processor.PostFileFolderInfo(responseDictionary, info);
            break;
        }
      }

      configuration.Update(Configuration.Load(response));
      configuration.Save();
    }

    public static IDictionary<string, object> LoadFileFolderInfo(Configuration configuration, IDictionary<string, object> responseDictionary)
    {
      var info = Utilities.FileSystem.Load(configuration, (string)responseDictionary[Constants.Configuration.Path]);
      info.Add(Constants.Configuration.LocationKey, configuration.LocationKey);
      info.Add(Constants.Configuration.DerivedMachineHash, configuration.DerivedMachineHash);
      info.Add(Constants.Configuration.SequenceId, configuration.SequenceId);
      return info;
    }

    public static IDictionary<string, object> PostFileFolderInfo(
      IDictionary<string, object> config,
      IDictionary<string, object> fileFolderInfo
    )
    {
      var response = RestClient.PostJson<JObject>((string)config[Constants.Configuration.Uri], fileFolderInfo);
      var responseDictionary = response.ToObject<Dictionary<string, object>>();
      return responseDictionary;
    }
  }
}