using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace produce.Models
{
    public static class DocumentDBRepository<T> where T : class
    {

        private static readonly string Endpoint = "https://affrss.documents.azure.com:443/";
        private static readonly string Key = "CJjeho7E0nhMTXM754zl0lJn0cvEZsDaKpIv1owiBWGZgDBAXp0RRti2TC5oN0WLwvf7Gyj7LsGIavblPtjRWQ==";
        private static readonly string DatabaseId = "AFFRSS";
        private static readonly string CollectionId = "rss";
        private static DocumentClient client;

        public static async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(predicate)
                    .AsDocumentQuery();

                List<T> results = new List<T>();
                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<T>());
                }
                return results;
            }
            catch (Exception e)
            {
                throw (e);
            }
            return null;
        }

        public static async Task<Document> CreateItemAsync(T item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public static async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public static async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }

        public static void Initialize()
        {
            if (client is null)
            {
                ConnectionPolicy connectionPolicy = new ConnectionPolicy();

                string currentLocation = Environment.GetEnvironmentVariable("CurrentRegion");

                switch (currentLocation)
                {
                    case LocationNames.WestUS2:
                        connectionPolicy.PreferredLocations.Add(LocationNames.WestUS2); // first preference
                        connectionPolicy.PreferredLocations.Add(LocationNames.EastUS2); // second preference
                        break;
                    default:
                        connectionPolicy.PreferredLocations.Add(LocationNames.EastUS2); // first preference
                        connectionPolicy.PreferredLocations.Add(LocationNames.WestUS2); // second preference
                        break;
                }

                //Setting read region selection preference
                //connectionPolicy.PreferredLocations.Add(LocationNames.WestUS2); // first preference
                //connectionPolicy.PreferredLocations.Add(LocationNames.SouthCentralUS); // second preference

                client = new DocumentClient(new Uri(Endpoint), Key, connectionPolicy);
            }
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
