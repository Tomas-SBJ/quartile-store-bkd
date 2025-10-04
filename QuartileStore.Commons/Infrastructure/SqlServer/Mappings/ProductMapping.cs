using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuartileStore.Commons.Domain.Entities.Products;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Mappings;

public class ProductMapping : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        
        builder.HasKey(x => x.Id);
    }
}