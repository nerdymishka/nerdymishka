function Invoke-DynamicDockerCompose()
{
   
    $nextDirectory = $false;
    $nextEnvironment = $false 
    $append = @()
    
    $base = '/dkr'
    if ($IsWindows)
    {
        $base = $env:ALLUSERsPROFILE.Replace("\", "/") + "/dkr"
    }

    Write-Host "base $base"

    if (!(Test-Path $base))
    {
        
        if (!$IsWindows)
        {
            sudo mkdir $base 
            # TODO: work on permissions
            sudo chown root:docker $base
            sudo chmod 755 $base 
        }
        else
        {
            mkdir $base | Write-Debug
        }
    }

    foreach ($arg in $args)
    {
        if ($arg -eq "--dir")
        {
            $nextDirectory = $true 
            continue;
        }

        if ($arg -eq "--environment")
        {
            $nextEnvironment = $true 
            continue;
        }

        if ($nextDirectory)
        {
            $d = $arg;
            $nextDirectory = $false;
            continue;
        }

        if ($nextEnvironment)
        {
            $OperationalEnvironment = $arg;
            $nextEnvironment = $false;
            continue;
        }

        $append += $arg;
    }

    $d = $d.Replace("\", "/")
    if ($d.StartsWith("./"))
    {
        $d = (Resolve-Path $d).Path.Replace("\", "/")
    }

    $d = $d.TrimEnd("/")

    Write-Host $d

    $attempts = @(
        "$d/config.$OperationalEnvironment.json",
        "$d/config.user.json",
        "$d/config.json"
    )

    $config = $null
    $envFile = "$d/.env"
    foreach ($file in $attempts)
    {
        if (Test-Path $file)
        {
            $config = Get-Content $file -Raw | ConvertFrom-Json 
            break;

            $fileName = Split-Path $file -Leaf

            $fileName.Replace("config", "").Replace(".json", "")
            if ($fileName.Length -gt 0)
            {
                $envFile = "$d/$fileName.env"
            }
        }
    }

    if ($config.directories)
    {
        foreach ($dir in $config.directories)
        {
            $next = "$base/$dir"
            if (!(Test-Path $next))
            {
                if ($IsWindows)
                {
                    New-Item $next -ItemType Directory | Write-Debug
                }
                else 
                {
                    sudo mkdir $next 
                    sudo chown root:docker $next 
                    sudo chmod 755 $next 
                }
               
            }
        }
    }

    $useEnv = $false;
    if (!(Test-Path $envFile))
    {
        if ($config.env)
        {
            $content = "NMX_DKR=$base`n";
            $config.env | Get-Member -MemberType NoteProperty | ForEach-Object {
                $key = $_.Name
                $value = $config.env.$key 
                if ($value -eq "::password")
                {
                    $value = New-GzPassword -AsString 
                }
                if ($value.StartsWith("::file:"))
                {
                    $file = $value.Substring(7)
                    if (Test-Path $file)
                    {
                        $value = Get-Content $file -Raw
                        $content += "$key=$value`n"
                    }
                    return 
                }
                if ($value -is [String])
                {
                    $content += "$key=$value`n"
                    return 
                }
                if ($value -is [Array])
                {
                    $content += "$key=`""
                    $value = [string]::Join("`n", $value)
                    $content += "$value`"`n"
                    return 
                }
                $content += "$key=$value`n"
            }

            $content | Out-File $envFile -Encoding utf8NoBOM
            $useEnv = $true 
        }
    }
    else
    {
        $useEnv = $true;
    }

    $cmp = "docker-compose.yml"
    if ($config.compose)
    {
        $cmp = $config.compose.$OperationalEnvironment
        if (!$cmp)
        {
            $cmp = $config.compose.user
        }
        if (!$cmp)
        {
            $cmp = $config.compose.default
        }
        if (!$cmp)
        {
            $cmp = "docker-compose.yml"
        }
    }
    $cmp = "$d/$cmp"

    $splat = @(
        '-f',
        "`"$cmp`""
    );

    if ($useEnv)
    {
        $splat += "--env-file"
        $splat += "`"$envFile`""
    }

    foreach ($value in $append)
    {
        $splat += $value 
    }

    if ($IsWindows)
    {
        docker-compose @splat 
    }
    else 
    {
        sudo docker-compose @splat
    }
}