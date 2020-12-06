function Set-NmxEnviromentVar()
{
    <#
    .SYNOPSIS
        Sets cross platform nmx prefixed variables which are useful for 
        applications or shell profiles.
    .DESCRIPTION
        Sets cross platform nmx prefixed variables which are useful for 
        applications or shell profiles.

        For variables to represent paths, these are not guaranteed to exist
        such as NMX_HOME_DATA or NMX_DOCUMENTS.
    .EXAMPLE
        PS C:\> <example usage>
        Explanation of what the example does
    .INPUTS
        Inputs (if any)
    .OUTPUTS
        Output (if any)
    .NOTES
        General notes
    #>
    param(
        [Parameter()]
        [String] 
        $Prefix = "NMX"
    )

    $h = $HOME.Replace("\", "/")
    $od = if ($Env:OneDriveCommercial) { $Env:OneDriveCommercial } else { $Env:OneDrive }
    if ($od) { $od = $od.replace("\", "/") }
    $Paths = @{
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
    }

    # TODO: set XDG_ vars for linux. These are generally the same as the above.

    if ($IsWindows)
    {
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

    if ($IsWindows)
    {
        # Add shim locations to path. Google chocolatey bin shims.
        $homeShims = $Paths["${Prefix}_HOME_BIN_SHIMS"]
        if ($homeShims)
        {
            $path = $env:Path
            $value = $homeShims.Replace("/", "\").Trim("\")
            if (!$path.ToLower().Contains($value.ToLower()))
            {
                $env:Path += ";$value"
            }
        }

        $globalShims = $Paths["${Prefix}_BIN_SHIMS"]
        if ($globalShims)
        {
            $path = $env:Path
            $value = $globalShims.Replace("/", "\").Trim("\")
            if (!$path.ToLower().Contains($value.ToLower()))
            {
                $env:Path += ";$value"
            }
        }
    }
}