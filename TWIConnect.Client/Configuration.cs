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
using Newtonsoft.Json.Linq;

namespace TWIConnect.Client
{
  public class Configuration
  {
    public string LocationKey { get; set; }
    public int ScheduledIntervalSec { get; set; }
    public int ThreadTimeToLiveSec { get; set; }
    public string SequenceId { get; set; }
    public string Uri { get; set; }

    private string derivedMachineHash = null;
    public string DerivedMachineHash
    {
      get { return this.derivedMachineHash ?? Configuration.GenerateDerivedMachineHash();}
      set { this.derivedMachineHash = value; }
    }

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

    public static Configuration Load()
    {
      try
      {
        System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
        string configurationFilePath = FileSystem.GetCurrentFolderName() + "\\" + Constants.FileNames.ConfigurationFileName;
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartReadingLocalConfiguration, configurationFilePath);
        string json = Utilities.FileSystem.ReadTextFile(configurationFilePath);
        Configuration config = Configuration.FromJson(json);
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "Configuration.Load()", Logger.GetTimeElapsed(stopWatch));
        return config;
      }
      catch (Exception ex)
      {
        Utilities.Logger.Log(ex);
        throw ex;
      }
    }

    public static Configuration FromFile(string path)
    {
      var json = Utilities.FileSystem.ReadTextFile(path);
      var config = Configuration.FromJson(json);
      return config;
    }

    public static Configuration FromJson(string json)
    {
      return JsonConvert.DeserializeObject<Configuration>(json);
    }

    public static Configuration Load(JToken jtoken)
    {
      return jtoken.ToObject<Configuration>();
    }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }

    public void Update(Configuration newConfiguration)
    {
      if (newConfiguration != null)
      {
        this.LocationKey = SelectValue<string>(newConfiguration.LocationKey, this.LocationKey);
        this.ThreadTimeToLiveSec = SelectValue<int?>(newConfiguration.ThreadTimeToLiveSec, (int?)this.ThreadTimeToLiveSec).Value;
        this.SequenceId = SelectValue<string>(newConfiguration.SequenceId, this.SequenceId);
        this.Uri = SelectValue<string>(newConfiguration.Uri, this.Uri);
        this.ScheduledIntervalSec = SelectValue<int?>(newConfiguration.ScheduledIntervalSec, (int?)this.ScheduledIntervalSec).Value;
        this.ThreadTimeToLiveSec = SelectValue<int?>(newConfiguration.ThreadTimeToLiveSec, (int?)this.ThreadTimeToLiveSec).Value;
        this.SequenceId = SelectValue<string>(newConfiguration.SequenceId, this.SequenceId);
        this.Save();
      }
    }

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

    internal void Save()
    {
      object fileLock = new object();
      try
      {
        //Shallow copy to remove File Settings from saved file
        Configuration configToSave = this.MemberwiseClone() as Configuration;

        lock (fileLock)
        {
            Utilities.FileSystem.WriteTextFile(Constants.FileNames.ConfigurationFileName, configToSave.ToString());
        }
      }
      catch (Exception ex)
      {
        Utilities.Logger.Log(ex);
        throw ex;
      }
    }

    public static Configuration FromJObject(JObject jObject)
    {
      return jObject.ToObject<Configuration>();
    }
  }
}
