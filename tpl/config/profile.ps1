Write-Debug "profile loaded"
$Projects = if ($Env:Projects) { $Env:Projects } else { $HOME.Replace("\", "/") + "/projects" }

# code $Profile should let you edit your profile
[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("", "PSAvoidAssignmentToAutomaticVariable")]
$Profile = "$Env:OneDrive\profile.ps1"

# create the project directory, ideally in the home directory as git repos
# tend to be set to a specific user locally.
if (!(Test-Path $Projects))
{
    New-Item $Projects -ItemType Directory | Write-Debug
}

# create a ps drive of p to easily switch between projects
# e.g.  cd p:/myproject
if (Test-Path $Projects)
{
    New-PSDrive -Name "P" -PSProvider FileSystem -Root "$Projects"
}

Set-Location P:/

# Setup Powerline for the shell
$mod = Get-Module "oh-my-posh" -ListAvailable
if ($mod)
{
    Import-Module posh-git
    Import-Module oh-my-posh
    Set-Theme Paradox
    # should allow emoji in cmder
    chcp 65001 | Out-Null
}

if ((Get-Module "Dockercompletion" -ListAvailable -EA SilentlyContinue))
{
    Import-Module "DockerCompletion"
}

if ((Get-Module "PSKubectlCompletion" -ListAvailable -EA SilentlyContinue))
{
    Import-Module PSKubectlCompletion
}


# add PowerShell Core backwards compatibility for PowerShell 5.1 and below
if ($null -eq $IsWindows)
{
    $Global:IsWindows = $false
    $Global:IsLinux = $false
    $Global:IsMacOS = $false
    $Global:IsCoreCLR = $host.Version.Major -ge 6
    $platform = [Environment]::OsVersion.Platform.ToString()
    if ($platform.StartsWith("Win")) { $Global:IsWindows = $true }

    Switch ($platform)
    {
        "Unix" { $Global:IsLinux = $true }
        "MacOS" { $Global:IsMacOS = $true }
    }
}

Write-Debug ("IsWindows".PadLeft(40) + "$IsWindows")
Write-Debug ("IsLinux".PadLeft(40) + "$IsLinux")
Write-Debug ("IsMacOS".PadLeft(40) + "$IsMacOS")
Write-Debug ("IsCoreCLR".PadLeft(40) + "$IsCoreCLR")
Write-Debug ("Profile".PadLeft(40) + "$Profile")

$h = $HOME.Replace("\", "/")
$od = if ($Env:OneDriveCommercial) { $Env:OneDriveCommercial } else { $Env:OneDrive }
if ($od) { $od = $od.replace("\", "/") }
$Paths = @{
    "NMX_HOME"           = $h
    "NMX_HOME_DATA"      = "$h/.local/share"
    "NMX_HOME_CACHE"     = "$h/.cache"
    "NMX_HOME_CONFIG"    = "$h/.config"
    "NMX_HOME_TMP"       = "$h/.local/tmp"
    "NMX_HOME_ROAMING"   = $null
    "NMX_HOME_BIN_SHIMS" = $null
    "NMX_DOCUMENTS"      = "$h/Documents"
    "NMX_DOWNLOADS"      = "$h/Downloads"
    "NMX_ONEDRIVE"       = $od
    "NMX_DESKTOP"        = "$h/Desktop"
    "NMX_VAR"            = "/var"
    "NMX_CONFIG"         = "/etc"
    "NMX_CACHE"          = "/var/cache"
    "NNX_TMP"            = "/var/tmp"
    "NMX_BIN_SHIMS"      = $null
    "NMX_HOSTNAME"       = $Env:HOSTNAME
    "NMX_USER"           = $Env:USER
}

if (Get-Module "psake" -EA SilentlyContinue)
{
    Set-Alias "psake" "invoke-psake"
}


if (Get-Module "pester" -EA SilentlyContinue)
{
    Set-Alias "paster" "invoke-pester"
}

# TODO: set XDG_ vars for linux. These are generally the same as the above.

if ($IsWindows)
{
    $desktop = [Environment]::GetEnvironmentVariable([Environment+SpecialFolder]::Desktop)
    $documents = [Environment]::GetEnvironmentVariable([Environment+SpecialFolder]::MyDocuments)

    if (![string]::IsNullOrWhiteSpace($desktop))
    {
        $Paths.NMX_DESKTOP = $desktop.Replace("\", "/")
    }
    if (![string]::IsNullOrWhiteSpace($documents))
    {
        $Paths.NMX_DOCUMENTS = $documents.Replace("\", "/")
    }

    $var = $Env:ProgramData.Replace("\", "/")
    $local = $Env:LOCALAPPDATA.Replace("\", "/")
    $Paths.NMX_VAR = $var
    $Paths.NMX_CACHE = $var
    $Paths.NMX_TMP = $var
    $Paths.NMX_CONFIG = $var
    $Paths.NMX_HOSTNAME = $Env:COMPUTERNAME
    $Paths.NMX_USER = $Env:USERNAME
    $Paths.NMX_HOME_DATA = $local
    $Paths.NMX_HOME_CACHE = $local
    $Paths.NMX_HOME_ROAMING = $ENV:APPDATA.Replace("\", "/")
    $Paths.NMX_HOME_TMP = $Env:TEMP.Replace("\", "/")
    $Paths.NMX_HOME_BIN_SHIMS = "$local/Apps/bin".Replace("\", "/")
    if ($Env:ChocolateyToolsLocation)
    {
        $Paths.NMX_BIN_SHIMS = $Env:ChocolateyToolsLocation.Replace("\", "/")
    }
}

foreach ($key in $Paths.Keys)
{
    # only set the environment variables if they do not already exist
    $var = Get-Item -Path Env:$key -EA SilentlyContinue
    if ($null -eq $var)
    {
        Set-Item -Path "Env:$key" -Value ($Paths[$key])
    }
}

# Add shim locations to path. Google chocolatey bin shims.
if ($Env:NMX_HOME_BIN_SHIMS)
{
    $path = $env:Path
    $value = $Env:NMX_HOME_BIN_SHIMS.Replace("/", "\").Trim("\")
    if (!$path.ToLower().Contains($value.ToLower()))
    {
        $env:Path += ";$value"
    }
}

if ($Env:NMX_BIN_SHIMS)
{
    $path = $env:Path
    $value = $Env:NMX_BIN_SHIMS.Replace("/", "\").Trim("\")
    if (!$path.ToLower().Contains($value.ToLower()))
    {
        $env:Path += ";$value"
    }
}

function Get-NmxEnvVar()
{
    $x = Get-Item Env:
    foreach ($item in $x)
    {
        if ($item.Name.StartsWith("NMX_"))
        {
            Write-Output $item
        }
    }
}

Remove-Variable -Name "Paths"

# add tab completion for dotnet.exe
if ($null -ne (Get-Command dotnet -EA SilentlyContinue))
{
    Register-ArgumentCompleter -Native -CommandName dotnet -ScriptBlock {
        param($commandName, $wordToComplete, $cursorPosition)
        dotnet complete --position $cursorPosition "$wordToComplete" | ForEach-Object {
            [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
        }
    }
}

# add tab completion for choco.exe
if (!$Env:ChocolateyInstall)
{
    $cmd = (Get-Command choco -EA SilentlyContinue)
    if ($null -eq $cmd)
    {
        Write-Debug "chocolatey not installed"
    }
    else
    {
        # path/to/bin/choco.exe
        $Env:ChocolateyInstall = (Resolve-Path ($cmd.path | Split-Path -Parent) + "/../").Path
    }
}
if ($Env:ChocolateyInstall)
{
    if (Test-Path "$Env:ChocolateyInstall/helpers/chocolateyProfile.psm1")
    {
        # enables tab completion
        Import-Module "$Env:ChocolateyInstall/helpers/chocolateyProfile.psm1"
    }
}