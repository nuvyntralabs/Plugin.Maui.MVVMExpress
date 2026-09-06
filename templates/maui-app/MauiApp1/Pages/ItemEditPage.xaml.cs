namespace MauiApp1;

public partial class ItemEditPage : ContentPage
{
    public ItemEditPage(ItemEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
