
function Get-BuildNumber()
{
    
    if($ENV:NM_BUILD_NUMBER)
    {
        return $ENV:NM_BUILD_NUMBER
    }

    $ENV:NM_BUILD_NUMBER = New-BuildNumber 
    return $Env:NM_BUILD_NUMBER
}