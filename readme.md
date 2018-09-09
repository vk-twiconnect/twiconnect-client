# Overview

## Flow Diagram

![](./media/Flow.jpg)

## Client Requests

### Current Configuration (Request #1)

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ScheduledIntervalSec": "15",
  "FileSizeLimitMb": "501",
  "ImmutabilityIntervalSec": "2",
  "ThreadTimeToLiveSec": "5",
  "SequenceId": "1397608612",
  “ObjectType”:  “none”,
  "UrlPostFile": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "IgnoreSizeLimit": "False",
  "IgnoreImmutabilityInterval": "False",
  "SendVersionAfterTimeStampUtc": "1970-01-01T01:01:40"
}
```

### Post a file (Request #2)

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ObjectType": "File",
  "FileContent": "long-base64encoded-string",
  "FileSize": "1234578",
  "Path": "C:\\Temp\\Configuration.json",
  "Modified": "2013-01-01-T00:00:00"
}
```

### Post folder meta-data (Request #3)

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ObjectType": "Folder",
  "FolderSize": "1234578",
  "SubFoldersCount": "35",
  "FileCount": "3",
  "SubFolders": [
    {"Path": "C:\\Temp\\_1"},
    {"Path": "C:\\Temp\\_2}"
  ],
  "Files": [
    { "Path": "C:\\Temp\\_1\\aa.txt" },
    { "Path": "C:\\Temp\\_2\\bb.tmp" }
  ],
  "Path": "C:\\Temp",
  "Modified": "2013-01-01-T00:00:00"
}
```

### Execute Command Results (Request #4)

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ObjectType": "Command",
  "CommandLine": "dir C:\\"
  "CommandExitCode": 1,
  "CommandOutput": "long-text"
}
```

## Server Responsea

### Update configuration only

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ScheduledIntervalSec": "15",
  "FileSizeLimitMb": "501",
  "ImmutabilityIntervalSec": "2",
  "ThreadTimeToLiveSec": "5",
  "SequenceId": "1397608612",
  "ObjectType":  "None",
  "UrlPostFile": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "IgnoreSizeLimit": "False",
  "IgnoreImmutabilityInterval": "False",
  "SendVersionAfterTimeStampUtc": "1970-01-01T01:01:40"
}
```

### Upload file

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ObjectType":  "File",
  "SequenceId": "1397608612",
  "UrlPostFile": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "IgnoreSizeLimit": "False",
  "IgnoreImmutabilityInterval": "False",
  "SendVersionAfterTimeStampUtc": "1970-01-01T01:01:40",
  "Path": "C:\\Temp\\Configuration.json"
}
```

### Upload folder meta-data

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ObjectType":  "Folder",
  "SequenceId": "1397608612",
  "UrlPostFile": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "Path": "C:\\Temp"
}
```

### Run Command
```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ObjectType":  "Command",
  "UrlPostFile": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "CommandLine": "dir C:"
}
```
* ThreadTimeToLive settings will limite command line execution


## Developemtn Setup

* Windows Server 2016 Base, may work on other modern Windows OS
* Install [Visual Studio 2017 Community Edition](https://visualstudio.microsoft.com/downloads/)
* Install [Wix Toolset](http://wixtoolset.org/releases/)
* Insttall Wix Toolset VS2017 Extension from Tools->Extensions and Updates