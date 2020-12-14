
Write-host "loadding"
$functions = @()
$files = Get-Item "$PSScriptRoot/public/*.ps1"
foreach ($file in $files)
{
    $functions += $file.Name.Replace(".ps1", "")
    Write-Host ($file.Name.ReplacE(".ps1", ""))
    . "$($file.FullName)"
}

Export-ModuleMember -Function $functions