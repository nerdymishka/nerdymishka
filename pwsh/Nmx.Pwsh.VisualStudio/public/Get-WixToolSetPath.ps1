$wixInstalls = $null 

function Get-WixToolSetPath()
{
    [CmdletBinding()]
    param (
        [Parameter()]
        [Switch]
        $All
    )

    if (!$IsWindows)
    {
        # revisit for 4.0 release
        return $null
    }

    if ($wixInstalls)
    {
        if ($All)
        {
            $wixInstalls.Path 
        }

        $wixInstalls | Sort-Object -Property Version | Select -First 1
        if ($result)
        {
            return $result.Path 
        }
    }

    $UninstallKeys = @('HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall',
        'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall',
        'HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall'
    )

    $set = @()
    $keys = Get-ChildItem $UninstallKeys
    $versions = @()
    foreach ($key in $keys)
    {
        $displayName = $key.GetValue("DisplayName")
        if (!$displayName)
        {
            continue;
        }

        if ($displayName -match "^Wix Toolset v*")
        {
            $v = $key.GetValue("DisplayVersion")
            if ($versions.Contains($v))
            {
                continue;
            }
          
            $versions += [Version]$v          
        }
    }

    foreach ($v in $versions)
    {
        $location = "$env:ProgramFiles\WiX Toolset v$($v.Major).$($v.Minor)"
        if (Test-Path $location)
        {
            $set += [PsCustomObject]@{
                version = $v 
                path    = $location
            }
            continue;
        }

        $location = "${env:ProgramFiles(x86)}\WiX Toolset v$($v.Major).$($v.Minor)"
        if (Test-Path $location)
        {
            $set += [PsCustomObject]@{
                version = $v 
                path    = $location
            }
            continue;
        }
    }

    $wixInstalls = $set;
    if ($wixInstalls)
    {
        if ($All)
        {
            $wixInstalls.Path 
            return 
        }

        $result = $wixInstalls | Sort-Object -Property Version | Select -First 1
        if ($result)
        {
            return $result.Path 
        }
        return 
    }
}