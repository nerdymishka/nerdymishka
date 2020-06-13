
function Get-LocalBuildDatabase()
{
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $InputObject 
    )

    if([string]::IsNullOrWhiteSpace($InputObject))
    {
        $InputObject = "$HOME/.nerdymishka/build/build.db"
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