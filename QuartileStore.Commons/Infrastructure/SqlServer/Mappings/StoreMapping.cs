using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuartileStore.Commons.Domain.Entities.Stores;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Mappings;

public class StoreMapping : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("stores");
    }
}