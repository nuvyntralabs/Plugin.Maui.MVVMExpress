using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Caching;
using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Auth;
using Plugin.Maui.MVVMExpress.Samples.Basic;
using Plugin.Maui.MVVMExpress.Samples.Crud;
using Plugin.Maui.MVVMExpress.Samples.Enterprise;
using Plugin.Maui.MVVMExpress.Samples.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Offline;
using Plugin.Maui.MVVMExpress.Generated;
using Plugin.Maui.MVVMExpress.Samples.Generated;
using Plugin.Maui.MVVMExpress.Samples.Pagination;
using Plugin.Maui.MVVMExpress.Samples.Reactive;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Validation;

namespace Plugin.Maui.MVVMExpress.Samples;

public static class SampleServiceCollectionExtensions
{
    public static IServiceCollection AddMvvmExpressSamples(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddSingleton<IErrorSink, RecordingErrorSink>();
        services.AddSingleton<IAuthState, InMemoryAuthState>();
        services.AddSingleton<IValidator>(_ => DataAnnotationsValidator.Instance);
        services.AddMvvmExpress();

        services.RemoveAll<INavigator>();
        services.RemoveAll<IPageNavigator>();
        services.AddSingleton<InMemoryNavigator>(_ =>
        {
            var navigator = new InMemoryNavigator()
                .Map<ProductListViewModel>("products")
                .Map<ProductDetailsViewModel>("details");
            MvvmExpressGeneratedRegistrations.ApplyRoutes((type, route) => navigator.Map(type, route));
            return navigator;
        });
        services.AddSingleton<INavigator>(sp => new GuardedNavigator(
            sp.GetRequiredService<InMemoryNavigator>(),
            sp.GetRequiredService<IAuthState>(),
            MvvmExpressGeneratedRegistrations.AuthPolicy,
            typeof(SecureHomeViewModel),
            typeof(EnterpriseShellViewModel)));
        services.AddSingleton<IPageNavigator>(_ => new InMemoryNavigator(window: new WindowContext("page-stack"))
            .Map<PageStackViewModel>("stack")
            .Map<PageStackItemViewModel>("stack-item"));

        services.AddSingleton<InMemoryProductCatalog>();
        services.AddSingleton<IProductCatalog>(sp =>
        {
            var inner = sp.GetRequiredService<InMemoryProductCatalog>();
            var cache = sp.GetRequiredService<ICache>();
            return new CacheFirstCatalog(inner, cache);
        });

        services.AddTransient<CounterViewModel>();
        services.AddTransient<ProductListViewModel>();
        services.AddTransient<ProductEditViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<ProductDetailsViewModel>();
        services.AddTransient<PageStackViewModel>();
        services.AddTransient<PageStackItemViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<SecureHomeViewModel>();
        services.AddTransient<OfflineCatalogViewModel>();
        services.AddTransient<PagedProductViewModel>();
        services.AddTransient<SearchViewModel>();
        services.AddTransient<EnterpriseShellViewModel>();
        services.AddTransient<ScopedCatalogFlowViewModel>();
        services.AddTransient<GeneratedCatalogViewModel>();
        services.AddGeneratedViewModels();
        return services;
    }
}
