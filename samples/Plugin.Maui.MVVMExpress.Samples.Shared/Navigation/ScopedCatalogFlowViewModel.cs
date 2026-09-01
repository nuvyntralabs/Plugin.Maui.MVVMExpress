using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Composition;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Samples.Crud;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

/// <summary>
/// Page-scope session: each push resolves a ViewModel from a new DI scope; back disposes that page.
/// </summary>
public sealed class ScopedCatalogFlowViewModel : PageViewModel
{
    private readonly IViewModelScopeFactory _scopes;
    private readonly Stack<(IViewModelScope Scope, IViewModel ViewModel)> _stack = new();
    private IViewModel? _current;

    public ScopedCatalogFlowViewModel(IViewModelScopeFactory scopes)
    {
        ArgumentNullException.ThrowIfNull(scopes);
        _scopes = scopes;
        OpenListCommand = new AsyncModelCommand(OpenListAsync, () => Current is not ProductListViewModel);
        OpenDetailsCommand = new AsyncModelCommand<int>(OpenDetailsAsync, id => id > 0 && Current is ProductListViewModel);
        OpenSecondCommand = new AsyncModelCommand(ct => OpenDetailsAsync(2, ct), () => Current is ProductListViewModel);
        GoBackCommand = new ModelCommand(GoBack, () => CanGoBack);
    }

    public IViewModel? Current
    {
        get => _current;
        private set
        {
            if (SetProperty(ref _current, value))
            {
                Notify(nameof(List));
                Notify(nameof(Details));
                Notify(nameof(Depth));
                Notify(nameof(CanGoBack));
                Notify(nameof(CurrentTitle));
                OpenListCommand.NotifyCanExecuteChanged();
                OpenDetailsCommand.NotifyCanExecuteChanged();
                OpenSecondCommand.NotifyCanExecuteChanged();
                GoBackCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public ProductListViewModel? List => Current as ProductListViewModel;

    public ProductDetailsViewModel? Details => Current as ProductDetailsViewModel;

    public int Depth => _stack.Count;

    public bool CanGoBack => _stack.Count > 0;

    public string CurrentTitle => Current switch
    {
        ProductListViewModel => "Catalog",
        ProductDetailsViewModel details => $"Product {details.ProductId}",
        _ => "Empty"
    };

    public AsyncModelCommand OpenListCommand { get; }

    public AsyncModelCommand<int> OpenDetailsCommand { get; }

    public AsyncModelCommand OpenSecondCommand { get; }

    public ModelCommand GoBackCommand { get; }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => OpenListCommand.ExecuteAsync(cancellationToken);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            while (_stack.Count > 0)
            {
                PopCore();
            }
        }

        base.Dispose(disposing);
    }

    private async Task OpenListAsync(CancellationToken cancellationToken)
    {
        if (Current is ProductListViewModel)
        {
            return;
        }

        var scope = _scopes.CreatePageScope();
        var list = scope.GetViewModel<ProductListViewModel>();
        _stack.Push((scope, list));
        Current = list;
        await list.InitializeAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task OpenDetailsAsync(int productId, CancellationToken cancellationToken)
    {
        var scope = _scopes.CreatePageScope();
        var details = scope.GetViewModel<ProductDetailsViewModel>();
        details.Accept(new ProductDetailsArgs(productId));
        _stack.Push((scope, details));
        Current = details;
        await details.InitializeAsync(cancellationToken).ConfigureAwait(false);
    }

    private void GoBack()
    {
        if (_stack.Count == 0)
        {
            return;
        }

        PopCore();
        Current = _stack.Count == 0 ? null : _stack.Peek().ViewModel;
    }

    private void PopCore()
    {
        var (scope, viewModel) = _stack.Pop();
        viewModel.Dispose();
        scope.Dispose();
    }
}
