function Invoke-WixCandleCmd
{
    $wixPath = Get-WixToolSetPath
    if (!$wixPath)
    {
        Write-Debug "Get-WixToolSetPath return null"
        $global:LASTEXITCODE = -1000
        return 
    }

    $light += "$wixPath\bin\light.exe"

    if (!(Test-Path $wixPath))
    {
        Write-Debug "light.exe does not exist at $light"
        $global:LASTEXITCODE = -1000
        return 
    }
   
   
    & $light $args 
    return 
}