using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Windows;

/// <summary>
/// Phase 9: two <see cref="IWindowContext"/> keys. Navigate on A must not change B's stack.
/// Default apps stay single-window; this proves the registry.
/// </summary>
[RegisterViewModel]
[Route("two-window")]
public sealed class TwoWindowDemoViewModel : ViewModel
{
    private readonly WindowNavigatorRegistry _registry = new();
    private readonly InMemoryNavigator _windowA = new(window: new WindowContext("A"));
    private readonly InMemoryNavigator _windowB = new(window: new WindowContext("B"));

    /// <summary>Creates isolated A/B navigators.</summary>
    public TwoWindowDemoViewModel()
    {
        _windowA.Map<HomeViewModel>("home");
        _windowB.Map<HomeViewModel>("home");
        _registry.Register(new WindowContext("A"), _windowA);
        _registry.Register(new WindowContext("B"), _windowB);
        NavigateACommand = new AsyncModelCommand(ct =>
        {
            _registry.CurrentWindow = new WindowContext("A");
            return _registry.GetCurrent().NavigateToAsync<HomeViewModel>(ct);
        });
        NavigateBCommand = new AsyncModelCommand(ct =>
        {
            _registry.CurrentWindow = new WindowContext("B");
            return _registry.GetCurrent().NavigateToAsync<HomeViewModel>(ct);
        });
    }

    /// <summary>Window A navigator.</summary>
    public InMemoryNavigator WindowA => _windowA;

    /// <summary>Window B navigator.</summary>
    public InMemoryNavigator WindowB => _windowB;

    /// <summary>Registry under test.</summary>
    public IWindowNavigatorRegistry Registry => _registry;

    /// <summary>Pushes Home on window A.</summary>
    public AsyncModelCommand NavigateACommand { get; }

    /// <summary>Pushes Home on window B.</summary>
    public AsyncModelCommand NavigateBCommand { get; }
}
