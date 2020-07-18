
function Get-BuildDefinitionName()
{
    Param(
        [String] $Location
    )

    if($ENV:NM_BUILD_DEFINITIONNAME) 
    {
        return $ENV:NM_BUILD_DEFINITIONNAME
    }

    if($ENV:BUILD_DEFINITIONNAME) 
    {
        $ENV:NM_BUILD_DEFINITIONNAME = $ENV:BUILD_DEFINITIONNAME
        return $ENV:BUILD_DEFINITIONNAME
    }

    $info = Get-GitProjectInfo
    $project = $info.Project 
    $ENV:NM_BUILD_DEFINITIONAME = "${project}_Local" 

    return $ENV:NM_BUILD_DEFINITIONAME
}