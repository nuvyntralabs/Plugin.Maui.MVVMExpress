namespace MauiApp1;

public partial class FeaturePage : ContentPage
{
    public FeaturePage(FeatureViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
