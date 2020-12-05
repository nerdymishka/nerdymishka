function Get-SaveStateFileName()
{
    $vars = Get-ModuleVar 
    if (!$vars["stateFile"])
    {
        $default = "$HOME/.config/nmx/pwsh/save-points.json"
        Set-SaveStateFileName $default -Force
        $vars = Get-ModuleVar 
    }

    return $vars["stateFile"];
}