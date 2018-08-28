using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TWIConnect.Client.Domain
{
    [XmlRoot("file")]
    public class File
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public long SizeBytes { get; set; }
        public DateTime TimeStampUtc { get; set; }

        internal static File Load(Domain.Configuration configuration, Domain.FileSettings fileSettings)
        {
            System.IO.FileInfo fileInfo = Utilities.FileSystem.GetFileInfo(fileSettings.Name);

            //Check size limit if applicable
            int sizeMb = Utilities.FileSystem.GetFileSizeInMb(fileInfo);

            if (
                    (!fileSettings.IgnoreSizeLimit) &&
                    (sizeMb > configuration.FileSizeLimitMb))
            {
                throw new Exception(string.Format(Resources.Messages.FileSizeExceeded, fileSettings.Name, sizeMb, configuration.FileSizeLimitMb));
            }

            //Check immutability interval if applicable
            double immutabilitySec = DateTime.UtcNow.Subtract(fileInfo.LastWriteTimeUtc).TotalSeconds;
            if (
                    (!fileSettings.IgnoreImmutabilityInterval) &&
                    (configuration.ImmutabilityIntervalSec <= 0) &&
                    (immutabilitySec < configuration.ImmutabilityIntervalSec)
                )
            {
                throw new Exception(string.Format(Resources.Messages.FileImmutabilityIntervalNotReached, fileSettings.Name, immutabilitySec, configuration.ImmutabilityIntervalSec));
            }

            //Check SendVersionAfterTimeStampUtc has been reached
            if (
                    (fileSettings.SendVersionAfterTimeStampUtc.HasValue) &&
                    (fileSettings.SendVersionAfterTimeStampUtc != DateTime.MinValue) &&
                    (fileInfo.LastWriteTimeUtc < fileSettings.SendVersionAfterTimeStampUtc)
                )
            {
                throw new Exception(string.Format(Resources.Messages.SendVersionAfterTimeStampUtcNotReached, fileSettings.Name, fileInfo.LastWriteTimeUtc, fileSettings.SendVersionAfterTimeStampUtc));
            }

            //Load file content
            Domain.File file = new File()
                {
                    Content = Utilities.FileSystem.ReadFileAsBase64String(fileSettings.Name),
                    Name = fileSettings.Name,
                    SizeBytes = fileInfo.Length,
                    TimeStampUtc = fileInfo.LastAccessTimeUtc
                };

            return file;
        }
    }
}
