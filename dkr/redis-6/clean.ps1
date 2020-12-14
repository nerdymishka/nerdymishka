$ErrorActionPreference = "Stop"

. ./down.ps1

$base = '/data'
if($IsWindows)
{
    $base = $env:ALLUSERsPROFILE.Replace("\", "/") + "/dkr"
}

$directories = @(
    "$base/data/redis",
    "$base/etc/redis",
    "$base/log/redis"
)

foreach($dir in $directories)
{
    if(Test-Path $dir)
    {
        Remove-Item $dir -Recurse -Force 
    }
}