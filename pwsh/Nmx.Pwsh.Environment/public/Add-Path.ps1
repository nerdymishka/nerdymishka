function Add-Path()
{
    <#
    .SYNOPSIS
        Appends a path to the enviroment path variable. By default appends to the
        Process $env:Path variable.
    .DESCRIPTION
        Appends a path to the enviroment path variable. By default appends to the
        Process $env:Path variable.

        For Linux & Mac, the colon character is used. For windows, the semi-colon
        character is used. 

        Setting an environment variable for MacOS for machine or user scope is
        not currently supported.
    .EXAMPLE
        PS C:\> Add-Path "C:/Program Files/dotnet/bin"
        Appends the above path to $Env:Path 
    .INPUTS
        [String] The path to add.
    .OUTPUTS
        [Void]
    #>
    [CmdletBinding()]
    param(
        # The additional path to append to the environment path variable
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] 
        $Path,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = $true)]
        [System.EnvironmentVariableTarget] 
        $Scope = "Process"
    )

    if ($Scope -eq "Process")
    {
        if ($IsWindows)
        {
            $Env:Path += ";$Path"
        }
        else 
        {
            $Env:Path += ":$Path"
        }

        return 
    }

    if ($IsLinux)
    {
        $content = $null 
        if ($Scope -eq [System.EnvironmentVariableTarget]::Machine)
        {
            $pf = "/etc/environment"
            if ($IsMacOS)
            {
                $pf = "/etc/launchd.conf"
            }
        }
        if ($Scope -eq [System.EnvironmentVariableTarget]::User)
        {
            # if zprofile is available, 
            if (Test-Path "$HOME./.zprofile")
            {
                $pf = "$HOME/.zprofile"
            }
            elseif (Test-Path "$HOME/.bash_profile")
            {
                $pf = "$HOME/.bash_profile"
            }
            else 
            {
                $pf = "$HOME/.profile"
            }
        }

        $content = Get-Content $pf
        $pathData = $null
        foreach ($line in $content)
        {
            $n = $line.Trim()
            if ($n.StartsWith("PATH=") -or $line.StartsWith("export PATH="))
            {
                $pathData = $line.Substring(0, $line.IndexOf("="))
            }
        }

        if ($pathData)
        {
            $Path = "${pathData}:${PATH}"
        }

        Set-EnvironmentVar -Name "PATH" -Value $Path -Scope $Scope
    }
}