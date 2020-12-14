function New-DockerDirectory()
{
    [CmdletBinding()]
    param (
        [Parameter(Position = 0)]
        [String[]]
        $Directories
    )

    $base = '/dkr'
    if ($IsWindows)
    {
        $base = $env:ALLUSERsPROFILE.Replace("\", "/") + "/dkr"
    }

    if (!(Test-Path $base))
    {
        New-Item $base -ItemType Directory | Write-Debug
        if (!$IsWindows)
        {
            sudo chmod 755 $base 
        }
    }

    foreach ($dir in $Directories)
    {
        $next = "$base/$dir"
        if (!(Test-Path $next))
        {
            New-Item $next -ItemType Directory | Write-Debug
        }
    }
}