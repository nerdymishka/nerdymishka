Import-Module "$PSScriptRoot/../../Nmx.Pwsh.Environment/Nmx.Pwsh.Environment.psd1" -Force
Import-Module "$PSScriptRoot/../Nmx.Pwsh.VisualStudio.psd1" -Force



Describe "Nmx.Pwsh.VisualStudio" {

    if ($IsWindows)
    {
        It "Should find vswhere" {
            $vsWhere = Get-VsWherePath 
            $vsWhere | Should Not Be $null
            (Test-Path $vsWhere) | Should Be $True
        }

        It "Should find msbuild" {
            $msbuild = Get-MsBuildPath
            $msbuild | Should Not Be $null
            (Test-Path $msbuild) | Should Be $True
        }

    
        It "Should find vstest" {
            $vstest = Get-VsTestPath
            $vstest | Should Not Be $null
            (Test-Path $vstest) | Should Be $True
        }

        It "Should find vsdevcmd" {
            $devcmd = Get-VsDevCmdPath
            $devcmd | Should Not Be $null
            (Test-Path $devcmd) | Should Be $True
        }

        It "Should find latest wix tool set path" {
            $wix = Get-WixToolSetPath 
            $wix | Should Not Be $null
            (test-Path $wix) | Should Be $true 
        }

        It "Should invoke-msbuildcmd with args" {
            $data = Invoke-MsBuildCmd -ver 
            $data | Should Not Be $null
            $data[0] -match "Microsoft" | Should Be $true
        }

        It "Should invoke-wixcandle with args" {
            $data = Invoke-WixCandleCmd -v 
            $data | Should Not Be $null
            $LASTEXITCODE | Should not be -1000
           
        }
        
    }
}