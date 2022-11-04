using ACME.Backend.Tools.Converters;
using ACME.DataLayer.Documents;
using ACME.DataLayer.Repository.SqlServer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace ACME.Frontend.ChangeTrackerConsole;

internal class CosmosDb
{
    public const string METAS = "metas";
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
        await MigrateMetasAsync();
    }
    private async Task CreateDatabaseAsync()
    {
        Console.WriteLine($"Creating Database {_database}...");
        await _cosmosClient.CreateDatabaseIfNotExistsAsync(_database, throughput:4000);
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
    private async Task MigrateMetasAsync()
    {
        var container = _cosmosClient!.GetContainer(_database, METAS);
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
