Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "Read-LocalBuildDatabase" {
        
        IT "should use NM_BUILD_DB" {
            $ENV:NM_BUILD_DB = $null;
            $ENV:NM_BUILD_DB | Should Be $null
            $ENV:NM_BUILD_DB = "$PSScriptRoot/build1.db"
            
            try 
            {
                $v = Read-LocalBuildDatabase -Query "nerdymishka_Local/buildId"
                $v | Should Not Be $Null 
                $v | Should Be 0  
            } finally {
                $ENV:NM_BUILD_DB = $null 
            }            
        }

        IT "should use DatabasePath" {
            $ENV:NM_BUILD_DB = $null;
            $ENV:NM_BUILD_DB | Should Be $null
            
            $v = Read-LocalBuildDatabase -Query "nerdymishka_Local/project" -DatabasePath "$PSScriptRoot/build1.db"
            $v | Should Not Be $Null 
            $v | Should Be "nerdymishka"
        }
    }
}