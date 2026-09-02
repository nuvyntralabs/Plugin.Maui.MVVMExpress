using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>Host option to install <c>GuardedNavigator</c> without reconstructing it in app code.</summary>
public static class MvvmExpressAuthExtensions
{
    /// <summary>
    /// Wraps the registered <see cref="INavigator"/> with <c>GuardedNavigator</c>.
    /// Call after <c>UseNavigationPage</c> or <c>UseShell</c>. Do not wrap or <c>RemoveAll</c> the navigator yourself.
    /// </summary>
    /// <typeparam name="TChallenge">Login ViewModel opened when a <c>[RequiresAuth]</c> route is blocked.</typeparam>
    /// <param name="options">Host options.</param>
    public static MvvmExpressOptions UseAuth<TChallenge>(this MvvmExpressOptions options)
        where TChallenge : class, IViewModel
    {
        ArgumentNullException.ThrowIfNull(options);
        options.AuthChallengeViewModel = typeof(TChallenge);
        return options;
    }
}
