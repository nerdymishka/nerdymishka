
if($Null -eq (Get-Command dotnet -EA SilentlyContinue))
{
    $msg = "PowerShell Module NerdyMishka-Nuget requires " 
    $msg += "the dotnet command installed and available "
    $msg += "on the path";
    throw $msg 
} 
$functions = @()

<# 
Get-ChildItem "$PsScriptRoot\private\**\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}
#>

Get-ChildItem "$PsScriptRoot\public\**\*.ps1" | ForEach-Object {
    $functions += $_.Name.Substring(0, $_.Name.Length - 4)
    . "$($_.FullName)"
}

Export-ModuleMember -Function $functions