
<#
$vsWebGuids = @{
    "ASP.NET 5"     = "{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}";
    "ASP.NET MVC 1" = "{603C0E0B-DB56-11DC-BE95-000D561079B0}";
    "ASP.NET MVC 2"	= "{F85E285D-A4E0-4152-9332-AB1D724D3325}";
    "ASP.NET MVC 3" = "{E53F8FEA-EAE0-44A6-8774-FFD645390401}";
    "ASP.NET MVC 4" = "{E3E379DF-F4C6-4180-9B81-6769533ABE47}";
    "ASP.NET MVC 5" = "{349C5851-65DF-11DA-9384-00065B846F21}";
    "WebSite"       = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
}

$testRefs = @("Microsoft.VisualStudio.QualityTools.UnitTestFramework", "xunit", "NUnit.Framework", "NUnitLite")
$testPackageRef = @("Microsoft.NET.Test.Sdk") 

$tsImport = "`$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v`$(VisualStudioVersion)\TypeScript\Microsoft.TypeScript.targets"
$vslibSdkValue = "Microsoft.NET.Sdk"
$vsWebSdkValue = "Microsoft.NET.Sdk.Web" 
$ns = @{"msb" = "http://schemas.microsoft.com/developer/msbuild/2003" }
$serviceRef = "System.ServiceProcess"
$languageGuids = @{
    "C#"     = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
    "C++"    = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}"
    "F#"     = "{F2A71F9B-5D33-465A-A702-920D77279786}"
    "J#"     = "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}"
    "VB.NET" = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}"
}


# $vsWpfGuid = "{60dc8134-eba5-43b8-bcc9-bb4bc16c2548}";
#>


$vsProjectFiles = @{
    '.csproj'  = '.cs'
    '.vcxproj' = '.cpp'
    '.vbproj'  = '.vb'
    '.fsproj'  = '.fs'
    '.vsjproj' = '.jsl'
    '.wixproj' = '.wxs'
    '.njsproj' = '.js'
    '.ccproj'  = '.csdef'
} 


function Read-GzVisualStudioSolution()
{
    <#
.SYNOPSIS
    Reads a Visual Studio solution file and  returns meta
    information including the version of Visual Studio 
    for the solution file and a hashtable of projects
    with meta info about the projects.

.DESCRIPTION
    Reads a Visual Studio solution file and  returns meta
    information including the version of Visual Studio 
    for the solution file and a hashtable of projects
    with meta info about the projects.

    The hashtable key will be the project names. The value
    will have meta info about the project including:
    - Name: name of the project
    - File: the relative path to the project file
    - Id: the id of project e.g. guid
    - Ext: project extension e.g. csproj
    - LanguageExt: the code file extension e.g. .cs
    - IsSdk: true if the project is a dotnet standard/core project


.EXAMPLE
    PS C:\> info = Read-VisualStudioSolution "$Home/Projects/Project.sln"

.OUTPUTS
    a custom object with meta information about the projects in the
    solution and the version of Visual Studio that created the sln.
#>
    [CmdletBinding()]
    param(
        [Parameter(Position = 0)]
        [string] 
        $Path
    )
    $ErrorActionPreference = "Stop"
    if ([String]::IsNullOrWhiteSpace($Path))
    {
        $items = Get-Item "$($PWD.Path)/*.sln"
        if (!$items)
        {
            throw [System.IO.FileNotFoundException] "$($PWD.Path)/*.sln"
        }

        if ($items -is [Array])
        {
            $Path = $items[0].FullName
        }
        else 
        {
            $Path = $items.FullName
        }
    }

    if (!$Path.EndsWith(".sln"))
    {
        throw [System.IO.FileNotFoundException] "$Path does not end with the .sln extensions"
    }

    if (!(Test-Path $Path))
    {
        throw [System.IO.FileNotFoundException] $Path 
    }

    $projects = @{};
    $parentDir = Split-Path $Path 
    $lines = Get-Content $Path
    $version = $null;

    foreach ($line in $lines)
    {

        if ($line.StartsWith("VisualStudioVersion ="))
        {
            $version = $line.Substring($line.IndexOf("=") + 1).Trim()
        }

        if ($line.StartsWith("Project("))
        {
            $data = $line.Substring($line.IndexOf("=") + 1);
            $data = $data.Split(",")
            if ($data.Length -lt 3)
            {
                continue;
            }
          
            $projName = $data[0].Trim().Trim('"')
            $projFile = $data[1].Trim().Trim('"')
            $projFile = "$parentDir\$projFile"
            $projId = $data[2].Trim().Trim('"')
            $ext = [System.IO.Path]::GetExtension($projFile)
            $languageExt = $null
            $isSdk = $false;

            if ($vsProjectFiles.ContainsKey($ext))
            {
                $languageExt = $vsProjectFiles[$ext]
            }

            if (![IO.Path]::HasExtension($projFile))
            {
                continue;
            }

            try
            {
                $xml = [xml](Get-Content $projFile)
                if ($xml.DocumentElement.HasAttribute("Sdk"))
                {
                    $isSdk = $true;
                }
            }
            catch
            {

            }

            $model = [PsCustomObject]@{
                'Name'           = $projName
                'File'           = $projFile
                'Id'             = $projId
                'IsCloudService' = $ext -eq '.ccproj'
                'IsWixProject'   = $ext -eq ".wixproj"
                'Ext'            = $ext
                'LanguageExt'    = $languageExt
                'IsSdk'          = $isSdk
            }

            $projects.Add($projName, $model);
        }
    }

    return [PsCustomObject]@{
        Version  = $version
        Projects = $projects
    }
}