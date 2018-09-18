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


namespace TWIConnect.Client
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
    
    public string Uri { get; set; }
    public bool IgnoreSizeLimit { get; set; }
    public bool IgnoreImmutabilityInterval { get; set; }
    public DateTime? SendVersionAfterTimeStampUtc { get; set; }

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
    public static Configuration Load()
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

    /// <summary>
    /// Update local configuration using response from the server
    /// </summary>
    /// <param name="newConfiguration"></param>
    public void Update(Configuration newConfiguration)
    {
      if (newConfiguration != null)
      {
        /*

        public ObjectType ObjectType { get; set;  }
        public string Uri { get; set; }
        public bool IgnoreSizeLimit { get; set; }
        public bool IgnoreImmutabilityInterval { get; set; }
        public DateTime SendVersionAfterTimeStampUtc { get; set; }
        */
        this.LocationKey = SelectValue<string>(newConfiguration.LocationKey, this.LocationKey);
        this.FileSizeLimitMb = SelectValue<int?>(newConfiguration.FileSizeLimitMb, (int?)this.FileSizeLimitMb).Value;
        this.ImmutabilityIntervalSec = SelectValue<int?>(newConfiguration.ImmutabilityIntervalSec, (int?)this.ImmutabilityIntervalSec).Value;
        this.ThreadTimeToLiveSec = SelectValue<int?>(newConfiguration.ThreadTimeToLiveSec, (int?)this.ThreadTimeToLiveSec).Value;
        this.SequenceId = SelectValue<string>(newConfiguration.SequenceId, this.SequenceId);
        this.Uri = SelectValue<string>(newConfiguration.Uri, this.Uri);
        this.ScheduledIntervalSec = SelectValue<int?>(newConfiguration.ScheduledIntervalSec, (int?)this.ScheduledIntervalSec).Value;
        this.ThreadTimeToLiveSec = SelectValue<int?>(newConfiguration.ThreadTimeToLiveSec, (int?)this.ThreadTimeToLiveSec).Value;
        this.SequenceId = SelectValue<string>(newConfiguration.SequenceId, this.SequenceId);
        this.Save();
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
    /// Save configuration file
    /// </summary>
    internal void Save()
    {
      object fileLock = new object();
      try
      {
        //Shallow copy to remove File Settings from saved file
        Configuration configToSave = this.MemberwiseClone() as Configuration;

        lock (fileLock)
        {
            Utilities.FileSystem.WriteTextFile(_configurationFile, configToSave.ToString());
        }
      }
      catch (Exception ex)
      {
        Utilities.Logger.Log(ex);
        throw ex;
      }
    }
  }
}
