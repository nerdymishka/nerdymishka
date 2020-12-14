$ErrorActionPreference = "Stop"
Import-Module "$PSScriptRoot/../scripts/Nmx.Docker/Nmx.Docker.psm1" -Force

Invoke-DynamicDockerCompose --dir $PsScriptRoot --environment $env:NMX_ENV up -d 