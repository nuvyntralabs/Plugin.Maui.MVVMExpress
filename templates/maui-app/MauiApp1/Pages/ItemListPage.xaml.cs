namespace MauiApp1;

public partial class ItemListPage : ContentPage
{
    public ItemListPage(ItemListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
