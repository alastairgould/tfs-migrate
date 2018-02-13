# Tfs-Migrate

[![Build status](https://ci.appveyor.com/api/projects/status/97r3hxl15qufel9u?svg=true)](https://ci.appveyor.com/project/alastairgould/tfs-migrate)

Tfs Migrate is a set tools which helps you migrate from Tfs To Git. Although it can be used to create just plain GIT repositories, 
it is especially suited for migrating to VSTS hosted GIT. When used with VSTS it will let you reassociate your work items to the new Git commits.

Unlike the the built in VSTS Git migration tool, this will attempt transfer the entire history of your TFS repository and branches.

## How to Install

### Prerequisites

* Make sure GIT is installed and its on your environment path
* Make sure powershell 5 or above is installed

### Install From Powershell Gallery

## Powershell Cmdlets

### Get-TfsRepository

### Convert-ToGit

### Set-MasterBranch

### Import-ToVstsRepository

### Import-WorkItemAssociations

## Examples

### Simple Tfs to Git conversion

```powershell
Get-TfsRepository -ProjectCollection [projectcollectionurl] -Path [Path to folder in tfs repo] 
| Convert-ToGit -LocalRepositoryPath [local path to where the new git repo will be stored] 

```

### Set master branch

### Fix split histories

```powershell
Get-TfsRepository -ProjectCollection [projectcollectionurl] -Path [Path to folder in tfs repo] 
| Get-TfsRepository -ProjectCollection [projectcollectionurl] -Path [Path to folder in tfs repo] 
| Convert-ToGit -LocalRepositoryPath [local path to where the new git repo will be stored] 
```

### Import into VSTS GIT repository

### Import into VSTS GIT repository and work item reassociation
