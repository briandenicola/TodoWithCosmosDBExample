using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Net;

namespace todo.Models
{
	public class DocumentDBRepository<T>
	{
		private static DocumentDbSettings _settings;
		private static DocumentClient client;

		public DocumentDBRepository(DocumentDbSettings settings)
		{
			_settings = settings;
			client = new DocumentClient(_settings.DatabaseUri, _settings.DatabaseKey);
			client.OpenAsync().Wait();
		}

		private static async Task CreateDatabaseIfNotExistsAsync()
		{
			try {
				await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_settings.DatabaseName));
			}
			catch (DocumentClientException e) {
				if (e.StatusCode == System.Net.HttpStatusCode.NotFound)	
					await client.CreateDatabaseAsync(new Database { Id = _settings.DatabaseName });
				else
					throw;
			}
		}

		private static async Task CreateCollectionIfNotExistsAsync()
		{
			try	{
				await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseName, _settings.CollectionName));
			}
			catch (DocumentClientException e) {
				if (e.StatusCode == System.Net.HttpStatusCode.NotFound)	{
					await client.CreateDocumentCollectionAsync(
						UriFactory.CreateDatabaseUri(_settings.DatabaseName),
						new DocumentCollection { Id = _settings.CollectionName },
						new RequestOptions { OfferThroughput = 1000 });
				}
				else
					throw;
			}
		}

		public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
		{
			IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
				UriFactory.CreateDocumentCollectionUri(_settings.DatabaseName, _settings.CollectionName))
				.Where(predicate)
				.AsDocumentQuery();

			List<T> results = new List<T>();
			while (query.HasMoreResults) {
				results.AddRange(await query.ExecuteNextAsync<T>());
			}

			return results;
		}

		public async Task<Document> CreateItemAsync(T item)
		{
			return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DatabaseName, _settings.CollectionName), item);
		}

		public async Task<Document> UpdateItemAsync(string id, T item)
		{
			return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseName, _settings.CollectionName, id), item);
		}
		
		public async Task<Document> DeleteItemAsync(string id)
		{
			return await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseName, _settings.CollectionName, id));
		}

		public async Task<T> GetItemAsync(string id)
		{
			try {
				Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_settings.DatabaseName, _settings.CollectionName, id));
				return (T)(dynamic)document;
			}
			catch (DocumentClientException e) {
				if (e.StatusCode == HttpStatusCode.NotFound) {
					return default(T);
				}
				else {
					throw;
				}
			}
		}
	}
}