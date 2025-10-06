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
        
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();
        builder.Property(x => x.Description).HasColumnName("descriptions").IsRequired();
        builder.Property(x => x.Price).HasColumnName("price").IsRequired();
        
        builder
            .HasOne(product => product.Store)
            .WithMany(store => store.Products)
            .HasForeignKey(product => product.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(product => new { product.Code, product.StoreId }).IsUnique();
    }
}