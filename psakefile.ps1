
Import-Module "$PSScriptRoot/build/packages/NerdyMishka-BuildUtil" -Force


Task "dotenv" {
    Read-DotEnv 
}

Task "nuget:restore" {
    dotnet restore 
}

Task "test:unit" -depends "nuget:restore", "build" {
    $c = $ENV:MSBUILD_CONFIGURATION
    $artifactsDir = Get-BuildArtifactsDirectory 
    $testDir = "$artifactsDir/test-results"
    
    dotnet test -c $c --filter tag=unit -r "$testDir"
}

Task "build" -depends  "dotenv", "nuget:restore" 

Task "build:projects" {
    $c = $ENV:MSBUILD_CONFIGURATION
    dotnet build -c $c --no-restore
}