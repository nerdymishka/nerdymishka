function Clear-ModuleVar()
{
    $vars = Get-ModuleVar 
    $vars.Clear();
    
    # reset data
    $modName = $vars["ModuleName"] = "NerdyMishka.Ado"
    $vars["DataDirectory"] = "$HOME/.nerdymishka/pwsh/$($modName.ToLowerInvariant())".Replace("\", "/")
}