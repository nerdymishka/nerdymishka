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
        $global:IsProcessElevated = [Security.Principal.WindowsIdentity]::GetCurrent().Owner.IsWellKnown("BuiltInAdministratorsSid")
    }
    else
    {
        $global:IsProcessElevated = 0 -eq (id -u)
    }

    Write-Debug ("IsProcessElevated".PadRight(40) + $Global:IsProcessElevated)
}