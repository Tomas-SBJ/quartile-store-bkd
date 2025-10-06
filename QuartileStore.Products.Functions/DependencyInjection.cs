using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuartileStore.Commons;

namespace QuartileStore.Products.Functions;

public static class DependencyInjection
{
    public static IServiceCollection AddProductsFunctions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommons(configuration);

        return services;
    }
}