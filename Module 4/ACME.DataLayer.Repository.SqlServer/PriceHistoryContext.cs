using ACME.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ACME.DataLayer.Repository.SqlServer;
public partial class PriceHistoryContext : DbContext
{
    public PriceHistoryContext(DbContextOptions<PriceHistoryContext> options)
        : base(options)
    {
    }
    public DbSet<Price> Prices => Set<Price>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Core");

        modelBuilder.Entity<Price>(e => {
            e.ToTable("PriceHistory");
            e.Property(p => p.Id).ValueGeneratedNever();
            e.Property(p => p.ShopName)
                .HasMaxLength(255)
                .IsRequired();
            e.Property(p => p.ProductId).IsRequired(false);
            e.Property(p => p.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken();
        });
    }
}
