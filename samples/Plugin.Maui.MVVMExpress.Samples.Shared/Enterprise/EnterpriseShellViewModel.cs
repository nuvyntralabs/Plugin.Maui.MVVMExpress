using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Busy;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Connectivity;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Messaging;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Auth;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.State;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Samples.Enterprise;

public sealed class EnterpriseShellViewModel : PageViewModel
{
    private readonly IProductCatalog _catalog;
    private readonly IAuthState _auth;
    private readonly IConnectivityProbe _connectivity;
    private readonly IErrorSink _errors;
    private readonly IBusyGate _busy;
    private readonly IMainThread _mainThread;
    private readonly IDisposable _subscription;
    private int _notices;

    public EnterpriseShellViewModel(
        IProductCatalog catalog,
        IAuthState auth,
        IConnectivityProbe connectivity,
        IMessageHub hub,
        IErrorSink errors,
        IBusyGate busy,
        IMainThread mainThread,
        INavigator navigator,
        IDialogs? dialogs = null)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(auth);
        ArgumentNullException.ThrowIfNull(connectivity);
        ArgumentNullException.ThrowIfNull(hub);
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(busy);
        ArgumentNullException.ThrowIfNull(mainThread);
        ArgumentNullException.ThrowIfNull(navigator);
        _catalog = catalog;
        _auth = auth;
        _connectivity = connectivity;
        _errors = errors;
        _busy = busy;
        _mainThread = mainThread;
        RefreshCommand = new AsyncModelCommand(RefreshAsync);
        OpenSecureCommand = new AsyncModelCommand(ct => navigator.NavigateToAsync<SecureHomeViewModel>(ct));
        _subscription = hub.Subscribe<EnterpriseShellViewModel, ProductsChanged>(
            this,
            static (vm, msg) => vm.OnProductsChanged(msg),
            weak: true);
    }

    public AsyncState<IReadOnlyList<Product>> Products { get; } = new();

    public bool IsOnline => _connectivity.IsOnline;

    public bool IsAuthenticated => _auth.IsAuthenticated;

    public int Notices
    {
        get => _notices;
        private set => SetProperty(ref _notices, value);
    }

    public AsyncModelCommand RefreshCommand { get; }

    public AsyncModelCommand OpenSecureCommand { get; }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => RefreshCommand.ExecuteAsync(cancellationToken);

    public override Task OnDisappearingAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscription.Dispose();
        }

        base.Dispose(disposing);
    }

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        if (!_connectivity.IsOnline)
        {
            Status = ViewModelStatus.Offline;
            await _errors.HandleAsync(new Op.ErrorInfo("E_OFFLINE", "No network"), cancellationToken).ConfigureAwait(false);
            return;
        }

        using (_busy.Enter())
        {
            try
            {
                await _mainThread.InvokeAsync(
                    () => Products.LoadAsync(_catalog.ListAsync, cancellationToken),
                    cancellationToken).ConfigureAwait(false);
                Status = Products.HasError ? ViewModelStatus.Error : ViewModelStatus.Success;
            }
            catch (OperationCanceledException)
            {
                Status = ViewModelStatus.Cancelled;
                throw;
            }
            catch (Exception ex)
            {
                Status = ViewModelStatus.Error;
                await _errors.HandleAsync(new Op.ErrorInfo("E_LOAD", ex.Message, ex), cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private void OnProductsChanged(ProductsChanged message)
    {
        Notices = message.Count;
    }
}
