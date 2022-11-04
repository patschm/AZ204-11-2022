using ACME.DataLayer.Interfaces;

namespace ACME.DataLayer.Repository.SqlServer;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShopDatabaseContext _context;
    public UnitOfWork(ShopDatabaseContext context)
    {
        _context = context;
        Brands = new BrandRepository(_context);
        Prices = new PriceRepository(_context);
        ProductGroups = new ProductGroupRepository(_context);
        Products = new ProductRepository(_context);
        Reviews = new ReviewRepository(_context);
        Reviewers = new ReviewerRepository(_context);
        Specifications = new SpecificationRepository(_context);
        SpecificationsDefinition = new SpecificationDefinitionRepository(_context);
    }
    
    public IBrandRepository Brands { get; private set; }

    public IPriceRepository Prices { get; private set; }

    public IProductGroupRepository ProductGroups { get; private set; }

    public IProductRepository Products { get; private set; }

    public IReviewRepository Reviews { get; private set; }

    public ISpecificationRepository Specifications { get; private set; }

    public ISpecificationDefinitionRepository SpecificationsDefinition { get; private set; }

    public IReviewerRepository Reviewers { get; private set; }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
