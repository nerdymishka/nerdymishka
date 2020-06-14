
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

                $sb.Append($n);
                Set-BuildVariable -Name $Name -Value ($sb.ToString())
                $sb.Clear()
                $Name = $Null 
                continue;
            }

            if ($json -and $line -eq "}")
            {
                $json = $false 
                $multiLine = $false;
                $sb.Append($line);
                Set-BuildVariable -Name $Name -Value ($sb.ToSTring())
                $sb.Clear()
                $Name = $Null 
                continue; 
            }

            $sb.AppendLine($line);
            continue;
        }

        if ([string]::IsNullOrWhiteSpace($line) -or $line.StartsWith("#"))
        {
            continue
        }

        if ($line -match "=")
        {

            $set = $line.Split("=");
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
                    $sb.AppendLine($v.TrimStart("`""));
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
                    $sb.AppendLine($v.TrimString("'")); 
                }
            }

            if ($v[0] -eq '{')
            {
                $multiLine = $true;
                $json = $true;
                $sb.AppendLine($v)
            }

            if ($multiLine)
            {
                continue; 
            }
          
            Set-BuildVariable -Name $name -Value $value 
        }
    }
}