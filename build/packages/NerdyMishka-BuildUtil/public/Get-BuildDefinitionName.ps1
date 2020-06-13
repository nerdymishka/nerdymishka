
function Get-BuildDefinitionName()
{
    Param(
        [String] $Location
    )

    if($ENV:BUILD_DEFINITIONNAME) 
    {
        return $ENV:BUILD_DEFINITIONNAME
    }

    if($ENV:NM_BUILD_DEFINITIONAME) 
    {
        return $ENV:NM_BUILD_DEFINITIONAME
    }

    if([string]::IsNullOrWhiteSpace($Location))
    {
        $Location = "$($PWD.Path)"
    }
    

    $file = ".git/config"
    $path = "$($Location)/$file"
    $content = $null
    if((Test-Path $path))
    {
        $content = Get-Content $path 
    }

    if($content)
    {
        $next = $false;
        foreach($line in $content)
        {
            if($line -match "[remote `"origin`"]")
            {
                $next = $true;
                continue;
            }

            if($next -and ($line -match "url ="))
            {
                $name = $line.SubString($line.LastIndexOf("/")).Replace(".git", "")
                $ENV:NM_BUILD_DEFINITIONAME = "${name}_local"
                break;
            }
        }
    }

    return $ENV:NM_BUILD_DEFINITIONAME
}