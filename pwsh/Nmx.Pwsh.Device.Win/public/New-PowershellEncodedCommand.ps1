function New-PowerShellEncodedCommand()
{
    [CmdletBinding()]
    param (
        [Parameter()]
        [String]
        $Command
    )

    $cmd = [Convert]::ToBase64String([System.Text.Encoding]::Unicode.GetBytes($Command))

    return @(
        '-NoLogo',
        '-NoProfile',
        '-ExecutionPolicy',
        'Bypass',
        '-E',
        $cmd
    )
}