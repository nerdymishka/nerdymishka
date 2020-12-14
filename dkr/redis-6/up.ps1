$ErrorActionPreference = "Stop"

if($IsWindows)
{
    $lines = docker network ls -f "name=nmx-backend-vnet" -q
}
else 
{
    $lines = sudo docker network ls -f "name=nmx-backend-vnet" -q
}

if($lines.Count -eq 0)
{
    . "$PsScriptRoot/../network/up.ps1"
}



$base = '/data'
if($IsWindows)
{
    $base = $env:ALLUSERsPROFILE.Replace("\", "/") + "/dkr"
}

# TODO: figure out redis log directory
$directories = @(
    "$base",
    "$base/data",
    "$base/data/redis",
    "$base/etc",
    "$base/etc/redis",
    "$base/log",
    "$base/log/redis"
)

$env:NMX_DKR_DATA = $base 

foreach($dir in $directories)
{
    if(!(Test-Path $dir))
    {
        New-Item $dir -ItemType Directory | Write-Debug 
    }
}
$envFile = "$PsScriptRoot/.env"

if(!(Test-Path $envFile))
{
    if(!(Get-Command New-GzPassword -EA SilentlyContinue))
    {
        Install-Module Gz-PasswordGenerator -Force 
    }

    $rootPw = New-GzPassword -AsString 
    $cfg = Get-Content "$PsScriptRoot/config.json" -Raw | ConvertFrom-Json 

    $content = @"
NMX_DKR=$base
NMX_REDIS_IPV4=$($cfg.ip4)
NMX_REDIS_PASSWORD=$rootPw
"@

    $content | Out-File $envFile -Encoding "utf8nobom"
}

if($IsWindows)
{
    docker-compose -f "$PsScriptRoot/docker-compose.yml" `
        --env-file "$PsScriptRoot/.env"  up -d
}
else 
{
    sudo docker-compose -f "$PsScriptRoot/docker-compose.yml" `
        --env-file "$PsScriptRoot/.env"  up -d
}