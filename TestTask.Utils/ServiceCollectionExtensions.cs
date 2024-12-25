using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TestTask.Utils;

public static class ServiceCollectionExtensions
{
    public static void RegisterConfig<T>(this IServiceCollection services, IConfiguration configuration, string key) where T : class
    {
        services.Configure<T>(options => configuration.GetSection(key).Bind(options));
        services.AddSingleton(n => n.GetService<IOptions<T>>()?.Value!);
    }
}