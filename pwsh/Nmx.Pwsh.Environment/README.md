# Nmx.Pwsh.Environment

A PowerShell module for working with environment variables.  This module
can set environment variables for:

- Azure DevOps and Github actions
- Windows and Linux machine-level and user-level variables.
- Backwards compatible variables for PowerShell 5.1. e.g. IsWindows, IsCoreCLR
- NMX prefixed variables for normalized cross-platform use such as
  NMX_HOME, NMX_HOME_CONFIG, NMX_DOCUMENTS, NMX_HOSTNAME, NMX_USER, etc

## Build Variables

`Set-BuildEnvironmentVar` will set all environment variables as process variables
and if the function is executed as part of a task for Azure DevOps or GITHUB
it will use the task runner's special syntax to set the variable to be used
during the workflow/pipeline run.

## Global Compatibility Variables

PowerShell 5.1 does not have useful variables such as `$IsWindows`, `$IsLinux` and
all current versions of PowerShell do not have `$IsProcessElevated`.

Calling `Set-GlobalCompatibilityVar` will normalize these variables if they do
not already exist for the current PowerShell session.

## Environment Variables

`Set-EnvironmentVar` will set process environment variables for Win, Mac, and
Linux.

Setting machine and user variables on Mac are not currently supported.

For machine and user variables on linux, it will attempt to update files
if they exist:

- it will attempt to update /etc/environment for machine level variables
- it will attempt to update the a profile file in the following order; .zprofile,
  .bash_profile, and then .profile

## NMX Variables

Calling `Set-NmxEnvironmentVar` will set pre-determined in-process environment
variables that are cross platform.  The function is meant to be called as needed
or as part of powershell profile script.

Many of the NMX prefixed environment variables are meant to append an application
or powershell module name after the variable.

For example:

```powershell
$userTmpDir = "$env:NMX_HOME_TMP/appName"
$cfgDir = "$env:NMX_HOME_CONFIG/appName"
```

The `NMX_BIN_SHIMS` and `NMX_HOME_BIN_SHIMS` are really for windows where you can
create shim files that redirect to other .exe files to avoid path pollution.

### NMX Variables on Linux

The variables are mapped to the following

```powershell
$h = "$HOME".Replace("\", "/")
$Prefix = "NMX"
$od = if ($Env:OneDriveCommercial) { $Env:OneDriveCommercial } else { $Env:OneDrive }
if ($od) { $od = $od.replace("\", "/") }

"${Prefix}_HOME"           = $h
"${Prefix}_HOME_DATA"      = "$h/.local/share"
"${Prefix}_HOME_CACHE"     = "$h/.cache"
"${Prefix}_HOME_CONFIG"    = "$h/.config"
"${Prefix}_HOME_TMP"       = "$h/.local/tmp"
"${Prefix}_HOME_ROAMING"   = $null
"${Prefix}_HOME_BIN_SHIMS" = $null
"${Prefix}_DOCUMENTS"      = "$h/Documents"
"${Prefix}_DOWNLOADS"      = "$h/Downloads"
"${Prefix}_DESKTOP"        = "$h/Desktop"
"${Prefix}_ONEDRIVE"       = $od
"${Prefix}_VAR"            = "/var"
"${Prefix}_CONFIG"         = "/etc"
"${Prefix}_CACHE"          = "/var/cache"
"${Prefix}_TMP"            = "/var/tmp"
"${Prefix}_BIN_SHIMS"      = $null
"${Prefix}_HOSTNAME"       = $Env:HOSTNAME
"${Prefix}_USER"           = $Env:USER
"${Prefix}_OPT"            = "/opt"
```

### NMX Variables on Windows

The windows NMX variables map many of values to `ProgramData` or `LOCALAPPDATA`
where appending the application name to the value is important to working how
windows intends programs to store their data in an application sub folder.

```powershell
$desktop = [Environment]::GetEnvironmentVariable([Environment+SpecialFolder]::Desktop)
$documents = [Environment]::GetEnvironmentVariable([Environment+SpecialFolder]::MyDocuments)

if (![string]::IsNullOrWhiteSpace($desktop))
{
    $Paths["${Prefix}_DESKTOP"] = $desktop.Replace("\", "/")
}
if (![string]::IsNullOrWhiteSpace($documents))
{
    $Paths["{$Prefix}_DOCUMENTS"] = $documents.Replace("\", "/")
}

$var = $Env:ProgramData.Replace("\", "/")
$local = $Env:LOCALAPPDATA.Replace("\", "/")
$Paths["${Prefix}_VAR"] = $var
$Paths["${Prefix}_CACHE"] = $var
$Paths["${Prefix}_TMP"] = $var
$Paths["${Prefix}_CONFIG"] = $var
$Paths["${Prefix}_HOSTNAME"] = $Env:COMPUTERNAME
$Paths["${Prefix}_USER"] = $Env:USERNAME
$Paths["${Prefix}_HOME_DATA"] = $local
$Paths["${Prefix}_HOME_CACHE"] = $local
$Paths["${Prefix}_HOME_ROAMING"] = $ENV:APPDATA.Replace("\", "/")
$Paths["${Prefix}_HOME_TMP"] = $Env:TEMP.Replace("\", "/")
$Paths["${Prefix}_HOME_BIN_SHIMS"] = "$local/Apps/bin".Replace("\", "/")
$Paths["${Prefix}_OPT"] = $Env:SystemDrive + "/apps"
if ($Env:ChocolateyToolsLocation)
{
    $Paths["${Prefix}_BIN_SHIMS"] = $Env:ChocolateyToolsLocation.Replace("\", "/")
}
```

## License

Copyright 2020 Nerdy Mishka, Michael Herndon

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  [http://www.apache.org/licenses/LICENSE-2.0][license]

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

[license]: http://www.apache.org/licenses/LICENSE-2.0
