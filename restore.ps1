
Set-Alias "psake" "invoke-psake"

if ($null -eq (Get-Command nbgv -EA SilentlyContinue))
{
    dotnet tool install -g nbgv --version 3.1.91
}