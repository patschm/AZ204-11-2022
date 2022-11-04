using ACME.Backend.Tools.Converters;
using ACME.DataLayer.Documents;
using ACME.DataLayer.Repository.SqlServer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace ACME.Frontend.CoreSqlConsole;

internal class CosmosDb
{
    public const string METAS = "metas";
    public const string PRODUCTS = "products";
    public const string USERS = "users";
    private string _sqlServerSource = @"Server=.\SQLEXPRESS;Database=ProductCatalog;Trusted_Connection=True;MultipleActiveResultSets=true;";
    private readonly CosmosClient _cosmosClient;
    private readonly string _database;
    public CosmosClient CosmosClient { get => _cosmosClient; }
    public string Database { get => _database; }
        
    public CosmosDb(CosmosClient cosmosClient, string database)
    {
        _cosmosClient = cosmosClient;
        _database = database;
    }

    public async Task PopulateDatabaseAsync()
    {
        await CreateDatabaseAsync();
        await CreateContainerAsync(METAS);
        await CreateContainerAsync(PRODUCTS);
        await CreateContainerAsync(USERS);
        await MigrateReviewersAsync();
        await MigrateMetasAsync();
        await MigrateProductsAsync();
    }
    private async Task CreateDatabaseAsync()
    {
        Console.WriteLine($"Creating Database {Database}...");
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(Database, throughput:4000);
    }
    private async Task<Container> CreateContainerAsync(string container, string partitionKey="partitionkey")
    {
        Console.WriteLine($"Creating Container {container}...");
        var result = await _cosmosClient.GetDatabase(Database)
            .CreateContainerIfNotExistsAsync(container, "/partitionkey");
        return result.Container;
    }   
    private async Task WriteDocumentsAsync<TDoc>(Container container, IEnumerable<TDoc> documents) where TDoc : BaseDocument
    {
        Console.WriteLine($"Writing {typeof(TDoc).Name}s...");
        foreach (var doc in documents)
        {
            try
            {
                await container.CreateItemAsync(doc, partitionKey: new PartitionKey(doc.PartitionKey));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        Console.WriteLine($"Finished writing {typeof(TDoc).Name}s");
    }
    private async Task MigrateReviewersAsync()
    {
        var container = _cosmosClient.GetContainer(Database, USERS);
        await WriteDocumentsAsync(container, CreateSqlContext().Reviewers.Select(x => x.ToDocument()));
    }
    private async Task MigrateProductsAsync()
    {
        var dbContext = CreateSqlContext();
        var specquery = dbContext.Specifications.Include(p => p.Product)
            .Join(
                    dbContext.SpecificationDefinitions,
                    outer => new { Key = outer.Key, Grp = outer.Product.ProductGroupId },
                    inner => new { Key = inner.Key, Grp = inner.ProductGroupId },
                    (inner, outer) => new { Specification = inner, Definition = outer }
                );
        var container = _cosmosClient!.GetContainer(Database, PRODUCTS);

        await Task.WhenAll(
            WriteDocumentsAsync(container, CreateSqlContext().Products.Select(x => x.ToDocument())),
            WriteDocumentsAsync(container, CreateSqlContext().Prices.Select(x => x.ToDocument())),
            WriteDocumentsAsync(container, CreateSqlContext().WebReviews.Select(x => x.ToDocument())),
            WriteDocumentsAsync(container, CreateSqlContext().ExpertReviews.Select(x => x.ToDocument())),
            WriteDocumentsAsync(container, CreateSqlContext().ConsumerReviews.Select(x => x.ToDocument())),
            WriteDocumentsAsync(container, specquery.Select(x => x.Specification.ToDocument(x.Definition)))
        );
    }
    private async Task MigrateMetasAsync()
    {
        var container = _cosmosClient!.GetContainer(Database, METAS);
        await Task.WhenAll(
            WriteDocumentsAsync(container, CreateSqlContext().Brands.Select(x => x.ToDocument())),
            WriteDocumentsAsync(container, CreateSqlContext().ProductGroups.Select(x => x.ToDocument()))
        );
    }
    private ShopDatabaseContext CreateSqlContext()
    {
        var bld = new DbContextOptionsBuilder<ShopDatabaseContext>();
        bld.UseSqlServer(_sqlServerSource);
        return new ShopDatabaseContext(bld.Options);
    }
}
