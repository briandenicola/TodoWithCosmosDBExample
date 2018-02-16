using System.Collections.Generic;
using System.Threading.Tasks;
using todo.Models;
using Microsoft.Azure.Documents;
using System.Linq.Expressions;
using System;

namespace todo.core.Services
{
    public interface IDocumentDbService
    {
		Task<Item> GetItem(string id);
		Task<IEnumerable<Item>> GetItems(Expression<Func<Item, bool>> predicate);
		Task<Document> AddItem(Item item);
		Task UpdateItem(string id, Item item);
		Task DeleteItem(string id);
	}
}
