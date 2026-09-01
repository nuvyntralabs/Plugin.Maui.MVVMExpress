namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>
/// Page / <c>INavigation</c> host. Distinct from Shell so both can be registered in the same app.
/// </summary>
public interface IPageNavigator : INavigator
{
    /// <summary>Window this stack belongs to.</summary>
    IWindowContext Window { get; }
}
