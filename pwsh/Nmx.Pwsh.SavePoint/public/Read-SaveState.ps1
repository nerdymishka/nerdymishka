function Read-SaveState()
{
    [CmdletBinding()]
    param (
        [Parameter(Position = 0)]
        [String]
        $Path  
    ) 

    if ([String]::IsNullOrWhiteSpace($Path))
    {
        $Path = Get-SaveStateFileName
    }
    
    if ([String]::IsNullOrWhiteSpace($Path))
    {
        throw [System.ArgumentNullException] "-Path must not be mull or whitespace."
    }

    if (! (Test-Path $Path))
    {
        throw [System.IO.FileNotFoundException] $Path 
    }
       
    if ($Host.Version.Major -gt 5)
    {
        return Get-Content $Path -Raw | ConvertFrom-Json -AsHashtable -Depth 20
    }
    else 
    {
        $data = Get-Content $Path -Raw | ConvertFrom-Json 
        return $data | ConvertTo-Hashtable 
    }
}