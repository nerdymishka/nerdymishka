
function New-NugetConfig() 
{
    Param(
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
        [String] $Destination,

        [Switch] $Force 
    )

    if([string]::IsNullOrWhiteSpace($Destination))
    {
        $path = $PWD.Path.Replace("\", "/")
        $Destination = $path 
    }

    if((Test-PAth "$Destination/nuget.confg") -and $force.ToBool())
    {
        remove-item "$Destination/nuget.config" -Force | Write-Debug 
    }

    dotnet new nuget -o $path 
}