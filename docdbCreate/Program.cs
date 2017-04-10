using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace docdbCreate
{
    class Person
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

    class Program
    {
        private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy { UserAgentSuffix = " samples-net/3" };
	private static DocumentClient client;
	private static Database database; 
	private static DocumentCollection coll; 
	private static Document doc; 
        
	static void Main(string[] args)
        {
	    if( args.Length != 4 ) {
	        Console.WriteLine("docdbCreate <endpointUrl> <databaseName> <collectionName> <accessKey>");
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
		    doc = InsertIntoDatabaseCollection( new Person { Id = "001", lastName = "Caesar", firstName = "Julius", age = "60" } ).Result;
		    doc = InsertIntoDatabaseCollection( new Person { Id = "002", lastName = "Caesar", firstName = "August", age = "74" } ).Result;
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

	private static async Task<Document> InsertIntoDatabaseCollection( Person person ) 
	{ 
   	     Console.WriteLine( "Person being created {0}",  person.firstName );
	     Document doc = await client.CreateDocumentAsync( coll.SelfLink, person );	
	     return doc;
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
