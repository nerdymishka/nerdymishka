
Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "Write-LocalBuildDatabase" {
        
        IT "should use NM_BUILD_DB" {
            $ENV:NM_BUILD_DB = $null;
            $ENV:NM_BUILD_DB | Should Be $null
            $ENV:NM_BUILD_DB = "$PSScriptRoot/build2.db"
            
            try 
            {
                $obj = @{ "one"= "value "}
                $obj |  Write-LocalBuildDatabase
                Test-Path "$PSScriptRoot/build2.db" | Should Be $true 
            } finally {
                $ENV:NM_BUILD_DB = $null 
                Remove-Item "$PSScriptRoot/build2.db"
            }            
        }

        IT "should use -Destination" {
            $ENV:NM_BUILD_DB = $null;
            $ENV:NM_BUILD_DB | Should Be $nul
            Test-Path "$PSScriptRoot/build3.db" | Should Be $false  
            try 
            {
                $obj = @{ "one"= "value "}
                $obj |  Write-LocalBuildDatabase -Destination "$PSScriptRoot/build3.db"
                Test-Path "$PSScriptRoot/build3.db" | Should Be $true 
            } finally {
                $ENV:NM_BUILD_DB = $null 
                Remove-Item "$PSScriptRoot/build3.db"
            }            
        }
    }
}