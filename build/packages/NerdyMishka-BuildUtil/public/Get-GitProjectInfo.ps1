
function Get-GitProjectInfo() {
    Param(
        [String] $Location,

        [String] $Config = ".git/config",

        [String] $Remote = "origin"
    )

    if([string]::IsNullOrWhiteSpace($Location))
    {
        $Location = $PWD.Path
    }

    $cfgParts = $Config.Split("/")
    $Location = $Location.Replace("\\", "/")
    if(!$Location.EndsWith($Config))
    {
        
        if($cfgParts.Length -eq 2)
        {
            if($Location.EndsWith($cfgParts[0]))
            {
                $Location += "/" + $cfgParts[1];
            } else {
                $Location += "$($cfgParts[0])/$($cfgParts[1])"
            }
        } else {
            $Location += $cfgParts[0]
        }
    }

    $originalPath = $Location
    while(!(Test-Path $Location))
    {
        if($cfgParts.Length -eq 2)
        {
             # pop config
            $dir = $Location | Split-Path -Parent 
            # pop .git
            $dir = $dir | Split-Path -Parent
        } else {
            
            # pop config
            $dir = $Location | Split-Path -Parent 
        }
                
        # pop current directory
        $dir = $dir | Split-Path -Parent

        if([string]::IsNullOrWhiteSpace($dir))
        {
            Write-Debug "Could locate .git/config in the current path or directory ancestors: $originalPath"
            return $Null
        }

        $Location = "$dir/.git/config"
    }

    $content = Get-Content $Location

    if($content)
    {
        $next = $false;
        foreach($line in $content)
        {
            if($line -match "[remote `"$Remote`"]")
            {
                $next = $true;
                continue;
            }

            if($next -and ($line -match "url ="))
            {
                $uri = [Uri]$line.Replace("url =", "").Trim()
                $segments = $uri.LocalPath.Trim("/")
                if($uri.Authority -eq "dev.azure.com")
                {
                    $org = $segments[0]
                    $team = $segments[1]
                    $repo = $segments[3]

                    return [PSCustomObject]@{
                        Organization = $org 
                        Project = $team 
                        RepositoryName = $repo
                        RepositoryUri = $uri.ToString()
                    }
                }
                
                # github/gitlab is going to follow this format
                $project = $segments[1].Replace(".git", "")
                return [PSCustomObject]@{
                    Organization = $segments[0]
                    Project = $project
                    RepositoryName = $project
                    RepositoryUri = $uri.ToString()
                }
            }
        }
    }
    
    return $null 
}