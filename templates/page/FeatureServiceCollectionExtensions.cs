using Microsoft.Extensions.DependencyInjection;

namespace MauiApp1;

public static class FeatureServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IFeatureService"/> and <see cref="FeaturePage"/>.
    /// Also map the route: <c>.Map&lt;FeatureViewModel, FeaturePage&gt;("feature")</c>.
    /// </summary>
    public static IServiceCollection AddFeature(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IFeatureService, FeatureService>();
        services.AddTransient<FeaturePage>();
        return services;
    }
}
