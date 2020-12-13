function Install-Docker()
{
    if(!(Get-Command docker -EA SilentlyContinue))
    {
        curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
        $release = lsb_release -cs 
        $uri = "https://download.docker.com/linux/ubuntu"
        write-host $release 
        sudo add-apt-repository "deb [arch=amd64] $uri $release stable"
        sudo apt-get update
        sudo apt-get install -y docker-ce docker-ce-cli containerd.io
        if(Get-Command docker -EA SilentlyContinue)
        {
            Write-Host "docker installed"
        }
    }
   

    if(!(Get-Command docker-compose -EA SilentlyContinue))
    {
        $version = "1.27.4"
        $uri = "https://github.com/docker/compose/releases/download/$version/docker-compose-$(uname -s)-$(uname -m)" 
        sudo curl -L $uri -o /usr/local/bin/docker-compose
        sudo chmod +x /usr/local/bin/docker-compose
    }
}