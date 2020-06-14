
function Get-LocalBuildDatabase()
{
    Param(
        [Alias("Path")]
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $InputObject 
    )

    if([string]::IsNullOrWhiteSpace($InputObject))
    {
        if($ENV:NM_BUILD_DB)
        {
            $InputObject = $ENV:NM_BUILD_DB 
        } else {
            $InputObject = "$HOME/.nerdymishka/build/build.db"
        }
    }

    if(!(Test-Path $InputObject))
    {
        $dir = $InputObject | Split-Path -Parent
        if(!(Test-Path $dir))
        {
            New-Item $dir -ItemType Directory | Write-Debug
        }

        $content = "{}" | ConvertTo-Json 
        [IO.File]::WriteAllText($InputObject, $content)
    }

    return (Get-Content $InputObject -Raw) | ConvertFrom-Json 
}