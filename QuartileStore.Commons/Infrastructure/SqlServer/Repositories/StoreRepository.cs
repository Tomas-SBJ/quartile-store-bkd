using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Repositories;

internal class StoreRepository(
    IScopedDatabaseContext scopedContext
) : BaseRepository<Store>(scopedContext.Context), IStoreRepository
{
    public async Task<List<Store>> SelectAllByCompanyCodeAsync(int companyCode) =>
        await Entity
            .Include(x => x.Company)
            .Where(x => x.Company.Code == companyCode)
            .ToListAsync();

    public async Task<Store?> SelectOneWithCompanyAsync(Expression<Func<Store, bool>> predicate) =>
        await Entity.Include(x => x.Company).FirstOrDefaultAsync(predicate);
}