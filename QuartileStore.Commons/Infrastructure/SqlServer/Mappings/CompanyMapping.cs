using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuartileStore.Commons.Domain.Entities.Companies;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Mappings;

public class CompanyMapping : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.Code).HasColumnName("code").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();
        builder.Property(x => x.CountryCode).HasColumnName("country_code").IsRequired();
        
        builder.
            HasMany(company => company.Stores)
            .WithOne(store => store.Company)
            .HasForeignKey(store => store.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.Code).IsUnique();
    }
}