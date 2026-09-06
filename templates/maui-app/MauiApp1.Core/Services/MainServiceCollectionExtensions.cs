using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1;

public static class MainServiceCollectionExtensions
{
    public static IServiceCollection AddMain(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IGreetingService, GreetingService>();
        return services;
    }
}
