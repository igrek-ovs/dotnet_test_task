using Microsoft.Extensions.DependencyInjection;

namespace TestTask.Services;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<MarketService>();
    }
}