function Get-DynamicDockerDirectory()
{
    $base = '/dkr'
    if ($IsWindows)
    {
        $base = $env:ALLUSERsPROFILE.Replace("\", "/") + "/dkr"
    }

    return $base;
}