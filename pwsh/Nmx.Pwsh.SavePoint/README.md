# Nmx.Pwsh.SavePoint

A PowerShell module for tracking save points for procedural scripts such
as device configuration changes where a task should only be invoked if
it has not completed or certain conditions have not been met.

The save point file is stored as JSON. The default file is located in the
current user's home directory at `$HOME/.config/nmx/pwsh/save-points.json`

## Set the save state location

Its recommended that the `Set-SaveStateFileName` is executed at the beginning
of any workflow to isolate the saved state.

```powershell
# Force will ensure any directories are created if they
# do not exist
Set-SaveStateFileName "$HOME/.config/org/save-points.json" -Force
```

## Sync a save point

Syncing a save point will read the save state to see if the save point has
been reached. If a save point has not been reached, it will execute a supplied
task in the form of a `ScriptBlock` and save any data returned by the
`ScriptBlock`.

Otherwise, it will skip the save point.

```powershell
Sync-SavePoint "start" {
    Rename-Computer "awesome-robot"

    # the data below will be saved to the 'Start' save-point.
    return @{
        "computerName" = "awesome-robot"
        "updatedAt" = [DateTime]::UtcNow
    }
}
```

## Get data from a previous save-point

```powershell
Sync-SavePoint "restart1" {
    Param($State, $Name)
    Write-Debug "restarting computer $($State['Start']['computerName']) "
    Restart-Computer

    # null will be converted to [DateTime]::UtcNow
    return;
}
```

## Custom state test

There are times you want a save point to be ignored under certain conditions
such as expiring after a certain amount of time.  The `Sync-SavePoint` has a
`-Test` parameter that takes a `ScriptBlock` value and is expected to return
true if the save point is complete, or false when the task should be executed
again.

```powershell
$test = {
    Param($State, $Name)

    $last = $State[$Name]
    if($null -eq $last) {
        return $false; # not saved
    }
    $dt = $last['updatedAt']
    if($null -ne $dt -and $dt -is [DateTime])
    {
        $span = [DateTime]::UtcNow - $dt
        if($span.Days -gt 2)
        {
            return $false;
        }
    }

    return $true;
}

Sync-SavePoint -Name "restart1" -Test $test -Task {
    Param($State, $Name)
    Write-Debug "I will restart if save point is older than 2 days or never"
    Restart-Computer

    # null will be converted to [DateTime]::UtcNow
    return;
}
```

## License

Copyright 2020 Nerdy Mishka, Michael Herndon

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  [http://www.apache.org/licenses/LICENSE-2.0][license]

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

[license]: http://www.apache.org/licenses/LICENSE-2.0
