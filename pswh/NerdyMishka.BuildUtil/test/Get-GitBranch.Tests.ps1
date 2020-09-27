Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "Get-GitBranch" {
        $branches = git branch 
        $set = @()
        foreach($line in $branches)
        {
            if([string]::IsNullOrWhiteSpace($line))
            {
                continue;
            }
            $next = $line;
            $isCurrent = $false;
            if($next.StartsWith("* "))
            {
                $next = $next.Substring(2);
                $isCurrent =$true 
            }

            $set +=[PSCustomObject]@{
                IsCurrent = $isCurrent
                Name = $next 
            }
        }

        IT "should get the current branch" {
            $current = $set | Where-Object { $_.IsCurrent -eq $true }
            $name = Get-GitBranch -CurrentOnly 
            $name | Should Be $current.Name 
        }

        IT "should get all the branches"  {
            $b = Get-GitBranch
            $b.GetType().Name | Should Be "Object[]" 
            $master = $b | Where-Object { $_.Name -eq "master"}
            $master.Name | Should Be "master" 
            $master.IsCurrent | Should Be $True
            $set.Length -eq $b.Length | Should Be $True 
        }
    }
}