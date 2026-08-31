using Plugin.Maui.MVVMExpress.Collections;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Pagination;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Samples.Reactive;

public sealed class SearchViewModel : PageViewModel
{
    private readonly IProductCatalog _catalog;
    private readonly SearchQuery _query;
    private string? _first;
    private string? _last;

    public SearchViewModel(IProductCatalog catalog, TimeSpan? debounce = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        _catalog = catalog;
        _query = new SearchQuery(debounce ?? TimeSpan.FromMilliseconds(300));
        SearchCommand = new AsyncModelCommand(SearchNowAsync);
    }

    public TimeSpan Debounce => _query.Debounce;

    public SearchQuery QueryState => _query;

    public string Query
    {
        get => _query.Text;
        set
        {
            if (_query.Text == (value ?? ""))
            {
                return;
            }

            _query.Text = value ?? "";
            Notify(nameof(Query));
            _ = DebouncedSearchAsync();
        }
    }

    public string? First
    {
        get => _first;
        set
        {
            if (SetProperty(ref _first, value))
            {
                NotifyDependsOn(nameof(First), nameof(FullName));
            }
        }
    }

    public string? Last
    {
        get => _last;
        set
        {
            if (SetProperty(ref _last, value))
            {
                NotifyDependsOn(nameof(Last), nameof(FullName));
            }
        }
    }

    public string FullName => $"{First} {Last}".Trim();

    public AsyncState<IReadOnlyList<Product>> Results { get; } = new();

    public ObservableRangeCollection<Product> Items { get; } = [];

    public AsyncModelCommand SearchCommand { get; }

    public int SearchStarts { get; private set; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _query.Cancel();
        }

        base.Dispose(disposing);
    }

    private async Task DebouncedSearchAsync()
    {
        try
        {
            if (!await _query.WhenReadyAsync(ViewModelCancellationToken).ConfigureAwait(false))
            {
                return;
            }

            await SearchNowAsync(ViewModelCancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // A newer query replaced this one, or the ViewModel was disposed.
        }
    }

    private async Task SearchNowAsync(CancellationToken cancellationToken)
    {
        SearchStarts++;
        var page = await Results.LoadAsync(
            ct => _catalog.SearchAsync(Query, ct),
            cancellationToken).ConfigureAwait(false);
        Items.ReplaceRange(page);
    }
}
