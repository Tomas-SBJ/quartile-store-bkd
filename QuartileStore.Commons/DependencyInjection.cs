using Microsoft.Extensions.DependencyInjection;
using QuartileStore.Commons.Domain.Entities.Abstracts;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Products;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.SqlServer.Repositories;
using QuartileStore.Commons.Services;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Commons;

public static class DependencyInjection
{
    public static void AddCommons(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddServices();
    }
    
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IStoreService, StoreService>();
    }
}