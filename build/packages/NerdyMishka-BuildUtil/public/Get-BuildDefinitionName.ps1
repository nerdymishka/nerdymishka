
function Get-BuildDefinitionName()
{
    Param(
        [String] $Location
    )

    if($ENV:BUILD_DEFINITIONNAME) 
    {
        return $ENV:BUILD_DEFINITIONNAME
    }

    if($ENV:NM_BUILD_DEFINITIONAME) 
    {
        return $ENV:NM_BUILD_DEFINITIONAME
    }

    $info = Get-GitProjectInfo
    $project = $info.Project 
    $ENV:NM_BUILD_DEFINITIONAME = "${project}_Local" 

    return $ENV:NM_BUILD_DEFINITIONAME
}