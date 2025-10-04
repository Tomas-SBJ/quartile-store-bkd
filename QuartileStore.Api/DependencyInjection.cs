using QuartileStore.Commons;

namespace QuartileStore.Api;

public static class DependencyInjection
{
    public static void AddApi(this IServiceCollection services)
    {
        services.AddCommons();
    }
}