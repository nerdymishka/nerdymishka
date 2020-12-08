function Set-VsHome()
{
    [CmdletBinding()]
    param (
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 0)]
        [String]
        $Path
    )

    if (Test-Path $Path)
    {
        Write-Debug "env:NMX_VSHOME = $Path"
        $env:NMX_VSHOME = $Path
    }
}