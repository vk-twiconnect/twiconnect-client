# Overview

* TWI Client is an unattended bot running in the background as a Windows Service to upload files, collect folder meta-data, and to execute shell commands on the client
* Server side is fully in control of the client behaviour, updating client's configuration and instructing what file/folder to upload or what command to execute 
* Client provides no user interface for an end-user to configure
* Server responds with one of the sample responses, no more than one file/folder/command is processed by the client at a time

## Flow Diagram

![](./media/Flow.jpg)

## Configuration.json

* Each client location will use a unique configuration file
* Setup process will distribute unique confiuration file

## Client Requests

### Http-Verb

* All requests will use POST verb to the uri from the configuration/server response json


### Current Configuration

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ScheduledIntervalSec": "15",
  "ThreadTimeToLiveSec": "5",
  "SequenceId": "1397608612",
  "Uri": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "ExitCode": 0,
  "ErrorMessage": ""
}
```

### Post a File

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ObjectType": "File",
  "FileContent": "long-base64encoded-string",
  "FileSize": "1234578",
  "Path": "C:\\Temp\\Configuration.json",
  "Modified": "2013-01-01-T00:00:00",
  "ExitCode": 1,
  "ErrorMessage": ""
}
```

### Post Folder Meta-Data

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ObjectType": "Folder",
  "FolderSize": "1234578",
  "SubFoldersCount": "35",
  "FilesCount": "3",
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

### Execute Command

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ObjectType": "Command",
  "CommandLine": "powershell.exe",
  "CommandArguments": "dir c:\\",
  "CommandExitCode": 1,
  "CommandOutput": "long-text"
}
```

**Note:** do not use `cmd.exe`, it tends to hang on completion.

## Server Responses

### Update Configuration Only

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ScheduledIntervalSec": "15",
  "ThreadTimeToLiveSec": "5",
  "SequenceId": "1397608612",
  "ObjectType":  "None",
  "Uri": "http://transactionalweb.com/ienterprise/pollrequest.htm"
}
```

### Upload File

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ScheduledIntervalSec": "15",
  "ObjectType":  "File",
  "SequenceId": "1397608612",
  "Uri": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "ThreadTimeToLiveSec": "5",
  "ImmutabilityIntervalSec": "2",
  "FileSizeLimitMb": "501",
  "IgnoreSizeLimit": "False",
  "IgnoreImmutabilityInterval": "False",
  "SendVersionAfterTimeStampUtc": "1970-01-01T01:01:40",
  "Path": "C:\\Temp\\Configuration.json"
}
```

### Upload Folder Meta-data

```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "ScheduledIntervalSec": "15",
  "ObjectType":  "Folder",
  "SequenceId": "1397608612",
  "Uri": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "ThreadTimeToLiveSec": "5",
  "Path": "C:\\Temp"
}
```
* ThreadTimeToLive settings will limit time collecting folder meta-data

### Execute Command
```
{
  "LocationKey": "New Site Install",
  "DerivedMachineHash": "a3d61dcd929f262dc652ffce4ef61231",
  "SequenceId": "1397608612",
  "ScheduledIntervalSec": "15",
  "ObjectType":  "Command",
  "Uri": "http://transactionalweb.com/ienterprise/pollrequest.htm",
  "ThreadTimeToLiveSec": "5",
  "CommandLine": "powershell.exe"
  "CommandArguments": "dir c:\\"
}
```

## Development Setup

* Windows Server 2016 Base, may work on other modern Windows OS
* Install [Visual Studio 2017 Community Edition](https://visualstudio.microsoft.com/downloads/)
* Install [Wix Toolset](http://wixtoolset.org/releases/)
* Insttall Wix Toolset VS2017 Extension from Tools->Extensions and Updates
