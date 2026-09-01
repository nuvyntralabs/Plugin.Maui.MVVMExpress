using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Pagination;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Samples.Pagination;

public sealed class PagedProductViewModel : PageViewModel
{
    public PagedProductViewModel(IProductCatalog catalog, int pageSize = 20)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        Pages = new DelegatePagedCollection<Product>(catalog.ListPageAsync, pageSize);
        LoadMoreCommand = new AsyncModelCommand(
            ct => Pages.LoadMoreAsync(ct),
            () => Pages.HasMore && !Pages.State.IsLoading);
        RefreshCommand = new AsyncModelCommand(ct => Pages.RefreshAsync(ct));
        Pages.PropertyChanged += (_, e) =>
        {
            LoadMoreCommand.NotifyCanExecuteChanged();
            if (e.PropertyName is null or nameof(PagedCollection<Product>.HasMore))
            {
                Notify(nameof(HasMore));
            }
        };
        Pages.State.PropertyChanged += (_, _) => LoadMoreCommand.NotifyCanExecuteChanged();
    }

    public DelegatePagedCollection<Product> Pages { get; }

    public int PageSize => Pages.PageSize;

    public AsyncState<IReadOnlyList<Product>> Page => Pages.State;

    public Collections.ObservableRangeCollection<Product> Items => Pages.Items;

    public bool HasMore => Pages.HasMore;

    public AsyncModelCommand LoadMoreCommand { get; }

    public AsyncModelCommand RefreshCommand { get; }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => RefreshCommand.ExecuteAsync(cancellationToken);

    /// <summary>Load once in <see cref="InitializeAsync"/>. Do not refresh from appear.</summary>
    public override Task OnAppearingAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
