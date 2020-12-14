function Test-DynamicDockerNetwork()
{
    [CmdletBinding()]
    param (
        [Parameter(Position = 0)]
        [string]
        $NetworkName
    )

    if ($IsWindows)
    {
        $lines = docker network ls -f "name=$NetworkName" -q
    }
    else 
    {
        $lines = sudo docker network ls -f "name=$NetworkName" -q
    }

    return $lines.Length -gt 0
}