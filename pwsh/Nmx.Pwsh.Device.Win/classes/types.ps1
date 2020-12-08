class WindowsFeatureSection 
{
    [string] $name 
    [nullable[boolean]] $remove
}

enum WindowsFeatureStatus
{
    NotAvailable = 0
    NoOperation = -1
    Enabled = 1
    Disabled = 2
}

class WindowsFeatureResult
{
    [string] $name 
    [WindowsFeatureStatus] $status 
    [bool] $rebootRequired
    [bool] $enabled = $false
}

class WindowsRegistryKeySection
{
    [string] $Name 
    [string] $Key
    [nullable[boolean]] $Remove
    [string] $Type = [WindowsRegistryValueType]::None.ToString()
    [object] $Value
    [WindowsRegistryKeySection] $SubKeys
}

enum WindowsRegistryKeyStatus 
{
    NoOperation
    Added
    Removed
    Synced 
}

class WindowsRegistryKeyOutput
{
    [string] $Key
    [string] $Name
    [WindowsRegistryKeyStatus] $Status = [WindowsRegistryKeyStatus]::NoOperation
    [WindowsRegistryKeyOutput[]] $SubKeys
}

enum WindowsRegistryValueType 
{
    None = -1
    Binary = 6
    Dword = 5 
    ExpandString = 4
    MultiString = 3
    Qword = 2    
    String = 1
    Unknown = 0
}
