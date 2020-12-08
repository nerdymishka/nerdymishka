function Invoke-Process()
{
    [CmdletBinding()]
    param (
        [Parameter()]
        [String]
        $FileName,

        [Parameter()]
        [String]
        $Arguments,

        [Parameter()]
        [String]
        $WorkingDirectory,

        [Parameter()]
        [System.Int32] 
        $Timeout = 0,

        [Parameter()]
        [scriptblock] 
        $StdOutHandler,

        [Parameter()]
        [scriptblock] 
        $StdErrHandler 
    )

    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.CreateNoWindow = $true 
    $startInfo.UseShellExecute = $false 
    $startInfo.RedirectStandardError = $true 
    $startInfo.RedirectStandardOutput = $true 
    $startInfo.FileName = $FileName

    if (![string]::IsNullOrWhiteSpace($Arguments))
    {
        $startInfo.Arguments = $Arguments
    }
    
    if (![String]::IsNullOrWhiteSpace($WorkingDirectory))
    {
        $startInfo.WorkingDirectory = $WorkingDirectory
    }

    $process = [System.Diagnostics.Process]::new()
    $process.StartInfo = $startInfo

    $out = [System.Text.StringBuilder]::new()
    $err = [System.Text.StringBuilder]::new()

    $action = {
        if (! [String]::IsNullOrEmpty($EventArgs.Data))
        {
            $Event.MessageData.AppendLine($EventArgs.Data)
        }
    }
    $outEvent = Register-ObjectEvent -InputObject $process `
        -Action $action `
        -EventName 'OutputDataReceived' `
        -MessageData $out
    $errEvent = Register-ObjectEvent -InputObject $process `
        -Action $action `
        -EventName 'ErrorDataReceived' `
        -MessageData $err

    $outEvent2 = $null
    $errEvent2 = $null 

    if ($StdOutHandler)
    {
        $outEvent2 = Register-ObjectEvent -InputObject $process `
            -Action $outEvent2 `
            -EventName 'OutputDataReceived' 
    }

    if ($StdErrHandler)
    {
        $errEvent2 = Register-ObjectEvent -InputObject $process `
            -Action $action `
            -EventName 'ErrorDataReceived' `
    }

    [void]$process.Start()
    $process.BeginOutputReadLine()
    $process.BeginErrorReadLine()
    [void]$process.WaitForExit($Timeout)

    Unregister-Event -SourceIdentifier $outEvent.Name
    Unregister-Event -SourceIdentifier $errEvent
    if ($outEvent2)
    {
        Unregister-Event -SourceIdentifier $outEvent2
    }

    if ($errEvent2)
    {
        Unregister-Event -SourceIdentifier $errEvent2
    }

    $result = [PSCustomObject]@{
        FileName         = $FileName
        Arguments        = $Arguments
        WorkingDirectory = $WorkingDirectory
        StdOut           = $out.ToString()
        StdErr           = $err.ToString()
        ExitCode         = $process.ExitCode
        StartedAt        = $process.StartTime
        EndedAt          = $process.ExitTime
        Duration         = $process.ExitTime - $process.StartTime
    }
    $process.Dispose()

    Write-Output $result
}