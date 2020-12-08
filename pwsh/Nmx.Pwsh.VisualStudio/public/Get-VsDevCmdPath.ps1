function Get-VsDevCmdPath()
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

    $ErrorActionPreference = 'Stop'
    if ($env:NMX_VSDEVCMD -and !$All -and !$Pre)
    {
        return $env:NMX_VSDEVCMD 
    }

    if ($env:NMX_VS_HOME -and !$All -and !$Pre)
    {
        $items = Get-Item "$env:NMX_VS_HOME\Common7\Tools\VsDevCmd.bat" -EA SilentlyContinue
        if ($items)
        {
            return $items.FullName
        }
    }

    $vsWhere = Get-VsWherePath 
    if ($vsWhere)
    {
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

            $findStr = "Common7\Tools\VsDevCmd.bat"
          
            # '-product *' will intercept visual studio build tools
            & $vsWhere $latest $prerelease -products *  `
                -find $findStr
            return;
        }
    }
}