using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace ACME.Frontend.GremlinConsole;

internal class Program
{
    // az group create --name cosmos --location westeurope
    // az cosmosdb create --name ps-gremlins --resource-group cosmos --capabilities EnableGremlin --default-consistency-level Eventual --locations regionName=westeurope 
    // az cosmosdb gremlin database create --account-name ps-gremlins --resource-group cosmos --name products
    // az cosmosdb gremlin graph create --account-name ps-gremlins --resource-group cosmos --database-name products --name datagraph -p "/partitionkey" --throughput 4000 
    public const string ACCOUNT = "ps-gremlins";
    public const string HOST = $"{ACCOUNT}.gremlin.cosmosdb.azure.com";
    public const string KEY = "APaA8XSYUIggOfG0ELRVZ9NpmGQp9wq4DapZtGH5LQ35cX9tf5Zopa9m6q10fi460cf3sX3qu0zPVaSO4I9FwQ==";
    public const string DATABASE = "products";
    public const string GRAPH = "datagraph";
    public const string USERNAME = $"/dbs/{DATABASE}/colls/{GRAPH}";

    static async Task Main(string[] args)
    {
        //await PrepareDatabaseAsync();
        await ReadDataAsync();
        Console.WriteLine("Done");
        Console.ReadLine();
    }

    private static async Task ReadDataAsync()
    {
        var server = new GremlinServer(HOST, 443, enableSsl: true, username: USERNAME, password: KEY);
        var poolSettings = new ConnectionPoolSettings
        {
            MaxInProcessPerConnection = 10,
            PoolSize = 30,
            ReconnectionAttempts = 3,
            ReconnectionBaseDelay = TimeSpan.FromSeconds(1)
        };
        var client = new GremlinClient(
            server,
            messageSerializer: new GraphSON2MessageSerializer(),
            connectionPoolSettings: poolSettings);

        // Not supported yet! (2022)
        //var remote = new DriverRemoteConnection(client);
        //var g = AnonymousTraversalSource.Traversal().WithRemote(remote);
        //var v = g.V().HasId("pr_1").Next();

        // Return products of a certain barnd
        var cmdBrand = "g.V().hasId('br_1').out('products')";
        var cmdBrandResult = await client.SubmitAsync<dynamic>(cmdBrand);
        Console.WriteLine(JsonConvert.SerializeObject(cmdBrandResult));
        Console.WriteLine("==========================================");
        // Read Product and brand
        var cmd = "g.V().hasId('pr_1').as('produkt').out('brand').as('merk').select('produkt', 'merk')";
        var cmdResult = await client.SubmitAsync<dynamic>(cmd);
        var data = JsonConvert.SerializeObject(cmdResult.First());
        Console.WriteLine(data);
        Console.WriteLine("==========================================");
        // Reviews with score greater than 3
        var cmdFilter = "g.V().hasLabel('review').has('score', gt('3')).values('score')";
        var cmdFilterResult = await client.SubmitAsync<dynamic>(cmdFilter);
        Console.WriteLine(JsonConvert.SerializeObject(cmdFilterResult));
        Console.WriteLine("==========================================");
    }

    private static async Task PrepareDatabaseAsync()
    {
        var server = new GremlinServer(HOST, 443, enableSsl: true, username: USERNAME, password: KEY);
        var db = new CosmosDb(server);
        await db.PopulateDatabaseAsync();
    }
}