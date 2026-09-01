using Plugin.Maui.MVVMExpress.Samples.Pagination;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class PaginationPage : SampleContentPage
{
    public PaginationPage(PagedProductViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
