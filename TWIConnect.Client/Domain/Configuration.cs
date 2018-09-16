using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Xml;
using System.Xml.Serialization;
using System.Management;
using Newtonsoft.Json;

using TWIConnect.Client.Utilities;


namespace TWIConnect.Client.Domain
{
  public enum ObjectType
  {
      Unknown = 0,
      None = 1,
      File = 2,
      Folder = 3,
      Command = 4
  }

  public class Configuration
  {
    private const string _configurationFile = ".\\Configuration.json";

    /// {
    //  "LocationKey": "New Site Install",
    //  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
    //  "ScheduledIntervalSec": "15",
    //  "FileSizeLimitMb": "501",
    //  "ImmutabilityIntervalSec": "2",
    //  "ThreadTimeToLiveSec": "5",
    //  "SequenceId": "1397608612",
    //  “ObjectType”:  "None",
    //  "Uri": "http://transactionalweb.com/ienterprise/pollrequest.htm",
    //  "IgnoreSizeLimit": "False",
    //  "IgnoreImmutabilityInterval": "False",
    //  "SendVersionAfterTimeStampUtc": "1970-01-01T01:01:40"
    // }
    public string LocationKey { get; set; }
    public int ScheduledIntervalSec { get; set; }
    public int FileSizeLimitMb { get; set; }
    public int ImmutabilityIntervalSec { get; set; }
    public int ThreadTimeToLiveSec { get; set; }
    public string SequenceId { get; set; }
    public ObjectType ObjectType { get; set;  }
    public string Uri { get; set; }
    public bool IgnoreSizeLimit { get; set; }
    public bool IgnoreImmutabilityInterval { get; set; }
    public DateTime SendVersionAfterTimeStampUtc { get; set; }

    private string derivedMachineHash = null;
    public string DerivedMachineHash
    {
      get { return this.derivedMachineHash ?? Configuration.GenerateDerivedMachineHash();}
      set { this.derivedMachineHash = value; }
    }

    /// <summary>
    /// Derive unique machine hash using cpu ids and mac ids
    /// </summary>
    /// <returns></returns>
    private static string GenerateDerivedMachineHash()
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

        return uniqueId;
      }

    /// <summary>
    /// Load local configuration
    /// </summary>
    /// <returns>Configuration</returns>
    public static Domain.Configuration Load()
    {
      try
      {
        System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
        string configurationFilePath = Configuration._configurationFile;
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartReadingLocalConfiguration, configurationFilePath);
        string json = Utilities.FileSystem.ReadTextFile(configurationFilePath);
        Configuration config = Configuration.Load(json);
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "Configuration.Load()", Logger.GetTimeElapsed(stopWatch));
        return config;
      }
      catch (Exception ex)
      {
        Utilities.Logger.Log(ex);
        throw ex;
      }
    }

    /// <summary>
    /// Load Configuration from Json string
    /// </summary>
    /// <returns></returns>
    public static Configuration Load(string json)
    {
      return JsonConvert.DeserializeObject<Configuration>(json);
    }

    /// <summary>
    /// Serialize to Json
    /// </summary>
    /// <returns>String</returns>
    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }

    ///// <summary>
    ///// Load remote configuration
    ///// In case of an exception return null
    ///// </summary>
    ///// <param name="uri"></param>
    ///// <returns></returns>
    //private void Load(Uri uri)
    //{
    //  try
    //  {
    //    System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew(); ;
    //    Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartReadingRemoteConfiguration, uri);



    //    Domain.ConfigurationRequest request = new ConfigurationRequest(localConfiguration);
    //    Domain.ConfigurationResponse configurationResponse = new RestClient(localConfiguration).GetRemoteConfiguration(request);
    //    Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "Configuration.TryLoadRemoteConfiguration", Logger.GetTimeElapsed(stopWatch));
    //    this._configurationResponse = configurationResponse;
    //  }
    //  catch (Exception ex)
    //  {
    //    Utilities.Logger.Log(ex);
    //    this._configurationResponse = null;
    //  }
    //}

    ///// <summary>
    ///// Update local configuration using response from the server
    ///// </summary>
    ///// <param name="configurationResponse"></param>
    //internal void UpdateLocalConfiguration(Domain.ConfigurationResponse configurationResponse)
    //{
    //    if (configurationResponse != null)
    //    {
    //        this.LocationKey = SelectValue<string>(configurationResponse.LocationKey, this.LocationKey);
    //        this.UrlPostFile = SelectValue<string>(configurationResponse.UrlPostFile, this.UrlPostFile);
    //        this.ScheduledIntervalSec = SelectValue<int?>(configurationResponse.ScheduledIntervalSec, (int?)this.ScheduledIntervalSec).Value;
    //        this.FileSizeLimitMb = SelectValue<int?>(configurationResponse.FileSizeLimitMb, (int?)this.FileSizeLimitMb).Value;
    //        this.ImmutabilityIntervalSec = SelectValue<int?>(configurationResponse.ImmutabilityIntervalSec, (int?)this.ImmutabilityIntervalSec).Value;
    //        this.MaxThreads = SelectValue<int?>(configurationResponse.MaxThreads, (int?)this.MaxThreads).Value;
    //        this.ThreadTimeToLiveSec = SelectValue<int?>(configurationResponse.ThreadTimeToLiveSec, (int?)this.ThreadTimeToLiveSec).Value;
    //        this.SequenceId = SelectValue<string>(configurationResponse.SequenceId, this.SequenceId);
    //        this.Files = configurationResponse.Files ?? new HashSet<FileSettings>();
    //        this.RunCommand = this.SelectValue<string>(configurationResponse.RunCommand, this.RunCommand);
    //        this.RunCommandArgs = this.SelectValue<string>(configurationResponse.RunCommandArgs, this.RunCommandArgs);
    //        this.Save(GetConfigurationFilePath());
    //    }
    //}

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
    /// Save configuration file
    /// </summary>
    internal void Save()
    {
      object fileLock = new object();
      try
      {
        System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartSavingLocalConfiguration, Configuration._configurationFile);
                
        //Shallow copy to remove File Settings from saved file
        Domain.Configuration configToSave = this.MemberwiseClone() as Domain.Configuration;

        lock (fileLock)
        {
            Utilities.FileSystem.WriteTextFile(_configurationFile, configToSave.Serialize());
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
