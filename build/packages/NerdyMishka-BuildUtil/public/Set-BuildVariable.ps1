

function Set-BuildVariable()
{
    Param(
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true, ValueFromPipeline = $true)]
        [Alias("Name")]
        [Object] $InputObject,

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = $true)]
        [String] $Value 
    )

    if($InputObject -is [string])
    {
        $Name = $InputObject
        if($ENV:TF_BUILD)
        {
            Write-Host "##vso[task.setvariable variable=$Name]$Value"
        }
    
        Set-Item -Path "Env:/$Name" -value $Value 
        return 
    }

    if($InputObject -is [PsCustomObject])
    {
        $InputObject | Get-Member -MemberType NoteProperty | ForEach-Object {
            $name = $_.Name 
            $value = $InputObject[$Name]
            Set-BuildVariable -Name $name -value $value 
        }

        return 
    }

    if($InputObject -is [Hashtable])
    {
        foreach($key in $InputObject.Keys)
        {
            $value = $InputObject[$key]
            Set-BuildVariable -Name $key -Value $Value
        }

        return 
    }
}