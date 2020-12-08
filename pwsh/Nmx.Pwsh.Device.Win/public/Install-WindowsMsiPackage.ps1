function Install-WindowsMsiPackage()
{
    Param(
        [String] $Path,

        [String] $Arguments = "/qn",

        [String] $LogFile = $null,

        [int[]] $ExitCodes = @(0, 1605, 1614, 1641, 3010)
    )

    if (!(Test-Path $Path))
    {
        throw [System.IO.FileNotFoundException] $Path
    }

    $Path = $Path.Replace("/", "\")
    $fileName = (Split-Path $Path -Leaf).Replace(".msi", "")

    if (![String]::IsNullOrWhiteSpace($LogFile))
    {
        $logDir = $LogFile | Split-Path -Parent
    }
    else 
    {
        $logDir = "$Env:Temp\nmx\installers\logs"
        $logFile = "$($logDir)\$($fileName)_$([DateTime]::UtcNow.ToString("yyyy-MM-dd_hh_mm")).log"
    }

    if (!(Test-Path $logDir))
    {
        New-Item $logDir -ItemType Directory | Write-Debug
    }

    "" > $logFile

    $result = Invoke-Process "msiexec.exe" -Arguments "/i `"$Path`" $Arguments  /L*V `"$LogFile`""

    $success = $ExitCodes -contains $result.ExitCode 
    $result | Add-Member -NotePropertyName "sucess" -NotePropertyValue $success -Force
    Write-Output $result
}