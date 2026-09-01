using Plugin.Maui.MVVMExpress.ComponentModel;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public abstract class SampleContentPage : ContentPage
{
    private bool _initialized;

    protected SampleContentPage(ViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ViewModel = viewModel;
        BindingContext = viewModel;
    }

    protected ViewModel ViewModel { get; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            if (!_initialized)
            {
                _initialized = true;
                await ViewModel.InitializeAsync();
            }

            await ViewModel.OnAppearingAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await ViewModel.OnDisappearingAsync();
    }
}
