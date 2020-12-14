$ErrorActionPreference = "Stop"
Import-Module "$PSScriptRoot/../scripts/Nmx.Docker/Nmx.Docker.psm1" -Force

if (!(Test-DynamicDockerNetwork "nmx-frontend-vnet"))
{
    . "$PSScriptRoot/../network/up.ps1"
}

$base = Get-DynamicDockerDirectory 
$etc = "$base/etc/nginx"
if (!(Test-Path $etc))
{
    if ($IsWindows)
    {
        mkdir $etc 
        mkdir "$etc/includes"
        mkdir "$etc/sites-available"
        mkdir "$etc/sites-enabled"
    }
    else 
    {
        sudo mkdir $etc 
        sudo mkdir "$etc/includes"
        sudo mkdir "$etc/site-available"
        sudo mkdir "$etc/sites-enabled"
        sudo chown root:docker $etc -R
        sudo chown 755 $etc -R 
    }
}

if ($IsWindows)
{
    if (!(Test-Path "$etc/nginx.conf"))
    {
        Copy-Item "./assets/*" "$etc/nginx/" -Recurse
    }
}
else 
{
    if (!(Test-Path "$etc/nginx.conf"))
    {
        sudo cp -R $PSScriptRoot/assets/* $etc/ 
        sudo chown root:docker $etc -R
        sudo chown 755 $etc -R 
    }
}



Invoke-DynamicDockerCompose --dir $PsScriptRoot --environment $env:NMX_ENV up