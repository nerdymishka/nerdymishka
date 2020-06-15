Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "New-BuildNumber" {
        $dbPath = "$PSScriptRoot/build99.db"

        if(Test-Path $dbPath)
        {
            Remove-Item $dbPath
        }

        IT "should generate a new build number and create build99.db file" {
            $ENV:NM_BUILD_NUMBER = $null
            $value = New-BuildNumber -DatabasePath $dbPath 
            # this will fail if the new-buildnumber is created whne UTC switches days
            $dt = [string]::Format("{0:yyyyMMdd}", [DateTime]::UtcNow)

            $value | Should Be "nerdymishka_Local_$dt.01"
            test-Path $dbPath | Should Be $true 
        }

        IT "should generate a new revision number" {
            $ENV:NM_BUILD_NUMBER = $null
            $value = New-BuildNumber -DatabasePath $dbPath 
            # this will fail if the new-buildnumber is created whne UTC switches days
            $dt = [string]::Format("{0:yyyyMMdd}", [DateTime]::UtcNow)

            $value | Should Be "nerdymishka_Local_$dt.02"
        }

       

       
    }
}