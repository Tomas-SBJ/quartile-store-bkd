using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;

namespace QuartileStore.Commons.Infrastructure.SqlServer.Repositories;

internal class CompanyRepository(
    IScopedDatabaseContext scopedContext
) : BaseRepository<Company>(scopedContext.Context), ICompanyRepository
{
}