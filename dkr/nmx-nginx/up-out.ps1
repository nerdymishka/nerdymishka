$ErrorActionPreference = "Stop"
Import-Module "$PSScriptRoot/../scripts/Nmx.Docker/Nmx.Docker.psm1" -Force

if (!(Test-DynamicDockerNetwork "nmx-frontend-vnet"))
{
    . "$PSScriptRoot/../network/up.ps1"
}

Invoke-DynamicDockerCompose --dir $PsScriptRoot --environment $env:NMX_ENV up