
function New-NugetConfig() 
{
    Param(
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
        [String] $Destination,
        
        [Parameter(ValueFromPipeline = $true)]
        [String] $InputObject,

        [Switch] $Force 
    )

    if([string]::IsNullOrWhiteSpace($Destination))
    {
        $path = $PWD.Path.Replace("\", "/")
        $Destination = "$path/Nuget.config"
    }

    if(!$Destination.EndsWith("Nuget.config"))
    {
        $Destination += "/Nuget.config";
    }

    $xmlString = "<?xml version=`"1.0`" encoding=`"utf-8`"?>
    <configuration>
      <packageSources>
        <clear />
        <add key=`"nuget.org`" value=`"https://api.nuget.org/v3/index.json`" />
      </packageSources>
    </configuration>"

    if(![string]::IsNullOrWhiteSpace($InputObject))
    {
        $xmlString = $InputObject;
    }

    $path = [IO.Path]::GetDirectoryName($Destination)

    if(!(Test-Path $path))
    {
        if($Force.ToBool())
        {
            New-Item $path -ItemType Directory | Write-Debug 
        } 
        else 
        {
            throw [IO.DirectoryNotFoundException] $path 
        }      
    }



    $xml = [xml]$xmlString
    if(Test-Path $Destination)
    {
        if($Force.ToBool())
        {
            Remove-Item $Destination -Force 
            $xml.Save($Destination);
        }

        return;
    }
    
    $xml.Save($Destination);
}