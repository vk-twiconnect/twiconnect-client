using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TWIConnect.Client.Utilities;

namespace TWIConnect.Client.Utilities
{
  public class FileSystem
  {

    public static IDictionary<string, object> Load(Configuration configuration, string path)
    {
      bool isFolder = FileSystem.IsDirectory(path);
      var info = isFolder? LoadFolderMetaData(configuration, path) : LoadFile(configuration, path);
      info.Add(Constants.Configuration.ObjectType, isFolder? Constants.ObjectType.Folder: Constants.ObjectType.File);

      #region Move out!
        info.Add(Constants.Configuration.LocationKey, configuration.LocationKey);
        info.Add(Constants.Configuration.DerivedMachineHash, configuration.DerivedMachineHash);
        info.Add(Constants.Configuration.SequenceId, configuration.SequenceId);
      #endregion

      return info;
    }

    public static IDictionary<string, object> LoadFolderMetaData(Configuration configuration, string path)
    {
      var files = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly).Select(
                        file => new Dictionary<string, object>() {
                          { Constants.Configuration.Path, file }
                        }
                      );
      var subFolders = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly).Select(folder => 
                          new Dictionary<string, object>(){ { Constants.Configuration.Path, folder } }
                        );

      var lastModified = new DirectoryInfo(path).GetDirectories("*", SearchOption.AllDirectories)
                              .OrderByDescending(d => d.LastWriteTimeUtc)
                              .Select(d => d.LastWriteTimeUtc)
                              .FirstOrDefault();

      var fileSizes = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                        //.AsParallel()
                        .Select(file => {
                          try
                          {
                            var size = FileSystem.GetFileInfo(file).Length;
                            return size;
                          }
                          catch (Exception ex)
                          {
                            Utilities.Logger.Log(ex);
                            return 0.0;
                          }
                        });

      return new Dictionary<string, object>()
      {
        { Constants.Configuration.Path,  path },
        { Constants.Configuration.FolderSize, fileSizes.Sum() },
        { Constants.Configuration.Modified, lastModified },
        { Constants.Configuration.SubFoldersCount, subFolders.Count() },
        { Constants.Configuration.SubFolders, subFolders },
        { Constants.Configuration.FilesCount, files.Count() },
        { Constants.Configuration.Files, files }
      };
    }

    public static IDictionary<string, object> LoadFile(Configuration configuration, string path)
    {
      System.IO.FileInfo fileInfo = Utilities.FileSystem.GetFileInfo(path);

      //Check size limit if applicable
      var sizeMb = Utilities.FileSystem.GetFileSizeInMb(fileInfo);

      if (
          (!configuration.IgnoreSizeLimit) &&
          (sizeMb > configuration.FileSizeLimitMb)
         )
      {
        throw new Exception(string.Format(Resources.Messages.FileSizeExceeded, path, sizeMb, configuration.FileSizeLimitMb));
      }

      //Check immutability interval if applicable
      double immutabilitySec = DateTime.UtcNow.Subtract(fileInfo.LastWriteTimeUtc).TotalSeconds;
      if (
            (!configuration.IgnoreImmutabilityInterval) &&
            (configuration.ImmutabilityIntervalSec <= 0) &&
            (immutabilitySec < configuration.ImmutabilityIntervalSec)
          )
      {
        throw new Exception(
          string.Format(
            Resources.Messages.FileImmutabilityIntervalNotReached,
            path,
            immutabilitySec,
            configuration.ImmutabilityIntervalSec
          )
        );
      }

      //Check SendVersionAfterTimeStampUtc has been reached
      if (
          (configuration.SendVersionAfterTimeStampUtc.HasValue) &&
          (configuration.SendVersionAfterTimeStampUtc != DateTime.MinValue) &&
          (fileInfo.LastWriteTimeUtc < configuration.SendVersionAfterTimeStampUtc)
         )
      {
        throw new Exception(
          string.Format(
            Resources.Messages.SendVersionAfterTimeStampUtcNotReached,
            path,
            fileInfo.LastWriteTimeUtc,
            configuration.SendVersionAfterTimeStampUtc
          )
        );
      }

      //Load file content
      var file = new Dictionary<string, object>
      {
        { Constants.Configuration.FileContent, Utilities.FileSystem.ReadFileAsBase64String(path) },
        { Constants.Configuration.Path, path },
        { Constants.Configuration.FileSize, fileInfo.Length },
        { Constants.Configuration.Modified, fileInfo.LastAccessTimeUtc }
      };

      return file;
    }

    public static string GetFullFilePath(string filePath)
    {
      const string backSlash = "\\";
      return (filePath.Contains(backSlash)) ? filePath : AppDomain.CurrentDomain.BaseDirectory + filePath;
    }

    public static double GetFileSizeInMb(System.IO.FileInfo fileInfo)
    {
      return fileInfo.Length / 1048576;
    }

    public static string ReadTextFile(string filePath)
    {
      System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
      Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartReadingIOFile, filePath);
            
      filePath = GetFullFilePath(filePath);
      using (System.IO.StreamReader stream = new StreamReader(filePath))
      {
        string fileContent = stream.ReadToEnd();
        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "FileSystem.ReadTextFile", Logger.GetTimeElapsed(stopWatch));
        return fileContent;
      }
    }

    public static void WriteTextFile(string filePath, string fileContent)
    {
      System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
      Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartWritingIOFile, filePath);

      filePath = GetFullFilePath(filePath);
      using (System.IO.StreamWriter stream = new StreamWriter(filePath, false))
      {
        stream.WriteLine(fileContent);
        stream.Close();
      }
      Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "FileSystem.WriteTextFile", Logger.GetTimeElapsed(stopWatch));
    }

    public static string ReadFileAsBase64String(string filePath)
    {
      System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
      Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartReadingIOFile, filePath);

      filePath = GetFullFilePath(filePath);
      using (System.IO.FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        int contentLength = (int)stream.Length;
        byte[] buffer = new byte[contentLength];
        stream.Read(buffer, 0, contentLength);
        string fileContent = buffer.ToBase64String();

        Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "FileSystem.ReadFileAsBase64String", Logger.GetTimeElapsed(stopWatch));
        return fileContent;
      }
    }

    public static System.IO.FileInfo GetFileInfo(string filePath)
    {
      System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
      filePath = GetFullFilePath(filePath); 
      return new System.IO.FileInfo(filePath);
    }

    public static bool IsDirectory(string path)
    {
      return System.IO.Directory.Exists(path);
    }
  }
}
