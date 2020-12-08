function Get-ModernVsPath()
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

    if ($Env:NMX_VS_HOME -and !$All -and !$Pre)
    {
        return $Env:NMX_VS_HOME;
    }

    $vsWhere = Get-VsWherePath 
    if ($vsWhere)
    {
        $latest = ""
        if (!$All)
        {
            $latest = "-latest"
        }
        if ($Pre)
        {
            $prerelease = "-prerelease"
        }

        & $vsWhere $latest $prerelease -product *  -requires Microsoft.Component.MSBuild -property installationPath
        return;
    }

    return $null;
}