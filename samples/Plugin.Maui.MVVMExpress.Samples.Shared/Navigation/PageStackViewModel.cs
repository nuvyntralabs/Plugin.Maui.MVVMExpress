using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

public sealed class PageStackViewModel : PageViewModel
{
    private readonly IPageNavigator _pages;
    private readonly INotifier _notifier;

    public PageStackViewModel(IPageNavigator pages, INotifier? notifier = null)
        : base(pages)
    {
        ArgumentNullException.ThrowIfNull(pages);
        _pages = pages;
        _notifier = notifier ?? NullDialogs.Instance;
        PushCommand = new AsyncModelCommand(PushAsync);
        PopCommand = new AsyncModelCommand(ct => _pages.GoBackAsync(ct), () => _pages.CanGoBack);
        PopToRootCommand = new AsyncModelCommand(ct => _pages.PopToRootAsync(ct), () => _pages.CanGoBack);
        ReplaceCommand = new AsyncModelCommand(ReplaceAsync);
        ResetCommand = new AsyncModelCommand(ct => _pages.ResetAsync<PageStackViewModel>(ct));
        ToastCommand = new AsyncModelCommand(ct =>
            _notifier.ToastAsync($"Window {_pages.Window.WindowId} · stack {_pages.Stack.Count}", cancellationToken: ct));
    }

    public string WindowId => _pages.Window.WindowId;

    public int StackCount => _pages.Stack.Count;

    public bool CanGoBack => _pages.CanGoBack;

    public AsyncModelCommand PushCommand { get; }

    public AsyncModelCommand PopCommand { get; }

    public AsyncModelCommand PopToRootCommand { get; }

    public AsyncModelCommand ReplaceCommand { get; }

    public AsyncModelCommand ResetCommand { get; }

    public AsyncModelCommand ToastCommand { get; }

    public override Task OnNavigatedToAsync(CancellationToken cancellationToken = default)
    {
        Refresh();
        return Task.CompletedTask;
    }

    private async Task PushAsync(CancellationToken cancellationToken)
    {
        var depth = _pages.Stack.Count + 1;
        var result = await _pages.NavigateToAsync(
            "stack-item",
            new Dictionary<string, object> { ["Title"] = $"Item {depth}", ["Depth"] = depth },
            cancellationToken: cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            Refresh();
        }
    }

    private async Task ReplaceAsync(CancellationToken cancellationToken)
    {
        var result = await _pages.NavigateToAsync(
            "stack-item",
            new Dictionary<string, object> { ["Title"] = "Replaced", ["Depth"] = Math.Max(1, _pages.Stack.Count) },
            new NavOptions { Replace = true },
            cancellationToken).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        Notify(nameof(StackCount));
        Notify(nameof(CanGoBack));
        Notify(nameof(WindowId));
        PopCommand.NotifyCanExecuteChanged();
        PopToRootCommand.NotifyCanExecuteChanged();
    }
}

public sealed class PageStackItemViewModel : PageViewModel, IAcceptNavQuery
{
    private readonly IPageNavigator _pages;
    private string _title = "Item";
    private int _depth = 1;

    public PageStackItemViewModel(IPageNavigator pages)
        : base(pages)
    {
        ArgumentNullException.ThrowIfNull(pages);
        _pages = pages;
        PushDeeperCommand = new AsyncModelCommand(PushAsync);
        GoBackCommand = new AsyncModelCommand(ct => _pages.GoBackAsync(ct));
    }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public int Depth
    {
        get => _depth;
        private set => SetProperty(ref _depth, value);
    }

    public string WindowId => _pages.Window.WindowId;

    public int StackCount => _pages.Stack.Count;

    public AsyncModelCommand PushDeeperCommand { get; }

    public AsyncModelCommand GoBackCommand { get; }

    public void Accept(IReadOnlyDictionary<string, object> query)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (query.TryGetValue(nameof(Title), out var title))
        {
            Title = Convert.ToString(title) ?? Title;
        }

        if (query.TryGetValue(nameof(Depth), out var depth)
            && int.TryParse(Convert.ToString(depth), out var parsed))
        {
            Depth = parsed;
        }
    }

    private Task PushAsync(CancellationToken cancellationToken)
        => _pages.NavigateToAsync(
            "stack-item",
            new Dictionary<string, object> { ["Title"] = $"Item {Depth + 1}", ["Depth"] = Depth + 1 },
            cancellationToken: cancellationToken);
}
