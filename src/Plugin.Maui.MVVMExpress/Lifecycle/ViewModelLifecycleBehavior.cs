using Plugin.Maui.MVVMExpress.ComponentModel;

namespace Plugin.Maui.MVVMExpress.Lifecycle;

/// <summary>Forwards page appear/disappear to <see cref="IViewModel"/> without code-behind.</summary>
public sealed class ViewModelLifecycleBehavior : Behavior<Page>
{
    private bool _initialized;

    /// <inheritdoc />
    protected override void OnAttachedTo(Page bindable)
    {
        ArgumentNullException.ThrowIfNull(bindable);
        base.OnAttachedTo(bindable);
        bindable.Appearing += OnAppearing;
        bindable.Disappearing += OnDisappearing;
    }

    /// <inheritdoc />
    protected override void OnDetachingFrom(Page bindable)
    {
        bindable.Appearing -= OnAppearing;
        bindable.Disappearing -= OnDisappearing;
        base.OnDetachingFrom(bindable);
    }

    private async void OnAppearing(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not IViewModel viewModel)
        {
            return;
        }

        try
        {
            if (!_initialized)
            {
                _initialized = true;
                await viewModel.InitializeAsync().ConfigureAwait(true);
            }

            await viewModel.OnAppearingAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private async void OnDisappearing(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not IViewModel viewModel)
        {
            return;
        }

        await viewModel.OnDisappearingAsync().ConfigureAwait(true);
    }
}
