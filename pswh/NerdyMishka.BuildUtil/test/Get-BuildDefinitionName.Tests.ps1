Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "Get-BuildDefinitionName" {
        
        IT "should use Env:NM_BUILD_DEFINITIONNAME" {
            $ENV:NM_BUILD_DEFINITIONNAME = $null;
            $ENV:NM_BUILD_DEFINITIONNAME | Should Be $null
            $ENV:NM_BUILD_DEFINITIONNAME = "MY_BUILD"
            
            $v = Get-BuildDefinitionName
            $v | Should Be "MY_BUILD"
        }

        IT "should use Env:BUILD_DEFINITIONNAME" {
            $ENV:NM_BUILD_DEFINITIONNAME = $null;
            $ENV:NM_BUILD_DEFINITIONNAME | Should Be $null
            $name = $ENV:BUILD_DEFINITIONNAME
            $clear = $false;
            if(!$ENV:BUILD_DEFINITIONNAME)
            {
                $ENV:BUILD_DEFINITIONNAME = "MY_BUILD_2"
                $name = $ENV:BUILD_DEFINITIONNAME
                $clear = $true;
            } 
            try 
            {
                $v = Get-BuildDefinitionName
                $v | Should Be $name
            } finally {
                if($clear)
                {
                    $ENV:BUILD_DEFINITIONNAME = $null;
                }
            }
        }

        IT "should use current project name when environment vars are not found" {
            $old = $ENV:BUILD_DEFINITIONNAME
            $ENV:NM_BUILD_DEFINITIONNAME = $null  
            if($old)
            {
                $ENV:BUILD_DEFINITIONNAME = $null
            }

            try 
            {
                $v = Get-BuildDefinitionName
                $v | Should Be "nerdymishka_Local"
            } finally 
            {
                if($old)
                {
                    $ENV:BUILD_DEFINITIONNAME = $old 
                }
            }
        }
    }
}