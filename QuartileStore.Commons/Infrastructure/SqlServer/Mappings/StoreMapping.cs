using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuartileStore.Commons.Domain.Entities.Stores;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Mappings;

public class StoreMapping : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("stores");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();
        builder.Property(x => x.Address).HasColumnName("address").IsRequired();
        
        builder
            .HasOne(store => store.Company)
            .WithMany(company => company.Stores)
            .HasForeignKey(store => store.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasMany(store => store.Products)
            .WithOne(product => product.Store)
            .HasForeignKey(product => product.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(store => new { store.Code, store.CompanyId }).IsUnique();
    }
}