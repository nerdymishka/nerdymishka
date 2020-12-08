function Invoke-VsWhereCmd()
{
    $ErrorActionPreference = 'Stop'
    $vsWhere = Get-VsWherePath 
    if (!$vsWhere)
    {
        $global:LASTEXITCODE = -1000
        return;
    }

    & $vsWhere $args
}