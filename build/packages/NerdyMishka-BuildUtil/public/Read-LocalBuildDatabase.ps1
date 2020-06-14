function Read-LocalBuildDatabase()
{
    Param(
        [Alias("Query", "PathQuery")]
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [String] $InputObject,

        [Parameter(Position = 1)]
        [String] $DatabasePath 
    )

    $db = Get-LocalBuildDatabase -InputObject $DatabasePath
    if([string]::IsNullOrWhiteSpace($InputObject))
    {
        return $db;
    }

    $segments = $InputObject.Split("/");
    $target = $db;
    for($i = 0; $i -lt $segments.Length; $i++)
    {
        $key = $segments[$i];
        $next = $target.$key 
        
        if($Null -eq $next)
        {
            return $null;
        }

        
        $target = $next;

        if($i -lt ($segments.Length - 1))
        {
            if(!($next -is [PsCustomObject]) -and `
              !($next -is [Hashtable]) -and `
              !($next -is [Array]))
            {
                return $null 
            }

            continue;
        }

        return $next;
    }
}