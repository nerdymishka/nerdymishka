function Invoke-VsTestCmd()
{
    $ErrorActionPreference = 'Stop'
    $vsTest = Get-VsTestPath
    if (!$vsTest)
    {
        $global:LASTEXITCODE = -1000
        return
    }

   
    & $vsTest $args
}