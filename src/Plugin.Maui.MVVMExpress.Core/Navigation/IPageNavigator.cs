using Plugin.Maui.MVVMExpress.ComponentModel;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>
/// Page / <c>INavigation</c> host. Distinct from Shell so both can be registered in the same app.
/// </summary>
public interface IPageNavigator : INavigator
{
    /// <summary>Window this stack belongs to.</summary>
    IWindowContext Window { get; }

    /// <summary>
    /// Replaces the window root with <typeparamref name="TViewModel"/> (login → host).
    /// Same contract as <see cref="INavigator.ResetAsync{TViewModel}"/>.
    /// </summary>
    Task<Result> ReplaceRootAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => ResetAsync<TViewModel>(cancellationToken);
}
