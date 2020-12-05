function Sync-SavePoint()
{
    [OutputType([void])]
    [CmdletBinding()]
    param (
        [Parameter(Position = 0)]
        [String]
        $Name,
        
        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Hashtable]
        $InputObject,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [ScriptBlock]
        $Test,

        [Parameter(Position = 1)]
        [ScriptBlock]
        $Task
    )

    if (!$InputObject)
    {
        $InputObject = Read-SaveState
    }

    $completed = $false;
    if (!$Test)
    {
        $Test = {
            Param(
                [Parameter(Position = 0)]
                [Hashtable]
                $State,

                [Parameter(Position = 1)]
                [String]
                $Name 
            )
            $result = $null;
            if (!$Name.Contains("."))
            {
                $result = $State[$Name]
            }
            else 
            {
                $segments = $Name.Split(".")
                $last = $segments.Length - 1;
                $node = $State
                for ($i = 0; $i -lt $segments.Length; $i++)
                {
                    $s = $segments[$i];
                    $next = $node[$s];
                    if ($i -eq $last)
                    {
                        $result = $next;
                        break;
                    }

                    if ($null -eq $next)
                    {
                        break;
                    }
                    
                    if ($next -is [Hashtable])
                    {
                        $node = $next;
                        continue;
                    }

                    break;
                }
            }

            if ($null -ne $result)
            {
                if ($result -is [Boolean])
                {
                    return $result;
                }

                return $true;
            }

            return $false;
        }
    }

    $completed = . $Test -State $InputObject -Name $Name 
    if ($completed)
    {
        Write-Verbose "Step $Name Previously Completed"
        return;
    }

    $Data = $null;
    if ($Task)
    {
        $ErrorActionPreference = "Stop"
        $Data = . $Task -State $InputObject -Name $Name 
    }
    
    $InputObject | Write-SaveState -Name $Name -Data $Data
}