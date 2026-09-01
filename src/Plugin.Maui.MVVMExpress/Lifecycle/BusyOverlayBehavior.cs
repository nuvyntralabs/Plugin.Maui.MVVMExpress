using Plugin.Maui.MVVMExpress.ComponentModel;

namespace Plugin.Maui.MVVMExpress.Lifecycle;

/// <summary>Shows an <see cref="ActivityIndicator"/> over the page while the ViewModel is busy.</summary>
public sealed class BusyOverlayBehavior : Behavior<Page>
{
    private Grid? _host;
    private ActivityIndicator? _indicator;
    private View? _original;
    private IViewModel? _viewModel;

    /// <inheritdoc />
    protected override void OnAttachedTo(Page bindable)
    {
        ArgumentNullException.ThrowIfNull(bindable);
        base.OnAttachedTo(bindable);
        bindable.BindingContextChanged += OnBindingContextChanged;
        AttachViewModel(bindable.BindingContext as IViewModel);
        EnsureOverlay(bindable);
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(Page bindable)
    {
        bindable.BindingContextChanged -= OnBindingContextChanged;
        DetachViewModel();
        base.OnDetachingFrom(bindable);
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
        => AttachViewModel((sender as BindableObject)?.BindingContext as IViewModel);

    private void AttachViewModel(IViewModel? viewModel)
    {
        if (_viewModel is ObservableModel previous)
        {
            previous.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = viewModel;
        if (viewModel is ObservableModel next)
        {
            next.PropertyChanged += OnViewModelPropertyChanged;
        }

        Update();
    }

    private void DetachViewModel()
    {
        if (_viewModel is ObservableModel previous)
        {
            previous.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = null;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IViewModel.IsBusy) or null)
        {
            Update();
        }
    }

    private void EnsureOverlay(Page page)
    {
        if (page is not ContentPage content)
        {
            return;
        }

        if (content.Content is Grid existing)
        {
            _host = existing;
        }
        else
        {
            _original = content.Content;
            _host = new Grid { AutomationId = "MvvmExpressBusyHost" };
            if (_original is not null)
            {
                _host.Add(_original);
            }

            content.Content = _host;
        }

        _indicator = new ActivityIndicator
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false,
            InputTransparent = true
        };
        _host.Add(_indicator);
    }

    private void Update()
    {
        if (_indicator is null)
        {
            return;
        }

        var busy = _viewModel?.IsBusy == true;
        _indicator.IsVisible = busy;
        _indicator.IsRunning = busy;
    }
}
