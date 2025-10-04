using Microsoft.EntityFrameworkCore;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Repositories;

internal class StoreRepository(
    IScopedDatabaseContext scopedContext
) : BaseRepository<Store>(scopedContext.Context), IStoreRepository
{
    public async Task<Guid> CreateAsync(Store store)
    {
        var storeCreated = await Entity.AddAsync(store);
        return storeCreated.Entity.Id;
    }

    public async Task<List<Store>> SelectAllByCompanyAsync(int companyCode) =>
        await Entity.Where(x => x.Company.Code == companyCode).ToListAsync();

    public void DeleteAsync(Store store) => Entity.Remove(store);

    public Store UpdateAsync(Store store)
    {
        var storeUpdated = Entity.Update(store);
        return storeUpdated.Entity;
    }
}