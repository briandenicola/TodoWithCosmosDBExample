using System;
using Microsoft.Extensions.Configuration;

namespace todo.Models
{
    public class DocumentDbSettings
    {
		public string DatabaseName { get; private set; }
		public string CollectionName { get; private set; }
		public Uri DatabaseUri { get; private set; }
		public string DatabaseKey { get; private set; }

		public DocumentDbSettings(IConfiguration configuration)
		{
			try
			{
				DatabaseName = configuration.GetSection("database").Value;
				CollectionName = configuration.GetSection("collection").Value;
				//DatabaseUri = new Uri(configuration.GetSection("endpoint").Value);
				//DatabaseKey = configuration.GetSection("authKey").Value;
				DatabaseKey = Environment.GetEnvironmentVariable("DOCDB_KEY");
				DatabaseUri = new Uri(Environment.GetEnvironmentVariable("DOCDB_ENDPOINT"));
			}
			catch
			{
				throw new MissingFieldException("IConfiguration missing a valid Azure DocumentDB fields on DocumentDB > [DatabaseName,CollectionName,EndpointUri,Key]");
			}
		}

	}
}