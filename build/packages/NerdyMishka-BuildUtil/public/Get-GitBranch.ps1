
function Get-GitBranch()
{
    Param(
        [Switch] $CurrentOnly 
    )

    $output = git branch
    $set = @();
    foreach($line in $output)
    {
        if([string]::IsNullOrWhiteSpace($line))
        {
            continue;
        }
        $current = $false
        $branchName = $null  
        if($line.StartsWith("*"))
        {
            $current = $true;
            $branchName = $line.SubString(2).Trim()
            if($CurrentOnly)
            {
                return $branchName
            }
        } else {
            $branchName = $line.Trim()
        }

        $set += [PsCustomObject]@{
            "IsCurrent" = $current
            "Name" = $branchName 
        }
    }

    Write-Output $set -NoEnumerate 
}