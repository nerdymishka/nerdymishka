function Set-VsTestPath()
{
    [CmdletBinding()]
    param (
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 0)]
        [String]
        $Path
    )

    if ((Test-Path $Path) -and $Path.EndsWith("vstest.console.exe"))
    {
        Write-Debug "env:NMX_MSBUILD = $Path"
        $env:NMX_MSBUILD = $Path
    }
}