function Set-SaveStateFileName()
{
    [CmdletBinding()]
    param (
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0)]
        [String]
        $Path,

        [Switch]
        $Force
    )

    $vars = Get-ModuleVar 

    if (!$Path.EndsWith(".json"))
    {
        throw "Checkpoint state file must be a json file"
    }

    $dir = $Path | Split-Path -Parent
    if (!(Test-Path $dir))
    {
        if (!$Force)
        {
            throw [IO.DirectoryNotFoundException] $dir 
        }

        New-Item $dir -ItemType Directory 
    }

    if (!(Test-Path $Path))
    {
        [IO.File]::WriteAllText($Path, "{}");
    }

    $vars["stateFile"] = $Path.Replace("\", "/")
}