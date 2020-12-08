function Get-WindowsUninstallRegistryKey()
{
    

    $hkcuKey = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'
    $hklmKey = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*'
    $hklm64Key = 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*'

    return Get-ChildItem -Path @($hklm64Key, $hklmKey, $hkcuKey) -ErrorAction SilentlyContinue  
}
