$ErrorActionPreference = "Stop"
Import-Module "$PSScriptRoot/../scripts/Nmx.Docker/Nmx.Docker.psm1" -Force

if (!(Test-DynamicDockerNetwork "nmx-backend-vnet"))
{
    . "$PSScriptRoot/../network/up.ps1"
}

$base = Get-DynamicDockerDirectory
if (!(Test-Path "$base/etc/vault_1/vault.json"))
{
    if ($IsWindows)
    {
        mkdir "$base/etc/vault_1"
        cp ./assets/vault.json "$base/etc/vault_1"
    }
    else 
    {
        sudo mkdir "$base/etc/vault_1"
        sudo chmod 775 "$base/etc/vault_1"
        sudo cp $PsScriptRoot/assets/vault.json $base/etc/vault_1/
    }
}

Invoke-DynamicDockerCompose --dir $PsScriptRoot --environment $env:NMX_ENV up