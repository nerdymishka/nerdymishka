
param(
    [Parameter(Position = 0)]
    [String]
    $MigrationName,

    [Alias("Env")]
    [Parameter(Position = 1)]
    [String]
    $OperationalEnvironment
)

if(![string]::IsNullOrWhiteSpace($OperationalEnvironment))
{
    $env:NMX_ENVIRONMENT = $OperationalEnvironment
}

if(!(Get-Command dotnet-ef -EA SilentlyContinue))
{
    dotnet tool install --global dotnet-ef
}

dotnet-ef database update $MigrationName