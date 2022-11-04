using ACME.DataLayer.Entities;
using ACME.DataLayer.Repository.SqlServer;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.EntityFrameworkCore;
using Polly.Bulkhead;
using System.Drawing.Drawing2D;
using System.Text;

namespace ACME.Frontend.GremlinConsole;

internal class CosmosDb
{
    private string _sqlServerSource = @"Server=.\SQLEXPRESS;Database=ProductCatalog;Trusted_Connection=True;MultipleActiveResultSets=true;";
    private readonly GremlinClient _client;

    public GremlinClient GremlinClient { get => _client; }

    public CosmosDb(GremlinServer server)
    {
        var poolSettings = new ConnectionPoolSettings
        {
            MaxInProcessPerConnection = 10,
            PoolSize = 30,
            ReconnectionAttempts = 3,
            ReconnectionBaseDelay = TimeSpan.FromSeconds(1)
        };
        // v3.6
        _client = new GremlinClient(
            server,
            messageSerializer: new GraphSON2MessageSerializer(),
            connectionPoolSettings: poolSettings);
        // v3.4
        //_client = new GremlinClient(
        //    server, 
        //    new GraphSON2Reader(), 
        //    new GraphSON2Writer(), 
        //    GremlinClient.GraphSON2MimeType);
    }

    public async Task PopulateDatabaseAsync()
    {
        await MigrateBrandsAsync();
        await MigrateProductGroupsAsync();
        await MigrateReviewersAsync();
        await MigrateProductsAsync();
    }


    private async Task MigrateBrandsAsync()
    {
        Console.WriteLine("Creating Brands...");
        var dbContext = CreateSqlContext();
        foreach (var brand in dbContext.Brands)
        {
            await CreateBrandVertexAsync(brand);
        }
    }
    private async Task MigrateProductGroupsAsync()
    {
        Console.WriteLine("Creating ProductGroups...");
        var dbContext = CreateSqlContext();
        foreach (var group in dbContext.ProductGroups)
        {
            await CreateProductGroupVertexAsync(group);
        }
    }
    private async Task MigrateProductsAsync()
    {
        Console.WriteLine("Creating Products...");
        var dbContext = CreateSqlContext();
        foreach (var product in dbContext.Products
            .Include(p => p.Specifications)
            .Include(p => p.Reviews))
        {
            await CreateProductVertexAsync(product);
            await CreateReviewsAsync(product);
            //await CreateSpecificationsAsync(product);
        }
    }
    private async Task MigrateReviewersAsync()
    {
        Console.WriteLine("Creating Reviewers...");
        var dbContext = CreateSqlContext();
        foreach (var user in dbContext.Reviewers)
        {
            await CreateReviewerVertexAsync(user);
        }
    }

    private async Task CreateReviewerVertexAsync(Reviewer user)
    {
        var cmd = $@"g.AddV('reviewer')
                                .property('id', 'us_{user.Id}')
                                .property('name', '{user.Name?.Parse()}')
                                .property('username', '{user.UserName?.Parse()}')
                                .property('email', '{user.Email?.Parse()}')
                                .property('partitionkey', 'us_{user.Id}')";

        await SubmitAsync(cmd);
    }
    private Task CreateSpecificationsAsync(Product product)
    {
        throw new NotImplementedException();
    }

    private async Task CreateReviewsAsync(Product product)
    {
        foreach (var review in product.Reviews)
        {
            var bld = new StringBuilder();
            bld.Append($"g.AddV('review')");
            bld.Append($".property('id', 'rv_{review.Id}')");
            bld.Append($".property('score', '{review.Score}')");
            bld.Append($".property('text', '{review.Text?.Parse()}')");
            bld.Append($".property('type', '{review.ReviewType}')");
            bld.Append($".property('partitionkey', 'rv_{review.Id}')");

            await SubmitAsync(bld.ToString());
            await SubmitAsync($"g.V('pr_{product.Id}').addE('reviews').to(g.V('rv_{review.Id}'))");
            await SubmitAsync($"g.V('rv_{review.Id}').addE('reviewer').to(g.V('us_{review.ReviewerId}'))");
            await SubmitAsync($"g.V('us_{review.ReviewerId}').addE('reviews').to(g.V('rv_{review.Id}'))");
        }
    }

    private async Task CreateBrandVertexAsync(Brand brand)
    {
       // Fluent Api not supported yet. (2022)
        //var remote = new DriverRemoteConnection(GremlinClient);
        //var g = AnonymousTraversalSource.Traversal().WithRemote(remote);
        //g.AddV("brand")
        //    .Property("id", $"br_{brand.Id}")
        //    .Property("name", brand.Name)
        //    .Property("website", brand.Website)
        //    .Property("partitionkey", $"br_{brand.Id}")
        //    .Next();

        var cmd = $@"g.AddV('brand')
                                .property('id', 'br_{brand.Id}')
                                .property('name', '{brand.Name?.Parse()}')
                                .property('website', '{brand.Website?.Parse()}')
                                .property('partitionkey', 'br_{brand.Id}')";

        await SubmitAsync(cmd);
    }
    private async Task CreateProductGroupVertexAsync(ProductGroup group)
    {
        // Not Supported yet!
        //var remote = new DriverRemoteConnection(GremlinClient);
        //var g = AnonymousTraversalSource.Traversal().WithRemote(remote);
        //g.AddV("productgroup")
        //    .Property("id", $"gr_{group.Id}")
        //    .Property("name", group.Name)
        //    .Property("website", group.Image)
        //    .Property("partitionkey", $"gr_{group.Id}")
        //    .Next();

        var cmd = $@"g.AddV('productgroup')
                                .property('id', 'pg_{group.Id}')
                                .property('name', '{group.Name?.Parse()}')
                                .property('image', '{group.Image?.Parse()}')
                                .property('partitionkey', 'gr_{group.Id}')";
        await SubmitAsync(cmd);
    }
    private async Task CreateProductVertexAsync(Product product)
    {
        // Not Supported yet!
        //var remote = new DriverRemoteConnection(GremlinClient);
        //var g = AnonymousTraversalSource.Traversal().WithRemote(remote);
        //g.AddV("product")
        //    .Property("id", $"pr_{product.Id}")
        //    .Property("name", product.Name)
        //    .Property("website", product.Image)
        //    .Property("partitionkey", $"pr_{product.Id}")
        //    .AddE("brand")
        //    .To(g.V($"br_{product.BrandId}"))
        //    .AddE("productgroup")
        //    .To(g.V($"pg_{product.ProductGroupId}"))
        //    .Next();

        var cmd = $@"g.AddV('product')
                                .property('id', 'pr_{product.Id}')
                                .property('name', '{product.Name?.Parse()}')
                                .property('image', '{product.Image?.Parse()}')
                                .property('partitionkey', 'pr_{product.Id}')";
        await SubmitAsync(cmd);
        await SubmitAsync($"g.V('pr_{product.Id}').addE('brand').to(g.V('br_{product.BrandId}'))");
        await SubmitAsync($"g.V('pr_{product.Id}').addE('productgroup').to(g.V('pg_{product.ProductGroupId}'))");
        await SubmitAsync($"g.V('br_{product.BrandId}').addE('products').to(g.V('pr_{product.Id}'))");
        await SubmitAsync($"g.V('pg_{product.ProductGroupId}').addE('products').to(g.V('pr_{product.Id}'))");
    }
    private async Task SubmitAsync(string cmd)
    {
        try
        {
            await GremlinClient.SubmitAsync(cmd);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in command {cmd}");
            Console.WriteLine(e.Message);
        }
    }
    private ShopDatabaseContext CreateSqlContext()
    {
        var bld = new DbContextOptionsBuilder<ShopDatabaseContext>();
        bld.UseSqlServer(_sqlServerSource);
        return new ShopDatabaseContext(bld.Options);
    }
}
