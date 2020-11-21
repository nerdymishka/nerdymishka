

#Requires -Module Psake


Task "ello" {
    write-host "ello"
}

Task "nuget:fetch" {
    $dir = "./packages"
    exec {
        nuget install nuget.versioning -o $dir 
        nuget install microsoft.extensions.configuration.json -o $dir 
        nuget install system.text.encoding.codepages -o $dir 
        nuget install system.drawing.common -o $dir 
        nuget install epplus -version "4.5.3.3" -o $dir
        nuget install yamldotnet -o $dir 
        nuget install serilog.sinks.applicationinsights -o $dir 
        nuget install serilog.sinks.console -o $dir 
        nuget install serilog.sinks.file -o $dir 
        nuget install serilog.enrichers.environment -o $dir 
        nuget install serilog.settings.configuration -o $dir 
    }
}

Task "nuget:copy" {
    $dir = "./packages"
    $platformSetup = "./Nmx.PowerShell.PlatformSetup"
    $dirs = @(
        "$platformSetup/bin/net5",
        "$platformSetup/bin/net45"
    )

    foreach ($d in $dirs)
    {
        if (!(Test-Path $d)) { mkdir $d | Write-Debug }
    }

    Copy-Item "$dir/NuGet.Versioning*/lib/netstandard2.0/*.*" "$platformSetup/bin/net5" -Force 
    Copy-Item "$dir/NuGet.Versioning*/lib/net45/*.*" "$platformSetup/bin/net45" -Force 
}

Task "default" -depends "ello"