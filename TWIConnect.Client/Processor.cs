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

//        public static void Run(Domain.Configuration configuration)
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

//        private void RunCommand(Domain.Configuration configuration)
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

//        private void ProcessFiles(Domain.Configuration configuration)
//        {
//            try
//            {
//                //Single threaded processing to support continuous firing of one file at the time
//                Domain.Configuration config = configuration;

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

//        private Domain.Configuration ProcessFileAsync(Domain.Configuration configuration, Domain.FileSettings fileSettings)
//        {
//            try
//            {
//                Func<Domain.Configuration> operation = new Func<Domain.Configuration>(() => ProcessFile(configuration, fileSettings));
//                return Utilities.Threading.AsyncCallWithTimeout(operation, configuration.ThreadTimeToLiveSec * 1000);
//            }
//            catch (Exception ex)
//            {
//                Utilities.Logger.Log(ex);
//                return null;
//            }
//        }

//        private Domain.Configuration ProcessFile(Domain.Configuration configuration, Domain.FileSettings fileSettings)
//        {
//            try
//            {
//                var restClient = new RestClient(configuration);
//                Domain.ConfigurationResponse configurationResponse = null;

//                if (Utilities.FileSystem.IsDirectory(fileSettings.Name))
//                {
//                    var filesNames = Utilities.FileSystem.GetFilesList(fileSettings.Name);
//                    var files = filesNames.Select
//                                        (
//                                            fi =>
//                                                new Domain.File
//                                                {
//                                                    Content = null,
//                                                    Name = fi.FullName,
//                                                    SizeBytes = fi.Length,
//                                                    TimeStampUtc = fi.LastWriteTimeUtc,
//                                                }

//                                        );

//                    configurationResponse = restClient.SelectFile(new Domain.SelectFileRequest(configuration, fileSettings, files));
//                }
//                else
//                {

//                    Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartProcessFile, fileSettings.Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
//                    System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
//                    Processor._filesProcessed++;
//                    Domain.File file = Domain.File.Load(configuration, fileSettings);
//                    Domain.PostFileRequest postFileRequest = new Domain.PostFileRequest(configuration, file);
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
