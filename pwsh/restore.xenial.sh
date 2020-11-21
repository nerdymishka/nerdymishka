if ! command -v pwsh &> /dev/null
then
    echo "installing powershell"
    # Update the list of packages
    sudo apt-get update
    # Install pre-requisite packages.
    sudo apt-get install -y wget apt-transport-https
    # Download the Microsoft repository GPG keys
    wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
    # Register the Microsoft repository GPG keys
    sudo dpkg -i packages-microsoft-prod.deb
    # Update the list of products
    sudo apt-get update
    # Enable the "universe" repositories
    sudo add-apt-repository universe
    # Install PowerShell
    sudo apt-get install -y powershell
    # Start PowerShell
fi

if ! command -v nuget &> /dev/null 
then 
   sudo apt install dotnet-sdk-5.0 nuget -y 
fi 

sudo pwsh restore.ps1