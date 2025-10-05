using Microsoft.EntityFrameworkCore;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Products;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.SqlServer.Mappings;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

public class QuartileDatabaseContext(DbContextOptions<QuartileDatabaseContext> options) : DbContext(options)
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Store> Stores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("quartile");

        modelBuilder.ApplyConfiguration(new CompanyMapping());
        modelBuilder.ApplyConfiguration(new StoreMapping());

        base.OnModelCreating(modelBuilder);
    }
}