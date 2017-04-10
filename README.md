This repo is to show how you can create an Azure DocumentDB Database and read from it using .Net Core running on Linux.  This guide will walk through how to install all the dependencies  all the way through deploying a docker container on Azure Web Apps for Linux. The guide provides the commands to run as of 4/9/2017. Some commands may have been updated since this has been posted. 

The follow was completed using Centos but .NET Core and the new Az 2.0 cli can run on a variety of Linux distributions. 

DotNet 1.1 Install 
==================
1. sudo yum install libunwind libicu
2. curl -sSL -o dotnet.tar.gz https://go.microsoft.com/fwlink/?linkid=843449
3. sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
4. sudo ln -s /opt/dotnet/dotnet /usr/local/bin

DocumentDB Console Code
==================
This this will be used to create a docdb database collection and insert two records
1. mkdir <path/to/docdbclient>
2. cd <path/to/docdbclient>
3. wget https://github.com/bjd145/DocumentDBSampleCode/archive/master.zip (or do a git clone https://github.com/bjd145/DocumentDBSampleCode.git )
4. unzip master.zip
5. dotnet restore
6. dotnet build - This will be executed later on.
	   
DocumentDB ASP.NET Code
==================
This this will be used to display items in the DocDb Database Collection in a web page 
1. dotnet new mvc -o docdb 
2. Update Working.csproj to include `<PackageReference Include="Microsoft.Azure.DocumentDB.Core" Version="1.2.1" />`
3. dotnet restore 
4. dotnet build
5. export PrimaryKey=''
6. export DocDbUri=''
7. Update Program.cs: 
	```CSharp
	var host = new WebHostBuilder()
		.UseKestrel().UseUrls("http://*:5000")
		.UseContentRoot(Directory.GetCurrentDirectory())
		.UseStartup<Startup>()
		.Build();
	```
8. Create Models/Person.cs
	```CSharp
		using System;
		using System.Collections.Generic;
		using Newtonsoft.Json;
		
		namespace Working.Models
		{
			public class Person
			{
				[JsonProperty(PropertyName = "id")]
				public string Id { get; set; }
				public string lastName { get; set; }
				public string firstName { get; set; }
				public string age { get; set; }
				public override string ToString()
				{
					return JsonConvert.SerializeObject(this);
				}
			}
		}
	```
9. Edit Controllers/HomeController.cs:using System;
	```CSharp
		using Microsoft.Azure.Documents;
		using Microsoft.Azure.Documents.Client;
		using Newtonsoft.Json;
		using Working.Models;
		…
		public IActionResult Index()
		{
			string databaseName = "wordsdb";
			string collectionName = "wordsCollection";
			string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");
			string EndpointUri= Environment.GetEnvironmentVariable("DocDbUri");

			this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
			
			FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };
			
			IQueryable<Person> query = this.client.CreateDocumentQuery<Person>(
				UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
				.Where(f => f.f != null);
			
			ViewData["Persons"] = query;
			return View();
		}
	```
10. Edit Views/Home/Index.cshtml
	```CSharp
		@model Working.Models.Person
		@{
			ViewData["Title"] = "Home Page";
		}
		<h2>Documents</h2>
		<div>
		@foreach (var item in ViewData["Persons"] as IQueryable<Working.Models.Person> ) {
			<p>FirstName - @(item.f), LastName - @(item.l), Age - @(item.a)</p>
		}
		</div>
	```
11. dotnet build
12. dotnet run
13. dotnet publish -o /path/to/publish/directory
	
Docker
======
1. Create dockerfile
	```docker
	FROM microsoft/dotnet:latest
	RUN mkdir -p /usr/src/app
	WORKDIR /usr/src/app
	COPY publish /usr/src/app
	EXPOSE 5000/tcp
	ENTRYPOINT dotnet /usr/src/app/Working.dll (or whatever your called your application with dotnet create)
	```
2. sudo docker build -t <name/for_docker_image> .
3. sudo docker run -p 8080:5000 -e PrimaryKey='…==' -e DocDbUri='https://..' <name/for_docker_image> -d
4. curl http://localhost:8080 (May need to do this from another console)
5. sudo docker push <name/for_docker_image>

Azure Cli
=========
1. curl -L https://aka.ms/InstallAzureCli | bash
2. az login 
3. az group create --name DocDbTest --location westus
4. az appservice plan create -g DocDbTest -n LinuxServicePlan --is-linux --number-of-workers 1 --sku S1
5. az appservice web create -g DocDbTest -p LinuxServicePlan -n <uniqueName>
6. az appservice web config container update -g DocDbTest -n  <uniqueName> -c <name/for_docker_image>
7. az documentdb create -g DocDbTest -n <uniqueName> --kind MongoDB (Make note of the "documentEndpoint")
8. az documentdb list-keys -g DocDbTest -n <uniqueName> (Make note of the "primaryMasterKey")
9. cd <path/to/docdbclient>
10. `dotnet run <endpointUrl> <databaseName> <collectionName> <accessKey>`
11. az appservice web config appsettings update -g DocDbTest -n <uniqueName> --settings PORT=5000 PrimaryKey=<accessKey> DocDbUri=<endpointUrl>

Enjoy!