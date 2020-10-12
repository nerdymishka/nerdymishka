
Set-Alias "psake" "invoke-psake"

$Env:DOCKER_DISK = "$HOME/.cache/docker_disk"

$dd = $Env:DOCKER_DISK

$paths = @(
    "$dd",
    "$dd/cache",
    "$dd/etc",
    "$dd/log",
    "$dd/secrets",
    "$dd/cache/mssql",
    "$dd/log/mssql",
    "$dd/cache/mysql",
    "$dd/etc/mysql",
    "$dd/cache/postgres",
    "$dd/etc/postgres",
    "$dd/cache/redis",
    "$dd/etc/redis",
    "$dd/cache/rabbitmq",
    "$dd/etc/rabbitmq",
    "$dd/log/rabbitmq",
    "$dd/cache/mongo"
)

foreach($p in $paths)
{
    if(!(Test-Path $p))
    {
        New-Item $p -ItemType Directory 
    }
}
