
function Enable-WindowsAutoLogon()
{
    <#
    .SYNOPSIS
        Enables the auto logon feature for windows.
    .DESCRIPTION
        The auto logon feature should only be used in limited scenarios
        such as automation with an account that rotates the password
        or is only enabled for automation purposes.

        `-Force` is require if the feature is already enabled and the desire
        is to overwrite the credentials.
    .EXAMPLE
        PS C:\> Enable-WindowsAutoLogin -Credential (Get-Credential)
        Prompts the user for a windows credential and then enables the
        auto logon feature.
    .PARAMETER Credential
        The credentials of the administrator that will be stored.
    .PARAMETER Force
        If an entry already exists, the force flag will overwrite the values.
    .INPUTS
        PSCredential
    .OUTPUTS
        None
    #>
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)]
        [PsCredential] 
        $Credential,

        [Switch] 
        $Force
    )

    $autoLogonPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon'

    $value = Get-ItemPropertyValue -Path $autoLogonPath -Name "AutoAdminLogin"

    if ($value -eq 1 -and !$Force.ToBool())
    {
        Write-Warning "AutoAdminLogin is already enabled"
        return
    }

    New-ItemProperty -Path $autoLogonPath `
        -Name AutoAdminLogon `
        -Value 1 `
        -Force

    New-ItemProperty -Path $autoLogonPath `
        -Name DefaultUserName `
        -Value $Credential.UserName `
        -Force

    New-ItemProperty -Path $autoLogonPath `
        -Name DefaultPassword `
        -Value ($Credential.GetNetworkCredential().Password) `
        -Force
}