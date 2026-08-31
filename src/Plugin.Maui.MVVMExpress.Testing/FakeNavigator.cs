using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Testing;

/// <summary>In-memory <see cref="INavigator"/> for unit tests.</summary>
public sealed class FakeNavigator : InMemoryNavigator
{
    /// <summary>Creates a fake navigator.</summary>
    /// <param name="canLeave">Optional dirty-page guard.</param>
    public FakeNavigator(Func<Type, bool>? canLeave = null)
        : base(canLeave)
    {
    }
}
