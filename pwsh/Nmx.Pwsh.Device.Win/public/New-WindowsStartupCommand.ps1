function New-WindowsStartupCommand()
{
    [CmdletBinding(SupportsShouldProcess)]
    Param(
        [Parameter(Position = 0)]
        [String] $Command,

        [Alias("FileName")]
        [Parameter(Position = 1)]
        [String] $StartUpFileName = "DefaultAutoRun",

        [Parameter(Position = 2)]
        [String] $HomeDirectory = $HOME
    )

    process
    {
        if ($PSCmdlet.ShouldProcess("Create startup file", "$StartUpFileName"))
        {
            $roamingDir = "$HomeDirectory\AppData\Roaming"
            $startupFolder = "$roamingDir\Microsoft\Windows\Start Menu\Programs\Startup"
            New-Item "$startupFolder\$StartUpFileName.bat" -type file -force -value $Command | Write-Debug
        }
    }
}