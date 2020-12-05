Import-Module "$PSScriptRoot/../Nmx.Pwsh.SavePoint.psd1" -Force

Describe 'Nmx.Pwsh.SavePoint' {
    $local = "$PSScriptRoot/.config/save-points.json".Replace("\", "/")
    It "Should Load the expected functions" {
        $functions = @(
            'Get-SaveStateFileName',
            'Read-SaveState',
            'Set-SaveStateFileName',
            'Sync-SavePoint',
            'Write-SaveState'
        )

        $mod = Get-Module "Nmx.Pwsh.SavePoint"
        $cmds = $mod.ExportedCommands.Keys
        foreach ($f in $functions)
        {
            $find = $cmds | Where-Object { $_ -eq $F }
            Write-Debug "Function $f Must Exist"
            $find | Should Not Be $null 
        }
    }

    It "Should have a default filename " {

        $default = "$HOME/.config/nmx/pwsh/save-points.json".Replace("\", "/")
        $current = Get-SaveStateFileName 
        $current | Should Be $default
    }

    It "Should set a default filename" {
        $default = "$HOME/.config/nmx/pwsh/save-points.json".Replace("\", "/")
       
        Set-SaveStateFileName $local -Force 
        $current = Get-SaveStateFileName 
        $current | Should Be $local
    }

    It "Should retain set filename between calls" {
        $current = Get-SaveStateFileName 
        $current | Should Be $local
    }

    It "Should read state as a hashtable" {
        $data = Read-SaveState 
        $data | Should not be $null 
        $data -is [hashtable] | Should Be $true 
    }

    It "Should write data with Write-SaveState" {
        Write-SaveState "1" -Data $true 
        $state = Read-SaveState 
        $state | Should Not be $null 
        $state['1'] -is [Boolean] | should be $true 
        $state['1'] | should be $true 
    }

    It "Should run a task if a save-point does not exist" {
        $g = $false 
        Sync-SavePoint '1' {
            $g = $true 
        }
        $g | Should Be $false 

        Sync-SavePoint '2' {
            $g = $true

            return $null
        }

        $g | Should Be $true 
    }

    It "Should get tasks state from a previous task" {
        Sync-SavePoint '3' {
            return @{
                'n'         = 'test'
                'updatedAt' = [DateTime]::UtcNow
            }
        }
       
        $g = $false 
        Sync-SavePoint '4' {
            Param($State, $Name)

            $Name | Should Be '4'
            $State['3']['n'] | Should Be 'test'
            $g = $true 
            return $null
        }

        $g | Should Be $true 
    }
   

    try 
    {
        if (Test-Path $local)
        {
            $dir = $local | Split-Path -Parent
            Remove-Item $dir -Force -Recurse
        }
    }
    finally
    {

    }
}