
$config = Get-Content -RAw "$PsScriptRoot" | ConvertFrom-Json 

Properties {

}

Task "nuget:restore" {
    dotnet restore 
}

Task "test:unit" {
    dotnet test -c $msbuild.configuration --filter tag=unit -r "$testDir"
}

Task "build" {
    dotnet build 
}