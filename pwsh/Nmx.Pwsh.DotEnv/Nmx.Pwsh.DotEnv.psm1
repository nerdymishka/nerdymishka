
if ($null -eq (Get-Command Set-BuildVariable -EA SilentlyContinue))
{
    function Set-BuildVariable()
    {
        Param(
            [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true, ValueFromPipeline = $true)]
            [Alias("Name")]
            [Object] $InputObject,

            [Parameter(Position = 1, ValueFromPipelineByPropertyName = $true)]
            [String] $Value 
        )

        if ($InputObject -is [string])
        {
            $Name = $InputObject
            if ($ENV:TF_BUILD)
            {
                Write-Host "##vso[task.setvariable variable=$Name]$Value"
            }

            if($env:GITHUB_ENV)
            {
                Write-Object "$InputObject=$Value"  | Out-File -FilePath $env:GITHUB_ENV -Encoding utf8 -Append
            }
    
            Set-Item -Path "Env:/$Name" -value $Value 
            return 
        }

        if ($InputObject -is [PsCustomObject])
        {
            $InputObject | Get-Member -MemberType NoteProperty | ForEach-Object {
                $name = $_.Name 
                $value = $InputObject[$Name]
                Set-BuildVariable -Name $name -value $value 
            }

            return 
        }

        if ($InputObject -is [Hashtable])
        {
            foreach ($key in $InputObject.Keys)
            {
                $value = $InputObject[$key]
                Set-BuildVariable -Name $key -Value $Value
            }

            return 
        }
    }
}


# TODO: replace with dotnet package https://gitlab.com/nerdymishka/nerdymishka/-/tree/master/DotEnv 
# The function below has many string allocations.
# the project above would cut some of that down.
function Read-DotEnv()
{
    [CmdletBinding()]
    param(
        [Alias("Path")]
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [String] $InputObject 
    )

    if ([string]::IsNullOrWhiteSpace($InputObject))
    {
        $InputObject = $PWD.Path;
        $InputObject += "/.env" 
        Write-Debug "Set InputObject to $InputObject"
    }

    $originalPath = $InputObject

    While (!(Test-Path $InputObject))
    {
        # pop .env
        $dir = (Split-Path $InputObject -Parent) 
        
        # pop current directory
        $dir = (Split-Path $dir -Parent)
        if ([string]::IsNullOrWhiteSpace($dir))
        {
            Write-Debug "Could not locate .env in the path or ancestor folders $($originalPath)"
            return
        }

        $InputObject += "$dir/.env"
    }

    $content = Get-Content $InputObject 
    $multiLine = $false
    $single = $false 
    $quote = $false 
    $json = $false 
    $name = $null 
    $sb = New-Object System.Text.StringBuilder 
    foreach ($line in $content) 
    {
        if ($multiLine)
        {
            if ($quote -and (($single -and $line.EndsWith("`'")) -or $line.EndsWith("`"")))
            {
                $multiLine = $false;
                $quote = $false 
                $n = $line.TrimEnd(); 
                if ($single) 
                {
                    $single = $false 
                    $n = $n.TrimEnd("'")
                }
                else
                {
                    $n = $n.TrimEnd("`"")
                }

                [void]$sb.Append($n);
                Set-BuildVariable -Name $Name -Value ($sb.ToString())
                [void]$sb.Clear()
                $Name = $Null 
                continue;
            }

            if ($json -and $line -eq "}")
            {
                $json = $false 
                $multiLine = $false;
                [void]$sb.Append($line);
                Set-BuildVariable -Name $Name -Value ($sb.ToSTring())
                [void]$sb.Clear()
                $Name = $Null 
                continue; 
            }

            [void]$sb.AppendLine($line);
            continue;
        }

        if ([string]::IsNullOrWhiteSpace($line) -or $line.StartsWith("#"))
        {
            continue
        }

        if ($line -match "=")
        {
            $index = $line.IndexOf("=");

            $set = @()
            $set += $line.Substring(0, $index)
            $set += $line.Substring($index + 1) 
            $name = $set[0].Trim();
     
            # only trim the start in case this is a multiline
            # value. 
            $v = $set[1].TrimStart();
            if ([string]::IsNullOrWhiteSpace($v))
            {
                # setting enviroment variables to an empty string is
                # transformed into null, so may as well do this
                # explicitly and continue early. 
                Set-BuildVariable -Name $name -Value $null
                continue;
            }
            $value = $set[1].Trim();
            if ($v[0] -eq "`"")
            {
                if ($v[$v.Length - 1] -eq "`"")
                {
                    $value = $v.Replace("\n", [Environment]::NewLine).Trim("`"");
                }
                else
                {
                    $multiLine = $true;
                    $quote = $true;
                    [void]$sb.AppendLine($v.TrimStart("`""));
                }
            }

            if ($v[0] -eq '''')
            {
                if ($v[$v.Length - 1] -eq '''')
                {
                    $value = $v.Trim("'");
                }
                else
                {
                    $multiLine = $true;
                    $quote = $true;
                    $single = $true
                    [void]$sb.AppendLine($v.TrimString("'")); 
                }
            }

            if ($v[0] -eq '{')
            {
                $multiLine = $true;
                $json = $true;
                [void]$sb.AppendLine($v)
            }

            if ($multiLine)
            {
                continue; 
            }
          
            Set-BuildVariable -Name $name -Value $value 
        }
    }
}

Export-ModuleMember -Function @('Read-DotEnv')