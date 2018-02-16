This repo is to show how you can create an Azure DocumentDB Database and read from it using .Net Core running on Linux.  This guide will walk through how to install all the dependencies  all the way through deploying a docker container on Azure Web Apps for Linux. The guide provides the commands to run as of 4/9/2017. Some commands may have been updated since this has been posted. 

The follow was completed using Centos but .NET Core and the new Az 2.0 cli can run on a variety of Linux distributions. 

Azure - Install cli
============================
1. curl -L https://aka.ms/InstallAzureCli | bash

DotNet 2.0 Install 
==================
1. sudo yum install libunwind libicu
2. curl -sSL -o dotnet.tar.gz https://download.microsoft.com/download/1/1/5/115B762D-2B41-4AF3-9A63-92D9680B9409/dotnet-sdk-2.1.4-linux-x64.tar.gz
3. sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
4. sudo ln -s /opt/dotnet/dotnet /usr/local/bin

Azure - Create Document DB
============================
1. az login 
2. az group create --name DocDbTest --location westus
3. az documentdb create -g DocDbTest -n <uniqueName> --kind SQL (Make note of the "documentEndpoint")
4. az documentdb list-keys -g DocDbTest -n <uniqueName> (Make note of the "primaryMasterKey")

DocumentDB ASP.NET Code
==================
This this will be used to display items in the DocDb Database Collection in a web page.  The code is also in the DocumentDB repo or we can create it from scratch.
1. dotnet new mvc -o docdb 
2. Update Working.csproj to include `<PackageReference Include="Microsoft.Azure.DocumentDB.Core" Version="1.2.1" />`
3. dotnet restore 
4. dotnet build
5. export DOCDB_KEY='' (primaryMasterKey from earlier)
6. export DOCDB_ENDPOINT='' (documentEndpoint from earlier)
7. dotnet build
8. dotnet run
9. dotnet publish -o /path/to/publish/directory
	
Docker
======
1. Create dockerfile in the parent directory of /path/to/publish/directory
	```docker
	FROM microsoft/dotnet:latest
	RUN mkdir -p /usr/src/app
	WORKDIR /usr/src/app
	COPY publish /usr/src/app
	EXPOSE 5000/tcp
	ENTRYPOINT dotnet /usr/src/app/todo.core.dll (or whatever your called your application with dotnet create)
	```
2. sudo docker build -t [name/for_docker_image] .
3. sudo docker run -p 8080:5000 -e DOCDB_KEY='…==' -e DOCDB_ENDPOINT='https://..' [name/for_docker_image] -d
4. curl http://localhost:8080 (May need to do this from another console)
5. sudo docker push [name/for_docker_image]

Azure - Create Web for Linux
============================
1. az login 
2. az appservice plan create -g DocDbTest -n LinuxServicePlan --is-linux --number-of-workers 1 --sku S1
3. az appservice web create -g DocDbTest -p LinuxServicePlan -n [uniqueName]
4. az appservice web config container update -g DocDbTest -n  [uniqueName] -c [name/for_docker_image]
5. az appservice web config appsettings update -g DocDbTest -n [uniqueName] --settings PORT=5000 DOCDB_KEY=<accessKey> DOCDB_ENDPOINT=<endpointUrl>

Enjoy!