using Microsoft.EntityFrameworkCore;
using QuartileStore.Commons.Domain.Entities.Products;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Repositories;

internal class ProductRepository(
    IScopedDatabaseContext scopedContext
) : BaseRepository<Product>(scopedContext.Context), IProductRepository
{
    public async Task<Product?> SelectOneWithHierarchyAsync(int code, int storeCode, int companyCode) =>
        await Entity
            .Include(product => product.Store)
            .ThenInclude(store => store.Company)
            .FirstOrDefaultAsync(x => 
                x.Code == code &&
                x.Store.Code == storeCode &&
                x.Store.Company.Code == companyCode);

    public async Task<IEnumerable<Product>> SelectAllWithHierarchyAsync(int storeCode, int companyCode) =>
        await Entity
            .Include(product => product.Store)
            .ThenInclude(store => store.Company)
            .Where(x => x.Store.Code == storeCode && x.Store.Company.Code == companyCode)
            .ToListAsync();
}