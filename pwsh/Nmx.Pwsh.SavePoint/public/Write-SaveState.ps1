function Write-SaveState()
{
    [CmdletBinding()]
    param (
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String]
        $Name,

        [Parameter(Position = 1)]
        [Object]
        $Data,
        
        [Alias("State")]
        [Parameter(ValueFromPipeline = $true)]
        [Hashtable]
        $InputObject
    )

    if ($null -eq $Data) 
    {
        $Data = [DateTime]::UtcNow
    }

    $stateFile = Get-SaveStateFileName
    $ht = $InputObject
    if (!$ht)
    {
        $ht = Read-SaveState $stateFile
    }

    if (!$Name.Contains("."))
    {
        $ht[$Name] = $Data
    }
    else 
    {
        $node = $ht;
        $segments = $Name.Split(".");
        $path = [String]::Empty
        $last = $segments.Length - 1 
        for ($i = 0; $i -lt $segments.Length; $i++)
        {
            $segment = $segments[$i];


            if ($i -eq $last)
            {
                $node[$segment] = $Data
                break;
            }

            $path += ".$segment"
            $path = $path.Trim(".")
            $next = $node[$segment];

            if ($null -eq $next)
            {
                $next = @{};
                $node[$segment] = $next;
                $node = $next;
                continue;
            }

            if ($next -is [Hashtable])
            {
                $node = $next;
                continue;
            }
          
            throw "$path already has a value of type $($next.GetType().FullName)"
        }
    }

    $json = $ht | ConvertTo-Json -Depth 20
    if ($VerbosePreference -eq "Continue")
    {
        Write-Verbose $json 
    }

    Write-Debug "Saving state file: $stateFile"
    if ($Host.Version.Major -gt 5)
    {
        Write-Debug "Saving state file: $stateFile"
        $json | Out-File $stateFile -Encoding utf8NoBOM 
    }
    else 
    {
        [IO.File]::WriteAllText($stateFile, $json)
    }
}

Set-Alias "Save-Point" "Write-SaveState"