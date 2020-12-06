

function Set-EnvironmentVar()
{
    param(
        # Parameter 
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true)]
        [String]
        $Name,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = $true)]
        [String]
        $Value,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [System.EnvironmentVariableTarget] $Scope = "Process"
    )

    # TODO: support MAC with launchd
    if ($IsMacOS -and $Scope -ne "Process")
    {
        throw [System.NotSupportedException] "Setting MacOs environment variables for $Scope is not currently supported"
    }
  
    if (($IsLinux) -and $Scope -ne "Process")
    {
        $pf = $null 
        $content = $null 
        $useSudo = $false;
        if ($Scope -eq [System.EnvironmentVariableTarget]::Machine)
        {
            $pf = "/etc/environment"
            $useSudo = $true 
        }
        if ($Scope -eq [System.EnvironmentVariableTarget]::User)
        {
            # if zprofile /zsh is installed, its more likely preferred
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

        if (!(Test-Path $pf))
        {
            Write-Warning "Could not locate $pf"
            return 
        }

        $content = Get-Content $pf 

        if ($content)
        {
            $replaced = $false;
            $lines = @();
            foreach ($line in $content)
            {
                if ($line.Length -eq 0 -or ($line[0] -eq "#" -or $line[0] -eq ' '))
                {
                    $lines += $line
                    continue;
                }

                if ($line.Contains("="))
                {
                    $varName = $line.Substring(0, $line.IndexOf("="))
                    if ($varName.StartsWith("export "))
                    {
                        $varName = $varName.Split(' ')
                        $varName = $varName[1]
                    }
                    if ($varName.Trim() -ne $Name)
                    {
                        $lines += $line 
                        continue;
                    }

                    $next = "$Name=$Value"
                    $lines += $next 
                    $replaced = $true 
                    continue;
                }

                $lines += $line 
            }

            $output = "$Name=$Value"

            # if the value wasn't replaced, use the less risker op and only append
            # to the given file.
            if (!$replaced)
            {
                Write-Debug "Append to $pf only"
                if ($useSudo)
                {
                    echo "$name=$value" | sudo tee -a $pf 
                }
                
                else 
                {
                    # this is tee in linux, not Tee-Object in powershell
                    echo "$name=$value" | tee -a $pf 
                }
            }
            else 
            {
                Write-Debug "Replace content for $pf"
                $output = [String]::Join([Environment]::NewLine, $content)
                if ($useSudo)
                {
                    echo $output | sudo tee $pf 
                }
                else 
                {
                    echo $output | tee $pf 
                }
            }
        }

        # ensure the value is available in the process
        Set-item "Env:$Name" -Value $Value 
        return 
    }

    [Environment]::SetEnvironmentVariable($Name, $Value, $Scope);
}