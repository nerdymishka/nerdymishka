function Get-MsBuildPath()
{
    [CmdletBinding()]
    param (
        [Parameter()]
        [Switch]
        $All,

        [Parameter()]
        [Switch]
        $Pre,

        [Parameter]
        [Switch]
        $X64 
    )

    $ErrorActionPreference = 'Stop'
    if ($env:NMX_MSBUILD -and !$All -and !$Pre)
    {
        return $env:NMX_MSBUILD
    }

    if ($env:NMX_VS_HOME -and !$All -and !$Pre)
    {
        $items = Get-Item "$env:NMX_VS_HOME\MsBuild\**\Bin\MsBuild.exe" -EA SilentlyContinue
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

            $findStr = "MSBuild\**\Bin\MSBuild.exe"
            if ($X64)
            {
                $findStr = "MSBuild\**\Bin\amd64\MSBuild.exe"
            }
            # '-product *' will intercept visual studio build tools
            & $vsWhere $latest $prerelease -products *  -requires Microsoft.Component.MSBuild `
                -find $findStr
            return;
        }
    }

    return $null
}