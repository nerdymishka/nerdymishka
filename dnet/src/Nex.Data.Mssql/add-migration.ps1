param(
    [String] $MigrationName
)

if(!(Get-Command dotnet-ef -EA SilentlyContinue))
{
    dotnet tool install --global dotnet-ef
}

dotnet-ef migrations add $MigrationName --output-dir $PsScriptRoot/Migrations