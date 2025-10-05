using Microsoft.EntityFrameworkCore;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Repositories;

internal class CompanyRepository(
    IScopedDatabaseContext scopedContext
) : BaseRepository<Company>(scopedContext.Context), ICompanyRepository
{
    public async Task<List<Company>> SelectAllAsync() =>
        await Entity.ToListAsync();

    public async Task<bool> HasStoresAsync(Guid companyId) =>
        await scopedContext.Context.Stores.AnyAsync(x => x.CompanyId == companyId);
}