using System.Linq.Expressions;
using QuartileStore.Commons.Domain.Entities.Abstracts;

namespace QuartileStore.Commons.Domain.Entities.Stores;

public interface IStoreRepository : IBaseRepository<Store>
{
    Task<List<Store>> SelectAllByCompanyCodeAsync(int companyCode);
    Task<Store?> SelectOneWithCompanyAsync(int storeCode, int companyCode);
}