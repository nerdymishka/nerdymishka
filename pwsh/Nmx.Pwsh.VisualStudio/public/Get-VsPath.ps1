function Get-VsPath()
{
    [CmdletBinding()]
    param (
        [Parameter()]
        [Switch]
        $All,

        [Parameter()]
        [Switch]
        $Pre
    )

    # TODO: support older versions
    return Get-ModernVsPath -All:$All -Pre:$Pre 
}