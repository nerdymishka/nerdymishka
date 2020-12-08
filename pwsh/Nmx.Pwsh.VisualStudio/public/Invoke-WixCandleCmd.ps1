function Invoke-WixCandleCmd
{
    $wixPath = Get-WixToolSetPath
    if (!$wixPath)
    {
        Write-Debug "Get-WixToolSetPath return null"
        $global:LASTEXITCODE = -1000
        return 
    }

    $candle += "$wixPath\bin\candle.exe"

    if (!(Test-Path $wixPath))
    {
        Write-Debug "candle.exe does not exist at $candle"
        $global:LASTEXITCODE = -1000
        return 
    }
   
   
    & $candle $args 
    return 
}