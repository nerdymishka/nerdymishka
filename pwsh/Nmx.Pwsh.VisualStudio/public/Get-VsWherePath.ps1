function Get-VsWherePath()
{
    $ErrorActionPreference = 'Stop'
    if (!$IsWindows)
    {
        Write-Debug "Get-VsWherePath is windows only"
        return $null 
    }

    if ($Env:NMX_VSWHERE)
    {
        return $Env:NMX_VSWHERE
    }

    $base = "${Env:ProgramFiles(x86)}"

    if ([System.IntPtr]::Size -eq 4)
    {
        $base = $Env:ProgramFiles
    }
    
    $defaultLocation = "$base\Microsoft Visual Studio\Installer\vsWhere.exe"

    if (Test-Path $defaultLocation)
    {
        $Env:NMX_VSWHERE = $defaultLocation
        return $defaultLocation
    }

    $cmd = Get-Command vswhere.exe -EA SilentlyContinue
    if ($cmd)
    {
        $Env:NMX_VSWHERE = $cmd.Path
        return $Env:NMX_VSWHERE
    }

    Write-Debug "Could not locate vswhere.exe on path or the default location"
    return $null;
}
    