function Find-GitProjectRoot()
{
    Param(
        [string] $Location 
    )

    if([string]::IsNullOrWhiteSpace($Location))
    {
        $Location = $PWD.Path;
    }
    $dir = $Location
    $originalPath = $dir;

    while(!(Test-Path "$dir/.git"))
    {
        $dir = $dir | Split-Path -Parent 

        if([string]::IsNullOrWhiteSpace($dir))
        {
            Write-Debug "Could locate .git directory for path or ancestors : $originalPath"
            return $null 
        }
    }

    return $dir.Replace("\", "/").TrimEnd("/")
}