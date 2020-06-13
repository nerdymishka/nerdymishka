

function New-BuildNumber() {
    Param(
        [String] $Format = '$(BUILD.DEFINITIONNAME)_(DATE:yyyyMMdd).{{Rev:rr}}',

        [String] $BuildNumber 
    )

    # build number is already set
    if($ENV:NM_BUILD_NUMBER)
    {
        return $ENV:NM_BUILD_NUMBER 
    }

    # build number was supplied
    if(![string]::IsNullOrWhiteSpace($BuildNumber))
    {
        $ENV:NM_BUILD_NUMBER = $BuildNumber;
        return $BuildNumber;
    }

    # build number exists
    if($ENV:BUILD_NUMBER)
    {
        $ENV:NM_BUILD_NUMBER = $ENV:BUILD_NUMBER
        return $ENV:BUILD_NUMBER
    }

    $definition = Get-BuildDefinitionName 
    $section = Get-LocalBuildDatabase $defintion 
    $branch = Get-GitBranch -Current  
    $r = 0
    if(!$section)
    {
        $info = Get-GitProjectName
        
        $teamProject = $ENV:SYSTEM_TEAMPROJECT
        if(!$teamProject) { $teamProject = $ENV:NM_PROJECT }
        if(!$teamProject) { $info.Project }


        $data = [PsCustomObject]@{
            revision = 0
            buildId = 0
            lastBuild = [DateTime]::UtcNow
            project = $teamProject
            sourceBranch = $branch 
        } 
        $section | Add-Member -MemberType NoteProperty -Name $definition -Value $data 
    }
    $now = [datetime]::UtcNow

    $r = $section.revision
    $buildId = $section.buildId 
    $lastBuild = $section.lastBuild 
    if($lastBuild.Year -ne $now.Year -or $lastBuild.Month -ne $now.Month -or $lastBuild.Day -ne $now.Day)
    {
        $r = 0
        $section.revision = 0;
    }

    $r = $r+1 
    $buildId = $buildId + 1;
    $section.buildId = $buildId
    $section.revision = $r;
    $section.lastBuild = $date 
    $section.sourceBranch = $branch

    $db[$definition] = $section

    $db | Write-LocalBuildDatabase

    $model = @{
        "Build.DefinitionName" = $definition
        "TeamProject" = $section.TeamProject
        "BuildId" = $buildId 
        "Rev" = $r 
        "SourceBranchName" = $branch
        "Date" = $now
        "DayOfYear" = $now.DayOfYear
        "DayOfMonth" = $now.DayOfMonth 
        "Hours" = $now.Hours 
        "Minutes" = $now.Minutes 
        "Month" = $now.Month
        "Seconds" = $now.Seconds 
        "Year" = $now.YEar 
    }
    
    $c = $Format;

    $sb = New-Object System.Text.StringBuilder 
    $token = New-Object System.Text.StringBuilder  
    $inToken = $false;
    for($i = 0; $i -lt $Format.Length; $i++)
    {
        $c = $Format[$i];
        if($c -eq '$' -and $Format[$i + 1] -eq "(")
        {
            $inToken = $true;
            continue;
        }

        if($inToken)
        {
            if([Char]::IsLetterOrDigit($c) -or $c -eq '.' -or $c -eq "_" -or $c -eq ":")
            {
                $token.Append($c);
                continue;
            }

            if($c -eq ')')
            {
                $inToken = $false;
                $t = $token.ToString()
                $token = New-Object System.Text.StringBuilder
                $format = $null 
                if($t -match ":")
                {
                    $p = $t.Split(":");
                    $format = $p[1]
                    $t = $p[0];
                }
                
                $value = "";
                foreach($key in $model.Keys)
                {
                    # ContainsKey is case sensitive
                    # using -eq instead 
                    if($key -eq $t)
                    {
                        $value = $model[$key]
                        break;
                    }
                }

                if($format)
                {
                    if($value -is [Date])
                    {
                        $value = [string]::Format($value, $format)
                    }
                    
                    if($value -is [Int32])
                    {
                        $format = $format.Replace($format[0], "0")
                        $value = [string]::Format("{0:$format}", $value)
                    }
                }

                $sb.Append($value)
                continue;
            }
        }

        $sb.Append($c);
    }

    $ENV:NM_BUILD_NUMBER = $sb.ToString()
    return $ENV:NM_BUILD_NUMBER
}