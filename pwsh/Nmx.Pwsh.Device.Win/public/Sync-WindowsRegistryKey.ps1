
function Sync-WindowsRegistryKey()
{
    [CmdletBinding()]
    param (
        [Parameter()]
        [WindowsRegistryKeySection[]]
        $Section
    )
    $ErrorActionPreference = "Stop"

    foreach ($s in $Section)
    {
        $key = Get-Item ($s.Key) -EA SilentlyContinue
        $hasSubKeys = $null -ne $s.SubKeys -and $s.SubKeys.Length -gt 0
        $hasValue = ![String]::IsNullOrWhiteSpace($s.Name)
        $record = [WindowsRegistryKeyOutput]::new()
        $record.Key = $s.Key 

        if ($hasSubKeys -and $hasValue)
        {
            $msg = "[WindowsRegistrySection]: $($s.Key) " 
            $msg += "When a section has subkeys and a value, the subkeys will take precendence"  
            Write-Debug $msg 
        }


        if (!$hasValue -and !$hasSubKeys)
        {   
            if ($key -and $s.remove)
            {
                Remove-Item $key -Recurse
                $record.Status = [WindowsRegistryKeyStatus]::Removed

                Write-Output $record
                continue;
            }

            if (!$key -and !$s.remove)
            {
                New-Item -Path $key 
                $record.Status = [WindowsRegistryKeyStatus]::Added
                Write-Output $record
                continue;
            }

            Write-Output $record 
            continue;
        }

        if (!$key -and !$s.Remove)
        {
            $key = New-Item -Path $($s.Key)
            $record.Status = [WindowsRegistryKeyStatus]::Added
        }

        if ($hasSubKeys)
        {
            $subKeys = $s.SubKeys 
            foreach ($sk in $subKeys)
            {
                $sk.Key = $s.Key 
            }

            $results = Sync-WindowsRegistryKey -Section $subKeys
            $synced = $false;
            foreach ($r in $results)
            {
                if ($r.status -ne [WindowsRegistryKeyStatus]::NoOperation)
                {
                    $synced = $true;
                    break
                }
            }
            $record.SubKeys = $results
            if ($synced)
            {
                $record.Status = [WindowsRegistryKeyStatus]::Synced
            }
        }
        elseif ($hasValue)
        {
            $uri = "$($s.Key)/$($s.Name)"
            $subKey = Get-ItemProperty -Path $uri  -EA SilentlyContinue
            $record.Name = $s.Name 
            if ($subKey)
            {
                if ($s.remove)
                {
                    Remove-ItemProperty -Path $uri -Force | Write-Debug
                    $record.Status = [WindowsRegistryKeyStatus]::Removed
                }
                else 
                {
                    $old = Get-ItemPropertyValue -Path $uri 
                    if ($old -ne $s.Value)
                    {
                        Set-ItemProperty "$($s.Key)/$($s.Name)" -Value $s.Value 
                        $record.Status = [WindowsRegistryKeyStatus]::Synced
                    }
                }
            }
            else 
            {
                if ($null -eq $s.Type -or $s.Type -eq "None")
                {
                    $key | New-ItemProperty -Name $s.Name -Value $s.Value | Write-Debug
                    $record.Status = [WindowsRegistryKeyStatus]::Added
                }
                else 
                {
                    $key | New-ItemProperty -Name $s.Name -Value $s.Value `
                        -PropertyType $s.Type | Write-Debug 
                }

                $record.Status = [WindowsRegistryKeyStatus]::Added
            }           
        }

        Write-Output $record 
    }
}