using ACME.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace ACME.DataLayer.Repository.SqlServer;
public partial class ShopDatabaseContext : DbContext
{
    private readonly DbCommandInterceptor? _commandInterceptor;
    public ShopDatabaseContext(DbContextOptions<ShopDatabaseContext> options)
        : base (options)
    {
    }
    public ShopDatabaseContext(DbContextOptions<ShopDatabaseContext> options, DbCommandInterceptor commandInterceptor)
        : base(options)
    {
        _commandInterceptor = commandInterceptor;
    }
   
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Price> Prices => Set<Price>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Reviewer> Reviewers => Set<Reviewer>();
    public DbSet<Specification> Specifications => Set<Specification>();
    public DbSet<SpecificationDefinition> SpecificationDefinitions => Set<SpecificationDefinition>();
    public DbSet<ConsumerReview> ConsumerReviews => Set<ConsumerReview>();
    public DbSet<ExpertReview> ExpertReviews => Set<ExpertReview>();
    public DbSet<WebReview> WebReviews => Set<WebReview>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       if (_commandInterceptor != null)
            optionsBuilder.AddInterceptors(_commandInterceptor);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Core");
        
        modelBuilder.Entity<Brand>(e => {
            e.Property(p => p.Website)
                .HasMaxLength(1024);
            e.Property(p=>p.Name)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
        });
        modelBuilder.Entity<Price>(e => {
            e.Property(p => p.ShopName)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            e.HasOne(p => p.Product)
                .WithMany(p => p.Prices)
                .HasForeignKey(p => p.ProductId);
                
        });
        modelBuilder.Entity<Product>(e => {
            e.Navigation(p => p.Brand).AutoInclude();
            e.Navigation(p => p.ProductGroup).AutoInclude();
            e.Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.Image)
               .HasMaxLength(1024);
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            e.HasOne(p => p.Brand)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.BrandId);
            e.HasOne(p => p.ProductGroup)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ProductGroupId);
        });
        modelBuilder.Entity<ProductGroup>(e => {
            e.Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.Image)
               .HasMaxLength(1024);
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
        });
        modelBuilder.Entity<Review>(e =>
        {
            e.Navigation(p => p.Reviewer).AutoInclude();
            e.Property(p => p.ReviewType)
                .HasConversion<int>();
            e.Property(p => p.Timestamp)
               .IsRowVersion()
               .IsConcurrencyToken();
            e.HasOne(p => p.Reviewer)
                .WithMany(p => p.Reviews);
            e.HasOne(p => p.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(p => p.ProductId);
            e.HasDiscriminator(p => p.ReviewType)
               .HasValue<Review>(ReviewType.Generic)
               .HasValue<ConsumerReview>(ReviewType.Consumer)
               .HasValue<ExpertReview>(ReviewType.Expert)
               .HasValue<WebReview>(ReviewType.Web);
        });
        modelBuilder.Entity<Reviewer>(e => {
            e.Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.UserName)
               .HasMaxLength(255)
               .IsRequired();
            e.Property(p => p.Email)
               .HasMaxLength(255)
               .IsRequired();
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
        });
        modelBuilder.Entity<Specification>(e => {
            e.Property(p => p.Key)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            e.HasOne(p => p.Product)
                .WithMany(p => p.Specifications)
                .HasForeignKey(p => p.ProductId);
        });
        modelBuilder.Entity<SpecificationDefinition>(e => {
            e.Property(p => p.Key)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.Name)
               .HasMaxLength(255)
               .IsRequired();
            e.Property(p => p.Unit)
               .HasMaxLength(127);
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            e.HasOne(p => p.ProductGroup)
                .WithMany(p => p.SpecificationDefinitions)
                .HasForeignKey(p => p.ProductGroupId);
        });
        modelBuilder.Entity<ExpertReview>(e => {
            e.Property(p => p.Organization)
                .HasMaxLength(512);
        });
        modelBuilder.Entity<WebReview>(e => {
            e.Property(p => p.ReviewUrl)
                .HasMaxLength(1024);
        });
    }
}
