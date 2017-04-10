using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace docdbCreate
{
    class Program
    {
        private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy { UserAgentSuffix = " samples-net/3" };
	private static DocumentClient client;
	private static Database database; 
	private static DocumentCollection coll; 
        
	static void Main(string[] args)
        {
	    if( args.Length != 4 ) {
	        System.Console.Write("docdbCreate <endpointUrl> <databaseName> <collectionName> <accessKey>");
		return;
	    }

	    string endpointUrl  = args[0];
	    string databaseName = args[1];
	    string collectionName = args[2];
	    string authorizationKey  = args[3];

            try
            {
                using (client = new DocumentClient(new Uri(endpointUrl), authorizationKey, connectionPolicy))
                {
                    database = CreateDatabaseAsync(databaseName).Result;
                    coll = CreateDatabaseCollectionAsync(collectionName).Result; 
                }
            }            
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
        }

        private static async Task<Database> CreateDatabaseAsync(string name)
        {
            Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = name });
            return database;
        }

        private static async Task<DocumentCollection> CreateDatabaseCollectionAsync(string name)
        {
	    DocumentCollection coll = await client.CreateDocumentCollectionIfNotExistsAsync(
		database.SelfLink,
		new DocumentCollection { Id = name },
		new RequestOptions { OfferThroughput = 1000 }
	    );
	    return coll;
        }
    }
}
