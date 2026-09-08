using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Generated;

namespace Plugin.Maui.MVVMExpress.Samples.Modules;

/// <summary>Phase 9: a feature assembly registers its own ViewModels.</summary>
public sealed class CatalogModule : IModule
{
    /// <inheritdoc />
    public void Configure(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<CatalogModuleMarker>();
        services.TryAddTransient<GeneratedCatalogViewModel>();
    }
}

/// <summary>Proof that <see cref="CatalogModule"/> ran.</summary>
public sealed class CatalogModuleMarker
{
    /// <summary>Feature key registered by the module.</summary>
    public string Feature { get; } = "generated-catalog";
}
