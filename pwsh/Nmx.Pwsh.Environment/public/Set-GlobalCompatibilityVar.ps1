function Set-GlobalCompatibilityVar()
{
    [CmdletBinding()]
    param ()

    $hasGlobals = ($null -eq (Get-Variable -Name "IsWindows" -Scope Global -EA SilentlyContinue))

    if (!$hasGlobals)
    {
        $platform = [System.Environment]::OSVersion.Platform
        $Global:IsWindows = $platform.ToString().Contains("Win")
        $Global:IsLinux = $platform -eq "Unix"
        $Global:IsMacOS = $platform -eq "MacOSX"
        $Global:IsCoreCLR = $Host.Version.Major -gt 5
    }

    Write-Debug ("IsWindows".PadRight(40) + $Global:IsWindows)
    Write-Debug ("IsLinux".PadRight(40) + $Global:IsLinux)
    Write-Debug ("IsMaxOS".PadRight(40) + $Global:IsMacOS)
    Write-Debug ("IsCoreCLR".PadRight(40) + $Global:IsCoreCLR)

    if ($IsWindows)
    {
        $global:IsElevatedProcess = [Security.Principal.WindowsIdentity]::GetCurrent().Owner.IsWellKnown("BuiltInAdministratorsSid")
    }
    else
    {
        $global:IsElevatedProcess = 0 -eq (id -u)
    }
    
    $global:Is64BitProcess = [Environment]::Is64BitProcess
    $global:Is64BitOs = [Environment]::Is64BitOperatingSystem

    Write-Debug ("IsElevatedProcess".PadRight(40) + $global:IsElevatedProcess)
    Write-Debug ("Is64BitProcess".PadRight(40) + $global:Is64BitProcess)
    Write-Debug ("Is64BitOs".PadRight(40) + $global:Is64BitOs)
}