function Read-ModuleVar()
{
    $vars = Get-ModuleVar
    $dataDir = $vars["DataDirectory"]
    $stateFile = "$dataDir/vars.json"
    if ($dataDir -and $moduleName -and (Test-Path $stateFile))
    {
        $json = Get-Content "$stateFile" -Raw | ConvertFrom-Json
        $set = ConvertTo-Hashtable $json 
        foreach ($key in $set.Keys)
        {
            $vars[$key] = $set[$key]
        }

        $vars["DataDirectory"] = $dataDir
        $vars["ModuleName"] = $moduleName
    }
}