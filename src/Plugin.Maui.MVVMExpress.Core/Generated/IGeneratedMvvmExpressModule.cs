using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Auth;

namespace Plugin.Maui.MVVMExpress.Generated;

/// <summary>AOT-safe generated ViewModel / route / auth registrations.</summary>
public interface IGeneratedMvvmExpressModule
{
    /// <summary>Registers generated ViewModels and views.</summary>
    void AddViewModels(IServiceCollection services);

    /// <summary>Applies <c>[Route]</c> mappings.</summary>
    void ApplyRoutes(Action<Type, string> map);

    /// <summary>Generated <c>[RequiresAuth]</c> / <c>[RequiresRole]</c> policy.</summary>
    INavigationAuthPolicy AuthPolicy { get; }
}

/// <summary>Collects <see cref="IGeneratedMvvmExpressModule"/> instances emitted by the source generator.</summary>
public static class GeneratedRegistrationHooks
{
    private static readonly List<IGeneratedMvvmExpressModule> Modules = [];

    /// <summary>Adds a generated module. Called from a <c>ModuleInitializer</c>.</summary>
    public static void Add(IGeneratedMvvmExpressModule module)
    {
        ArgumentNullException.ThrowIfNull(module);
        lock (Modules)
        {
            Modules.Add(module);
        }
    }

    /// <summary>Registered modules (copy).</summary>
    public static IReadOnlyList<IGeneratedMvvmExpressModule> Snapshot()
    {
        lock (Modules)
        {
            return [.. Modules];
        }
    }

    /// <summary>Applies every registered module to <paramref name="services"/>.</summary>
    public static void Apply(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        foreach (var module in Snapshot())
        {
            module.AddViewModels(services);
        }
    }
}
