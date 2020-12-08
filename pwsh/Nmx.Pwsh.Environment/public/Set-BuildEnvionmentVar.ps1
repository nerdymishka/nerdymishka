function Set-BuildEnvironmentVar()
{
    [CmdletBinding()]
    param(        
        [Parameter(Position = 0)]
        [String] 
        $Name,
        
        [Parameter(Position = 1)]
        [String]
        $Value,

        [Parameter(ParameterSetName = "_Set")]
        [Hashtable]
        $Set
    )

    process 
    {
        if ($Set -and $Set.Count -gt 0)
        {
            foreach ($key in $Set.Keys)
            {
                Set-BuildEnvironmentVar -Name $key -Value $Set[$key]
            }

            return 
        }

        if ($env:GITHUB_ENV)
        {
            if ($DebugPreference -eq "Continue")
            {
                Write-Debug "Setting $Name for github actions"
            }

            "$Name=$Value" | Out-File -FilePath $Env:GITHUB_ENV -Encoding utf8 -Append
        }

        if ($env:TF_BUILD)
        {
            if ($DebugPreference -eq "Continue")
            {
                Write-Debug "Setting $Name for azure devops"
            }

            Write-Host "##vso[task.setvariable variable=$Name]$Value"
        }

        [Environment]::SetEnvironmentVariable($Name, $Value, [System.EnvironmentVariableTarget]::Process)
    }
}