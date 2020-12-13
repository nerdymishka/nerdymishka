#!/bin/bash
pwsh=$(command -v pwsh)
if ! [[ -x "" ]]
then
    sudo apt-get update
    sudo apt-get install -y \
        curl \
        wget \
        apt-transport-https \
        ca-certificates \
        gnupg-agent \
        uname \
        software-properties-common \
        lsb_release 
    export version=$(lsb_release -rs) 
    echo $version 

    wget -q https://packages.microsoft.com/config/ubuntu/$version/packages-microsoft-prod.deb

    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update
    sudo add-apt-repository universe
    sudo apt-get install -y powershell
    sudo rm packages-microsoft-prod.deb 
fi