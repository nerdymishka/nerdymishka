$ErrorActionPreference = "Stop"

$base = '/data'
if($IsWindows)
{
    $base = $env:ALLUSERsPROFILE.Replace("\", "/") + "/dkr"
}

$directories = @(
    "$base",
    "$base/data",
    "$base/data/mysql",
    "$base/etc",
    "$base/etc/mysql",
    "$base/log",
    "$base/log/mysql"
)

if(!$IsWindows)
{
    sudo chmod 755 -R $base 
}

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
    $userPw = New-GzPassword -AsString 


    $content = @"
NMX_DKR=$base
NMX_MYSQL_DB=nmx_default
NMX_MYSQL_USER=nmx
NMX_MYSQL_ROOT_PASSWORD=$rootPw
NMX_MYSQL_PASSWORD=$userPw
"@

    $env:NMX_DKR=$base
    $env:NMX_MYSQ

    $content | Out-File $envFile -Encoding "utf8nobom"
}

Import-Module "$PsScriptRoot/../Nmx-DotEnv.psm1" -Force 
Read-DotEnv -Path "$PsScriptRoot/.env"

sudo docker-compose -f "$PsScriptRoot/docker-compose.yml" `
    --env-file "$PsScriptRoot/.env"  up -d