using QuartileStore.Commons.Domain.Entities.Abstracts;

namespace QuartileStore.Commons.Domain.Entities.Companies;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<List<Company>> SelectAllAsync();
    Task<bool> HasStoresAsync(Guid companyId);
}