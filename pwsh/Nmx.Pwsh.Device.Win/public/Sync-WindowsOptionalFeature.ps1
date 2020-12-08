
function Sync-WindowsOptionalFeature()
{
    [CmdletBinding(SupportsShouldProcess)]
    param(
        [Parameter(Position = 0)]
        [WindowsFeatureSection[]] 
        $Section
    )

    $ErrorActionPreference = "Stop"
    process 
    {
        foreach ($item in $Section)
        {
            $s = [WindowsFeatureSection]$item 

            $feature = Get-WindowsOptionalFeature -Online -FeatureName $s.name -EA SilentlyContinue
            if ($null -eq $feature)
            {
                $result = [WindowsFeatureResult]::new()
                $result.name = $s
                $result.status = [WindowsFeatureStatus]::NotAvailable

                Write-Output $result 
                continue;
            }

            $record = [WindowsFeatureResult]::new()
            $record.name = $s
            $record.status = [WindowsFeatureStatus]::NoOperation
            $record.enabled = $feature.State -eq [Microsoft.Dism.Commands.FeatureState]::Enabled
       
            if ($s.remove -and $feature.State -eq [Microsoft.Dism.Commands.FeatureState]::Enabled)
            {
                if ($PSCmdlet.ShouldProcess($s.Name, "Disable-WindowsOptionalFeature"))
                {
                    $result = Disable-WindowsOptionalFeature -Online -FeatureName $s.name -NoRestart
                    $record.rebootRequired = $result.RestartNeeded
                }

                $record.status = [WindowsFeatureStatus]::Disabled
                $record.enabled = $false;
            }

            if (!$s.remove -and $feature.State -eq [Microsoft.Dism.Commands.FeatureState]::Disabled)
            {
                if ($PSCmdlet.ShouldProcess($s.Name, "Enable-WindowsOptionalFeature"))
                {
                    $result = Disable-WindowsOptionalFeature -Online -FeatureName $s.name -NoRestart
                    $record.rebootRequired = $result.RestartNeeded
                }
                $record.status = [WindowsFeatureStatus]::Enabled
                $record.enabled = $true;
            }

            Write-Output $record
        }
    }
}