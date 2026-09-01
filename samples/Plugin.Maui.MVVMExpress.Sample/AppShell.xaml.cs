using Plugin.Maui.MVVMExpress.Sample.Pages;

namespace Plugin.Maui.MVVMExpress.Sample;

public partial class AppShell : Shell
{
    public AppShell(
        CounterPage counter,
        ProductListPage products,
        ProductEditPage edit,
        HomePage home,
        PageStackPage pageStack,
        LoginPage login,
        OfflinePage offline,
        PaginationPage pagination,
        SearchPage search,
        EnterprisePage enterprise)
    {
        InitializeComponent();
        FlyoutBehavior = FlyoutBehavior.Flyout;
        Items.Add(Create("Basic", "counter", counter));
        Items.Add(Create("CRUD", "products", products));
        Items.Add(Create("Edit", "edit", edit));
        Items.Add(Create("Navigation", "home", home));
        Items.Add(Create("Page stack", "stack", pageStack));
        Items.Add(Create("Auth", "login", login));
        Items.Add(Create("Offline", "offline", offline));
        Items.Add(Create("Pagination", "pagination", pagination));
        Items.Add(Create("Reactive", "search", search));
        Items.Add(Create("Enterprise", "enterprise", enterprise));

        Routing.RegisterRoute("details", typeof(ProductDetailsPage));
        Routing.RegisterRoute("secure", typeof(SecureHomePage));
    }

    private static FlyoutItem Create(string title, string route, Page page)
        => new()
        {
            Title = title,
            Items =
            {
                new ShellContent
                {
                    Title = title,
                    Route = route,
                    Content = page
                }
            }
        };
}
