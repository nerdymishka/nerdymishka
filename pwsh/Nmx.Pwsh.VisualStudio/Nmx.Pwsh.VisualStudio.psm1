$scripts = Get-Item "$PSScriptRoot/private/*.ps1" -EA SilentlyContinue
if($scripts)
{
    foreach($script in $scripts)
    {
        . $script
    }
}

$functions = @()
$scripts = Get-Item "$PSScriptRoot/public/*.ps1"
foreach($script in $scripts)
{
    $functions += $script.Name.Replace(".ps1", "")
    . $script
}

if($VerbosePreference -eq "Continue")
{
    Write-Verbose ($functions | ConvertTo-Json)
}

Export-ModuleMember -Function $functions