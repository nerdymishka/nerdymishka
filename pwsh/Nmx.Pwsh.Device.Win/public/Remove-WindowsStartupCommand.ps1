
function Remove-StartupCommand()
{
    [CmdletBinding(SupportsShouldProcess)]
    Param(
        [Alias("FileName")]
        [Parameter(Position = 0)]
        [String] $StartUpFileName = "DefaultAutoRun",

        [Parameter(Position = 1)]
        [String] $HomeDirectory = $HOME
    )

    Process
    {
        if ($PSCmdlet.ShouldProcess("Remove-Item", "$FileName"))
        {
            $roamingDir = "$HomeDirectory\AppData\Roaming"
            $startupFolder = "$roamingDir\Microsoft\Windows\Start Menu\Programs\Startup"
            $startFile = "$startupFolder\$StartUpFileName.bat"
            if (Test-Path $startFile)
            {
                Remove-Item $startFile -Force | Write-Debug
            }
        }
    }
}