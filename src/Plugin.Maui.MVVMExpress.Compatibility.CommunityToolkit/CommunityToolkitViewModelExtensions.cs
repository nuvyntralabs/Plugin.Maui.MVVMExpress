using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit;

/// <summary>
/// Registers a CommunityToolkit <c>ObservableObject</c> ViewModel so it can inject
/// <c>INavigator</c> and <c>IDialogs</c> without rewriting to <c>PageViewModel</c>.
/// </summary>
public static class CommunityToolkitViewModelExtensions
{
    /// <summary>Adds <typeparamref name="TViewModel"/> as a transient ViewModel.</summary>
    public static IServiceCollection AddCommunityToolkitViewModel<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel>(
        this IServiceCollection services)
        where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddTransient<TViewModel>();
        return services;
    }
}
