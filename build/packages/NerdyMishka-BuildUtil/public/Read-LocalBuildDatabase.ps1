function Read-LocalBuildDatabase()
{
    Param(
        [Alias("Query", "PathQuery")]
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [String] $InputObject,

        [Parameter(Position = 1)]
        [String] $DatabasePath 
    )

    $db = Get-LocalBuildDatabase $DatabasePath
    if([string]::IsNullOrWhiteSpace($InputObject))
    {
        return $db;
    }

    $segments = $InputObject.Split("/");
    $target = $db;
    for($i = 0; $segments.Length; $i++)
    {
        $key = $segments[$i];
        $next = $target[$key]
        
        if($Null -eq $next)
        {
            return $null;
        }

        if($i -lt $segments.Length)
        {
            if(!($next -is [PsCustomObject]) -and !($next -is [Hashtable]))
            {
                return $null 
            }

            $target = $next;
        }

        return $next;
    }
}