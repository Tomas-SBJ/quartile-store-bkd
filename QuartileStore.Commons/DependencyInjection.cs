using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuartileStore.Commons.Domain.Entities.Abstracts;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Products;
using QuartileStore.Commons.Domain.Entities.Stores;
using QuartileStore.Commons.Infrastructure.SqlServer.Contexts;
using QuartileStore.Commons.Infrastructure.SqlServer.Repositories;
using QuartileStore.Commons.Infrastructure.Transactions;
using QuartileStore.Commons.Services;
using QuartileStore.Commons.Services.Contracts;

namespace QuartileStore.Commons;

public static class DependencyInjection
{
    public static void AddCommons(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddRepositories()
            .AddServices()
            .AddSqlServer(configuration);
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }

    private static void AddSqlServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<QuartileDatabaseContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IScopedDatabaseContext, ScopedDatabaseContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}