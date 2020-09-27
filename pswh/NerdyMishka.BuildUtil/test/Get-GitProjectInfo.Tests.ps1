Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "Get-GitProjectInfo" {
       
        if(Test-Path "$PSScriptRoot/.git")
        {
            Remove-Item "$PsScriptRoot/.git" -Force -Recurse
        }

        New-Item "$PSScriptRoot/.git" -ItemType Directory 
        Copy-Item "$PSScriptRoot/config" "$PSScriptRoot/.git"

        IT "should get the project info for 'origin'" {

            $info = Get-GitProjectInfo -Location $PSScriptRoot
            $info | Should Not Be $Null
            $info.Project | Should Be "repo"
            $info.RepositoryName | Should be  "repo" 
            $info.Organization | Should be "org"
            $info.RepositoryUri | Should be "https://gitlab.com/org/repo"
        }

        It "should get the project info for 'vsts'" {

            $info = Get-GitProjectInfo -Location $PSScriptRoot -Remote "vsts"
            $info | Should Not Be $Null
            $info.Project | Should Be "proj"
            $info.RepositoryName | Should be  "repo" 
            $info.Organization | Should be "org"
            $info.RepositoryUri | Should be "https://nerdymishka@dev.azure.com/org/proj/_git/repo"
        }

        
        if(Test-Path "$PSScriptRoot/.git")
        {
            Remove-Item "$PsScriptRoot/.git" -Force -Recurse
        }
    }
}