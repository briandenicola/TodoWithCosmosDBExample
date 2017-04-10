using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Working.Models;

namespace Working.Controllers
{
    public class HomeController : Controller
    {
        private DocumentClient client;

        public IActionResult Index()
        {
            string databaseName = "wordsdb";
            string collectionName = "wordsCollection";
            string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");
            string EndpointUri = Environment.GetEnvironmentVariable("DocDbUri");

            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            // Here we find the Andersen family via its LastName
            IQueryable<Person> query = this.client.CreateDocumentQuery<Person>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(f => f.f != null);
           
            ViewData["Persons"] = query;
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
