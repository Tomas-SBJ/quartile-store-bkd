using Microsoft.EntityFrameworkCore;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Repositories;

internal class StoreRepository(
    IScopedDatabaseContext scopedContext
) : BaseRepository<Store>(scopedContext.Context), IStoreRepository
{
    public async Task<List<Store>> SelectAllByCompanyAsync(Guid companyId) =>
        await Entity.Where(x => x.CompanyId == companyId).ToListAsync();

    public void DeleteAsync(Store store) => Entity.Remove(store);
}