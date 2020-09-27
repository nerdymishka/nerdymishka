

function New-BuildNumber() {
    Param(
        [String] $Format = $null,

        [String] $BuildNumber,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [String] $DatabasePath 
    )

    if([string]::IsNullOrWhiteSpace($Format))
    {
        if($ENV:NM_BUILD_NUMBER_FORMAT)
        {
            $Format = $ENV:NM_BUILD_NUMBER_FORMAT
        } else {
            $Format = '$(Build.DefinitionName)_$(DATE:yyyyMMdd).$(Rev:rr)'
        }
    }

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
    $db = Get-LocalBuildDatabase -InputObject $DatabasePath 
    $section = Read-LocalBuildDatabase -Query $definition -DatabasePath $DatabasePath
    $branch = Get-GitBranch -CurrentOnly
    $r = 0
    $now = [datetime]::UtcNow
    if(!$section)
    {
       
        $teamProject = $ENV:SYSTEM_TEAMPROJECT
        if(!$teamProject) { $teamProject = $ENV:NM_PROJECT }
        if(!$teamProject) { 
            $info = Get-GitProjectInfo 
            $teamProject = $info.project 
        }

        $data = [PsCustomObject]@{
            revision = 0
            buildId = 0
            lastBuild = $now 
            project = $teamProject
            sourceBranch = $branch 
        } 
        $db | Add-Member -MemberType NoteProperty -Name $definition -Value $data
        $section = $data 
    }
    
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
    $section.lastBuild = $now; 
    $section.sourceBranch = $branch

   
    $db | Add-Member -MemberType NoteProperty -Force -Name $definition -Value $section | Out-Null 
    $db | Write-LocalBuildDatabase  -DatabasePath $DatabasePath

    $model = @{
        "Build.DefinitionName" = $definition
        "TeamProject" = $section.project
        "Project" = $section.project 
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
        "Year" = $now.Year 
    }
    
    $sb = New-Object System.Text.StringBuilder 
    $token = New-Object System.Text.StringBuilder  
    $inToken = $false;
    $f = $Format
    for($i = 0; $i -lt $f.Length; $i++)
    {
        $c = $f[$i];
        if($c -eq '$' -and $f[$i + 1] -eq "(")
        {
            $i++;
            $inToken = $true;
            continue;
        }

        if($inToken)
        {

            if([Char]::IsLetterOrDigit($c) -or $c -eq '.' -or $c -eq "_" -or $c -eq ":")
            {
                [void]$token.Append($c);
                continue;
            }

            if($c -eq ')')
            {
                $inToken = $false;
                $t = $token.ToString()
                [void]$token.Clear()
                $tokenFormat = $null

                if($t -match ":")
                {
                    $p = $t.Split(":");
                    $tokenFormat = $p[1]
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

                if($tokenFormat)
                {
                    if($value -is [DateTime])
                    {
                        $value = [string]::Format("{0:$tokenFormat}", $value)
                    }
                    
                    if($value -is [Int32])
                    {
                        $tokenFormat = $tokenFormat.Replace($tokenFormat[0], "0")
                        $value = [string]::Format("{0:$tokenFormat}", $value)
                    }
                }

                [void]$sb.Append($value)
                continue;
            }

            continue 
        }

        [void]$sb.Append($c);
    }

    $ENV:NM_BUILD_NUMBER = $sb.ToString().Trim()
    [void]$sb.Clear()
    return $ENV:NM_BUILD_NUMBER
}