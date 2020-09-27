Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "Get-LocalBuildDatabase" {
        
        IT "should use NM_BUILD_DB" {
            $ENV:NM_BUILD_DB = $null;
            $ENV:NM_BUILD_DB | Should Be $null
            $ENV:NM_BUILD_DB = "$PSScriptRoot/build1.db"
            
            try 
            {
                $db = Get-LocalBuildDatabase
                $db | Should Not BE $Null 
                $db.nerdymishka_Local | Should Not Be $Null 
            } finally {
                $ENV:NM_BUILD_DB = $null 
            }            
        }

        IT "should use -InputObject" {
            $ENV:NM_BUILD_DB = $null;
            $ENV:NM_BUILD_DB | Should Be $null
            
            $db = Get-LocalBuildDatabase -Path "$PSScriptRoot/build1.db"
            $db | Should Not BE $Null 
            $db.nerdymishka_Local | Should Not Be $Null 
        }
    }
}