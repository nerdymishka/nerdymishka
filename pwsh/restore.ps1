[CmdletBinding()]
param (
    [Parameter()]
    [Switch]
    $Force
)


if (!(Test-Path ".setup") -or $Force) 
{
    $ErrorActionPreference = "Stop"
    if (!$IsCoreCLR)
    {
        $isElevated = [bool](([System.Security.Principal.WindowsIdentity]::GetCurrent()).groups -match "S-1-5-32-544")

        if (!(Get-Command $pwsh -EA SilentlyContinue))
        {
            if (!$isElevated)
            {
                Write-Warning "re-run script as administrator"
                return 
            }

            if (!$env:ChocolateyInstall) { $env:ChocolateyInstall = "C:\apps\chocolatey" }
            if (!$env:ChocolateyToolsLocation ) { $env:ChocolateyToolsLocation = "C:\apps" }

            if ($null -eq (Get-Command choco.exe -ErrorAction SilentlyContinue))
            {
                Set-ExecutionPolicy Bypass -Scope Process -Force; 
                [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; 
                iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
            }

            choco install powershell-core -yf 
        }

        pwsh.exe -NoExit -ExecutionPolicy Bypass -File "$PSScriptRoot/restore.ps1"
        return 
    }

    if ($IsWindows)
    {
        $isElevated = [bool](([System.Security.Principal.WindowsIdentity]::GetCurrent()).groups -match "S-1-5-32-544")

        if (!$env:ChocolateyInstall) { $env:ChocolateyInstall = "C:\apps\chocolatey" }
        if (!$env:ChocolateyToolsLocation ) { $env:ChocolateyToolsLocation = "C:\apps" }

        if ($null -eq (Get-Command choco.exe -ErrorAction SilentlyContinue))
        {
            if (!$isElevated)
            {
                Write-Warning "re-run script as administrator"
                return 
            }

            Set-ExecutionPolicy Bypass -Scope Process -Force; 
            [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; 
            iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
        }

        if ($null -eq (Get-Command nuget.exe -ErrorAction SilentlyContinue))
        {
            if (!$isElevated)
            {
                Write-Warning "re-run script as administrator"
                return 
            }

            choco install nuget.commandline -yf 
        }

        if ($null -eq (Get-Command dotnet.exe -EA SilentlyContinue))
        {
            if (!$isElevated)
            {
                Write-Warning "re-rung script as administrator"
                return 
            }

            choco install dotnet-sdk
        }
    }

    $mods = @("Psake", "Pester", "PsReadline")
    foreach ($mod in $mods)
    {
        $installed = Get-Module $mod -ListAvailable -EA SilentlyContinue
        if ($installed)
        {
            Update-Module $mod 
        }
        else 
        {
            Install-Module $mod -Repository PsGallery -Force
        }
    }

    "" > .setup
}