using System.Linq.Expressions;
using QuartileStore.Commons.Domain.Entities.Abstracts;

namespace QuartileStore.Commons.Domain.Entities.Stores;

public interface IStoreRepository : IBaseRepository<Store>
{
    Task<List<Store>> SelectAllByCompanyAsync(Guid companyId);
    void DeleteAsync(Store store);
}