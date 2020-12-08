function Set-VsWherePath()
{
    [CmdletBinding()]
    param (
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 0)]
        [String]
        $Path
    )

    if ((Test-Path $Path) -and $Path.EndsWith("vswhere.exe"))
    {
        Write-Debug "env:NMX_VSWHERE = $Path"
        $env:NMX_VSWHERE = $Path
    }
}