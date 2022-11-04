using ACME.DataLayer.Documents;
using ACME.DataLayer.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Text.RegularExpressions;

namespace ACME.Frontend.CoreSqlConsole;

internal class Program
{
    private static string Host = "https://ps-kosmologie.documents.azure.com:443/";
    private static string PrimaryKey = "dBtaqbHjxHkJFLfjvfktzbikpxezTOUaL478ha5QV9ZCkDLmTFobURWvJJvsSPRkEjQaS799moaVLf399rmB3g==";
    private static string Database = "productDB";
    public static int Port = 443;
    public static bool EnableSSL = true;
    //private static string _connectionString = "AccountEndpoint=https://ps-coresql.documents.azure.com:443/;AccountKey=fFFEM8CVNeRp3GTsLwBvYbDPbvUicyWUivEh6d8sTXP5KIpwciVqlurIm9IVtyh4YFeZWsU78rR8n1eaMdzt8g==;";
    //private static string _database = "products";

    private static CosmosClient? _cosmosClient;
    static async Task Main(string[] args)
    {
        var options = new CosmosClientOptions
        {
            //AllowBulkExecution = true,
            //ApplicationRegion = "westeurope",
            //ConnectionMode = ConnectionMode.Direct,
            //ConsistencyLevel = ConsistencyLevel.Session,
            //MaxRetryAttemptsOnRateLimitedRequests = 5,
            //MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(5)
        };
        _cosmosClient = new CosmosClient(Host, PrimaryKey);

        //await PrepareDatabaseAsync();
        //await QueryDocumentsAsync();
        await QueryDocumentsWithContinuationAsync();
        //await QueryDocumentsLinqAsync();
        //await InsertDocumentAsync();
        //await UpsertDocumentAsync();
        //await DeleteDocumentAsync();
        Console.WriteLine("Done!");
        Console.ReadLine();
    }
    private static async Task QueryDocumentsAsync()
    {
        var brands = await ReadBrandsAsync();
        var groups = await ReadProductGroupsAsync();

        var query = "SELECT * FROM p WHERE p.Type = 'product' And STARTSWITH(p.Name, 'D')";
        var productContainer = _cosmosClient!.GetContainer(Database, CosmosDb.PRODUCTS);
        var queryDef = new QueryDefinition(query);

        var queryOptions = new QueryRequestOptions
        {
            //MaxItemCount = 25
        };
        // FeedIterator<ProductDocument>
        using var iterator = productContainer.GetItemQueryIterator<ProductDocument>(
            queryDef, requestOptions: queryOptions);
        while (iterator.HasMoreResults)
        {
            // FeedResponse<ProductDocument>
            var fResponse = await iterator.ReadNextAsync();
            Console.WriteLine($"Products request Charge: {fResponse.RequestCharge} RUs");
            foreach (var item in fResponse)
            {
                var brand = brands.First(b => b.InternalId == item.BrandId);
                var group = groups.First(g => g.InternalId == item.ProductGroupId);
                Console.WriteLine($"[{group.Name}] {brand.Name} {item.Name}.");
            }
        }
    }
    private static async Task QueryDocumentsWithContinuationAsync()
    {
        var brands = await ReadBrandsAsync();
        var groups = await ReadProductGroupsAsync();

        var query = "SELECT * FROM p WHERE p.Type='product'";
        var productContainer = _cosmosClient!.GetContainer(Database, CosmosDb.PRODUCTS);
        var queryDef = new QueryDefinition(query);

        var queryOptions = new QueryRequestOptions
        {
            MaxItemCount = 10
        };
        string? continuationToken = null;
        do
        {
            using var iterator = productContainer.GetItemQueryIterator<ProductDocument>(
                queryDef, continuationToken, queryOptions);
            while (iterator.HasMoreResults)
            {
                var fResponse = await iterator.ReadNextAsync();
                foreach (var item in fResponse)
                {
                    var brand = brands.First(b => b.InternalId == item.BrandId);
                    var group = groups.First(g => g.InternalId == item.ProductGroupId);
                    Console.WriteLine($"[{group.Name}] {brand.Name} {item.Name}.");
                }
                continuationToken = fResponse.ContinuationToken;
            }
        }
        while (continuationToken != null);
    }
    private static async Task QueryDocumentsLinqAsync()
    {
        var brands = await ReadBrandsAsync();
        var groups = await ReadProductGroupsAsync();

        var productContainer = _cosmosClient!.GetContainer(Database, CosmosDb.PRODUCTS);
        var linq =  productContainer.GetItemLinqQueryable<ProductDocument>();
        var query = linq.Where(g => g.Type == DocumentType.Product)
            .Where(g => g.Id == 1);
        var q2 = from g in linq where g.Type == DocumentType.Product && g.Id == 1 select g;
             
        using var iterator = query.ToFeedIterator<ProductDocument>();
        while (iterator.HasMoreResults)
        {
            var fResponse = await iterator.ReadNextAsync();
            Console.WriteLine($"Products request Charge: {fResponse.RequestCharge} RUs");
            foreach (var item in fResponse)
            {
                var brand = brands.First(b => b.InternalId == item.BrandId);
                var group = groups.First(g => g.InternalId == item.ProductGroupId);
                Console.WriteLine($"[{group.Name}] {brand.Name} {item.Name}.");
            }
        }
    }
    private static async Task<List<BrandDocument>> ReadBrandsAsync()
    {
        var metasContainer = _cosmosClient!.GetContainer(Database, CosmosDb.METAS);
        var query = "SELECT * FROM p";
        var queryDef = new QueryDefinition(query);
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey(DocumentType.Brand) };
        using var iterator = metasContainer.GetItemQueryIterator<BrandDocument>(
            queryDef, requestOptions:options);
        var fResponse = await iterator.ReadNextAsync();
        Console.WriteLine($"Brands request Charge: {fResponse.RequestCharge} RUs");
        return fResponse.ToList();
    }
    private static async Task<List<ProductGroupDocument>> ReadProductGroupsAsync()
    {
        var metasContainer = _cosmosClient!.GetContainer(Database, CosmosDb.METAS);
        var query = "SELECT * FROM p";
        var queryDef = new QueryDefinition(query);
        var options = new QueryRequestOptions { PartitionKey = new PartitionKey(DocumentType.ProductGroup) };
        using var iterator = metasContainer.GetItemQueryIterator<ProductGroupDocument>(
            queryDef, requestOptions: options);
        var fResponse = await iterator.ReadNextAsync();
        Console.WriteLine($"ProductGroups request Charge: {fResponse.RequestCharge} RUs");
        return fResponse.ToList();
    }
    private static async Task InsertDocumentAsync()
    {
        var document = new ProductDocument
        {
            BrandId = "brand_1",
            ProductGroupId = "productgroup_1",
            Image = "image.jpg",
            Name = "Name",
            Id = 1_000_000
        };

        var productContainer = _cosmosClient!.GetContainer(Database, CosmosDb.PRODUCTS);
        var response = await productContainer.CreateItemAsync(document, new PartitionKey(document.PartitionKey));
        Console.WriteLine($"Product Write costs {response.RequestCharge} RUs");

        var doc = await productContainer.ReadItemAsync<ProductDocument>(document.InternalId, new PartitionKey(document.PartitionKey));
        Console.WriteLine($"Product Read costs {doc.RequestCharge} RUs");
        Console.WriteLine($"{doc.Resource.Name}");
    }
    private static async Task UpsertDocumentAsync()
    {
        var document = new ProductDocument
        {
            BrandId = "brand_1",
            ProductGroupId = "productgroup_1",
            Image = "image.jpg",
            Name = "Name 2",
            Id = 1_000_000
        };

        var productContainer = _cosmosClient!.GetContainer(Database, CosmosDb.PRODUCTS);
        var response = await productContainer.UpsertItemAsync(document, new PartitionKey(document.PartitionKey));
        Console.WriteLine($"Product Write costs {response.RequestCharge} RUs");

        var doc = await productContainer.ReadItemAsync<ProductDocument>(document.InternalId, new PartitionKey(document.PartitionKey));
        Console.WriteLine($"Product Upsert costs {doc.RequestCharge} RUs");
        Console.WriteLine($"{doc.Resource.Name}");
    }
    private static async Task DeleteDocumentAsync()
    {
        var document = new ProductDocument
        {
            BrandId = "brand_1",
            ProductGroupId = "productgroup_1",
            Image = "image.jpg",
            Name = "Name 2",
            Id = 1_000_000
        };

        var productContainer = _cosmosClient!.GetContainer(Database, CosmosDb.PRODUCTS);
        var response = await productContainer.DeleteItemAsync<ProductDocument>(document.InternalId, new PartitionKey(document.PartitionKey));
        Console.WriteLine($"Product Delete costs {response.RequestCharge} RUs");
    }
    private static async Task PrepareDatabaseAsync()
    {
        var options = new CosmosClientOptions
        {
            //AllowBulkExecution = true,
            //ConnectionMode = ConnectionMode.Direct,
            //ConsistencyLevel = ConsistencyLevel.Eventual,
            //MaxRetryAttemptsOnRateLimitedRequests = 5,
            //MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(5)
        };
        _cosmosClient = new CosmosClient(Host, PrimaryKey);
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(Database);
        var tool = new CosmosDb(_cosmosClient, Database);
        await tool.PopulateDatabaseAsync();
    }
}