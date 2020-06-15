
function Get-BuildArtifactsDirectory()
{
    Param()

    if($ENV:NM_BUILD_ARTIFACTS_DIR) {
        return $ENV:NM_BUILD_ARTIFACTS_DIR;
    }

    if($ENV:BUILD_STAGINGDIRECTORY) {
        Set-BuildVariable -Name "NM_BUILD_ARTIFACTS_DIR" -Value $ENV:BUILD_STAGINGDIRECTORY
        return $ENV:BUILD_STAGINGDIRECTORY
    }

    $root = Find-GitProjectRoot 
    if([string]::IsNullOrWhiteSpace($root)) {
        throw "Could not locate the root folder"
    }
    $dir = "$root/artifacts"
    Set-BuildVariable -Name "NM_BUILD_ARTIFACTS_DIR" -Value $dir   

    return $dir 
}