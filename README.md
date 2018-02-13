# Tfs-Migrate

[![Build status](https://ci.appveyor.com/api/projects/status/97r3hxl15qufel9u?svg=true)](https://ci.appveyor.com/project/alastairgould/tfs-migrate)

## How to Install

### Prerequisites

* Make sure GIT is installed and its on your enviroment path
* Make sure powershell 5 or above is installed

### Powershell Gallery

## Powershell Cmdlets

### Get-TfsRepository

### Convert-ToGit

### Import-VstsRepository

### Import-WorkItemAssociations

## Examples

### Simple Tfs to Git Conversion

```powershell
Get-TfsRepository -ProjectCollection [projectcollectionurl] -Path [Path to folder in tfs repo] 
| Convert-ToGit -LocalRepositoryPath [local path to where the new git repo will be stored] 

```

### Fix Split Histories

```powershell
Get-TfsRepository -ProjectCollection [projectcollectionurl] -Path [Path to folder in tfs repo] 
| Get-TfsRepository -ProjectCollection [projectcollectionurl] -Path [Path to folder in tfs repo] 
| Convert-ToGit -LocalRepositoryPath [local path to where the new git repo will be stored] 
```

### Import into VSTS GIT Repository

### Import into VSTS GIT Repository and Work Item Reassociation
