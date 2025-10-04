using System.Linq.Expressions;
using QuartileStore.Commons.Domain.Entities.Abstracts;

namespace QuartileStore.Commons.Domain.Entities.Stores;

public interface IStoreRepository : IBaseRepository<Store>
{
    Task<Guid> CreateAsync(Store store);
    Task<List<Store>> SelectAllByCompanyAsync(int companyCode);
    void DeleteAsync(Store store);
    Store UpdateAsync(Store store);
}