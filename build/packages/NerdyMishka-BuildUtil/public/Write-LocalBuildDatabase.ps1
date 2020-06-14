function Write-LocalBuildDatabase()
{
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [Object] $InputObject,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [String] $Destination 
    )

    if([string]::IsNullOrWhiteSpace($Destination))
    {
        if($ENV:NM_BUILD_DB)
        {
            $Destination = $ENV:NM_BUILD_DB 
        } else {
            $Destination = "$HOME/.nerdymishka/build/build.db"
        }
    }

    if(!(Test-Path $Destination))
    {
        $dir = $Destination | Split-Path -Parent 
        if(!(Test-Path $dir))
        {
            New-Item $dir -ItemType Directory 
        }
    }

    $lock = "$Destination.lock"
    $count = 20
    $retries = 0;
    While(Test-Path $lock)
    {
        if($retries -gt $count)
        {
            throw "Write Failed. Retries exceeded $Retries. $Destination.lock still exists";
        }

        Start-Sleep -Seconds 5
        $retries++;
    }

    "" > "$Destination.lock"
    if(!($InputObject -is [String]))
    {
        $InputObject = $InputObject | ConvertTo-Json -Depth 15
    }
    [IO.File]::WriteAllText($Destination, $InputObject)
    Remove-Item $lock -Force
}