$ErrorActionPreference = "Stop"

. ./down.ps1

$base = '/data'
if($IsWindows)
{
    $base = $env:ALLUSERsPROFILE.Replace("\", "/") + "/dkr"
}

$directories = @(
    "$base/data/mysql",
    "$base/etc/mysql",
    "$base/log/mysql"
)

foreach($dir in $directories)
{
    if(Test-Path $dir)
    {
        Remove-Item $dir -Recurse -Force 
    }
}