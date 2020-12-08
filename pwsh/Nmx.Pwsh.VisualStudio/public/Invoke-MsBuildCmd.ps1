function Invoke-MsBuildCmd
{
    $msBuild = Get-MsBuildPath 
    if (!$msBuild)
    {
        Write-Debug "Get-MsBuildPath return null"
        $global:LASTEXITCODE = -1000
        return 
    }

   
    if (!($msBuild -is [String]))
    {
        Write-Debug "Get-MsBuildPath path returned type $($msBuild.GetType().FullName)"
        $global:LASTEXITCODE = -1000
        return
    }
    
    & $msBuild $args 
    return 
}