using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using System.Linq.Expressions;
using System.Collections.Generic;
using todo.Models;
using System;

namespace todo.core.Services
{
    public class DocumentDbService : IDocumentDbService
    {
		private static DocumentDBRepository<Item> _provider;
		public DocumentDbService(IConfiguration configuration)
		{
			_provider = new DocumentDBRepository<Item>(new DocumentDbSettings(configuration));
		}

		public async Task<Item> GetItem(string id) {
			return await _provider.GetItemAsync(id);
		}

		public async Task<IEnumerable<Item>> GetItems(Expression<Func<Item, bool>> predicate) {
			return await _provider.GetItemsAsync(predicate);
		}

		public async Task<Document> AddItem(Item item) {
			return await _provider.CreateItemAsync(item);
		}

		public async Task UpdateItem(string id, Item item) {
			await _provider.UpdateItemAsync(id, item);
		}
		public async Task DeleteItem(string id) {
			await _provider.DeleteItemAsync(id);
		}
	}
}
