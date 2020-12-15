param(
    [Parameter(Position = 0)]
    [String]
    $From

    [Parameter(Position = 1)]
    [String]
    $To,

    [Alias("o")]
    [Parameter()]
    [String]
    $Destination,

    [Switch] $Idempotent
)


if(!(Get-Command dotnet-ef -EA SilentlyContinue))
{
    dotnet tool install --global dotnet-ef
}

if([string]::IsNullOrWhiteSpace($Destination)) { $Destination = "$PsScriptRoot/Scripts" }

$splat = @()
if(![string]::IsNullOrWhiteSpace($From)) { $splat += $From }
if(![string]::IsNullOrWhiteSpace($To)) { $splat += $To }
if($Idempotent) { $splat += "-i"}

$splat += "--output"
$splat += "`"$Destination`""

dotnet-ef migrations script @splat