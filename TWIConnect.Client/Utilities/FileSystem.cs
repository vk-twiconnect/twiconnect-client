using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TWIConnect.Client.Utilities;

namespace TWIConnect.Client.Utilities
{
    class FileSystem
    {
        private static string GetFullFilePath(string filePath)
        {
            const string backSlash = "\\";
            return (filePath.Contains(backSlash)) ? filePath : AppDomain.CurrentDomain.BaseDirectory + filePath;

        }

        internal static int GetFileSizeInMb(System.IO.FileInfo fileInfo)
        {
            return (int)(fileInfo.Length / 1048576);
        }

        internal static string ReadTextFile(string filePath)
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

        internal static void WriteTextFile(string filePath, string fileContent)
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

        internal static string ReadFileAsBase64String(string filePath)
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

        internal static IEnumerable<System.IO.FileInfo> GetFilesList(string path, string searchPattern = "*.*")
        {
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartGettingIOFileList, path, searchPattern);

            var files = System.IO.Directory.GetFiles(path, searchPattern);
            IEnumerable<System.IO.FileInfo> fileInfos;

            if (files.Any())
            {
                fileInfos = files.Select(fn => FileSystem.GetFileInfo(fn));
            }
            else
            {
                fileInfos = new List<System.IO.FileInfo>();
            }

            Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "FileSystem.GetFilesList", Logger.GetTimeElapsed(stopWatch));
            return fileInfos;
        }

        internal static System.IO.FileInfo GetFileInfo(string filePath)
        {
            System.Diagnostics.Stopwatch stopWatch = System.Diagnostics.Stopwatch.StartNew();
            Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.StartGettingIOFileInfo, filePath);
            filePath = GetFullFilePath(filePath); 
            Utilities.Logger.Log(NLog.LogLevel.Trace, Resources.Messages.EndOfExecution, "FileSystem.GetFileInfo", Logger.GetTimeElapsed(stopWatch));
            return new System.IO.FileInfo(filePath);
        }

        internal static bool IsDirectory(string path)
        {
            return System.IO.Directory.Exists(path);
        }
    }
}
