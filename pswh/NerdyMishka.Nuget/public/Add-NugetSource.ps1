
function Add-NugetSource()
{
    [CmdletBinding(SupportsShouldProcess = $true)]
    Param(
        [Alias("n")]
        [Parameter(Position = 0, Mandatory = $true, ValueFromPipelineByPropertyName)]
        [String] $Name, 
        
        [Alias("s", "Source")]
        [Parameter(Position = 1, Mandatory = $true, ValueFromPipelineByPropertyName)]
        [String] $Uri,

        [Alias("u")]
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [String] $Username,

        [Alias("p")]
        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [securestring] $Password,

        [Switch] $DisablePasswordEncryption,

        [Parameter(ValueFromPipelineByPropertyName = $true)]
        [String] $ConfigFile
    )

    Process 
    {
        $pSet = @("$Uri", "--name", "$Name");

        

        if (![string]::IsNullOrWhiteSpace($ConfigFile))
        {
            if (!(Test-Path $ConfigFile))
            {
                throw [IO.FileNotFoundException] $ConfigFile
            }

            $pSet += "--configfile"
            $pSet += "`"$ConfigFile`""
        }

        if ($PSCmdlet.ShouldProcess("dotet nuget add"))
        {
            if (![string]::IsNullOrWhiteSpace($Username))
            {
                if (!$Password)
                {
                    throw "Username requires a password";
                }

                $pSet += "--username"
                $pSet += $Username

                $pSet += "--password"
                $pw = (New-Object PsCredential -ArgumentList "noop", $Password).GetNetworkCredential().Password
                $pSet += $pw 
            }

            dotnet nuget add source @pSet
        }
        else
        {
            if (![string]::IsNullOrWhiteSpace($Username))
            {
                if (!$Password)
                {
                    throw "Username requires a password";
                }

                $pSet += "--username"
                $pSet += $Username
                $pSet += "--password"
                $pSet += "*************"
            }

            Write-Debug "dotnet nuget add source $([string]::Join(" ", $pSet))"
        }       
    }
}