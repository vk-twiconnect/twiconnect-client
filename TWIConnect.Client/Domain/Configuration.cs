using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Xml;
using System.Xml.Serialization;
using System.Management;
using TWIConnect.Client.Utilities;

namespace TWIConnect.Client.Domain
{
    public class Configuration : Domain.BaseRequest
    {
        private Domain.ConfigurationResponse _configurationResponse;
        public int ScheduledIntervalSec { get; set; }
        public int FileSizeLimitMb { get; set; }
        public int ImmutabilityIntervalSec { get; set; }
        public int MaxThreads { get; set; }
        public int ThreadTimeToLiveSec { get; set; }
        public string SequenceId { get; set; }
        public HashSet<FileSettings> Files { get; set; }
        public string RunCommand { get; set; }
        public string RunCommandArgs { get; set; }

        [XmlIgnore]
        public string UrlPostFile { get; set; }

        [XmlElement("UrlPostFile")]
        public XmlCDataSection CDataUrlPostFile
        {
            get
            {
                return (new XmlDocument()).CreateCDataSection(this.UrlPostFile.ToBase64String());
            }
            set
            {
                this.UrlPostFile = value.Value.FromBase64String();
            }
        }

        private static string _installationId;
        public static string GetInstallationId()
        {
            if (string.IsNullOrWhiteSpace(Configuration._installationId))
            {
                var ids = new List<string>();

                var cpus = new ManagementClass("win32_processor").GetInstances();

                foreach (var cpu in cpus)
                {
                    ids.Add(cpu.Properties["ProcessorId"].Value.ToString());
                }

                foreach (var nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    string address = nic.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrWhiteSpace(address) && !ids.Contains(address))
                    {
                        ids.Add(nic.GetPhysicalAddress().ToString());
                    }
                }

                byte[] hash;
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    hash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Join(string.Empty, ids.ToArray())));
                }

                string uniqueId = string.Join(string.Empty, hash.Select(h => h.ToString("x2")));

                Configuration._installationId = uniqueId;
            }
            return Configuration._installationId;
        }

        /// <summary>
        /// Load local configuration
        /// Using local configuration load remote configuration
        /// Update and Save local configuration
        /// </summary>
        /// <returns></returns>
        public static Domain.Configuration Load()
        {
            try
            {
                string configurationFilePath = GetConfigurationFilePath();
                Domain.Configuration localConfiguration = LoadLocalConfiguration(configurationFilePath);

                Utilities.Threading.AsyncCallWithTimeout
                    (
                        () => localConfiguration.TryLoadRemoteConfiguration(localConfiguration), 
                        localConfiguration.ThreadTimeToLiveSec * 1000
                    );

                localConfiguration.UpdateLocalConfiguration(localConfiguration._configurationResponse);
                return localConfiguration;
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Configuration File Path from app.settings
        /// When not found in app.settings use CurrentDirectory\Configuration.Xml
        /// </summary>
        /// <returns></returns>
        private static string GetConfigurationFilePath()
        {
            string configurationFilePath = null;
            try
            {
                configurationFilePath = Utilities.AppSettings.Get<string>(Constants.Configuration.ConfigurationFilePath);
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
            }
            return (!string.IsNullOrEmpty(configurationFilePath)) ? configurationFilePath : "Configuration.xml";

        }

        /// <summary>
        /// Load local configuration settings
        /// In case of exception cannot continues
        /// </summary>
        /// <returns></returns>
        private static Domain.Configuration LoadLocalConfiguration(string configurationFilePath)
        {
            try
            {
                System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
                Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartReadingLocalConfiguration, configurationFilePath);
                Domain.Configuration configuration = Utilities.FileSystem.ReadTextFile(configurationFilePath).Deserialize<Domain.Configuration>();

                if (string.IsNullOrWhiteSpace(configuration.InstallationId))
                {
                    Configuration._installationId = configuration.InstallationId = Guid.NewGuid().ToString();
                    configuration.Save(configurationFilePath);
                }
                else
                {
                    Configuration._installationId = configuration.InstallationId;
                }
               
                Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "Configuration.LoadLocalConfiguration", Logger.GetTimeElapsed(stopWatch));
                return configuration;
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Try loading remote configuration
        /// In case of exception return null
        /// </summary>
        /// <param name="localConfiguration"></param>
        /// <returns></returns>
        private void TryLoadRemoteConfiguration(Domain.Configuration localConfiguration)
        {
            try
            {
                System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew(); ;
                Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartReadingRemoteConfiguration, localConfiguration.UrlPostFile);
                Domain.ConfigurationRequest request = new ConfigurationRequest(localConfiguration);
                Domain.ConfigurationResponse configurationResponse = new RestClient(localConfiguration).GetRemoteConfiguration(request);
                Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "Configuration.TryLoadRemoteConfiguration", Logger.GetTimeElapsed(stopWatch));
                this._configurationResponse = configurationResponse;
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
                this._configurationResponse = null;
            }
        }

        /// <summary>
        /// Update local configuration using response from the server
        /// </summary>
        /// <param name="configurationResponse"></param>
        internal void UpdateLocalConfiguration(Domain.ConfigurationResponse configurationResponse)
        {
            if (configurationResponse != null)
            {
                this.LocationKey = SelectValue<string>(configurationResponse.LocationKey, this.LocationKey);
                this.UrlPostFile = SelectValue<string>(configurationResponse.UrlPostFile, this.UrlPostFile);
                this.ScheduledIntervalSec = SelectValue<int?>(configurationResponse.ScheduledIntervalSec, (int?)this.ScheduledIntervalSec).Value;
                this.FileSizeLimitMb = SelectValue<int?>(configurationResponse.FileSizeLimitMb, (int?)this.FileSizeLimitMb).Value;
                this.ImmutabilityIntervalSec = SelectValue<int?>(configurationResponse.ImmutabilityIntervalSec, (int?)this.ImmutabilityIntervalSec).Value;
                this.MaxThreads = SelectValue<int?>(configurationResponse.MaxThreads, (int?)this.MaxThreads).Value;
                this.ThreadTimeToLiveSec = SelectValue<int?>(configurationResponse.ThreadTimeToLiveSec, (int?)this.ThreadTimeToLiveSec).Value;
                this.SequenceId = SelectValue<string>(configurationResponse.SequenceId, this.SequenceId);
                this.Files = configurationResponse.Files ?? new HashSet<FileSettings>();
                this.RunCommand = this.SelectValue<string>(configurationResponse.RunCommand, this.RunCommand);
                this.RunCommandArgs = this.SelectValue<string>(configurationResponse.RunCommandArgs, this.RunCommandArgs);
                this.Save(GetConfigurationFilePath());
            }
        }

        /// <summary>
        /// Update configuration settings using remote configuration response
        /// </summary>
        /// <typeparam name="T">Data Type for the property value</typeparam>
        /// <param name="sourceValue">Local Configuration Value</param>
        /// <param name="targetValue">Remote Configuration Value</param>
        /// <returns></returns>
        private T SelectValue<T>(T sourceValue, T targetValue)
        {
            if (typeof(T) == typeof(string))
            {
                return ((sourceValue != null) &&
                        (!string.IsNullOrEmpty(sourceValue.ToString()))) ?
                        sourceValue:
                        targetValue;
            }
            else
            {
                return (!sourceValue.Equals(default(T))) ?
                        sourceValue :
                        targetValue;
            }
        }

        /// <summary>
        /// Save configuration file - exclude list of files to upload.
        /// </summary>
        internal void Save(string configurationFilePath)
        {
            object fileLock = new object();
            try
            {
                System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
                Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartSavingLocalConfiguration, configurationFilePath);
                
                //Shallow copy to remove File Settings from saved file
                Domain.Configuration configToSave = this.MemberwiseClone() as Domain.Configuration;

                lock (fileLock)
                {
                    Utilities.FileSystem.WriteTextFile(configurationFilePath, configToSave.Serialize());
                }
                Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "Configuration.Save", Logger.GetTimeElapsed(stopWatch));
            }
            catch (Exception ex)
            {
                Utilities.Logger.Log(ex);
                throw ex;
            }
        }
    }
}
