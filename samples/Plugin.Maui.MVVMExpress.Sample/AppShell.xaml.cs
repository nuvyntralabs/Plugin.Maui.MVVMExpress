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
        EnterprisePage enterprise,
        ScopesPage scopes,
        ChatHostPage chats,
        ManualCounterPage manual,
        ComputedNamePage computed,
        PipelinePage pipeline,
        ModalHostPage modal,
        TwoWindowPage twoWindow,
        GeneratedCatalogPage generated,
        AdapterCatalogPage adapters,
        ToolkitInboxPage toolkit)
    {
        InitializeComponent();
        FlyoutBehavior = FlyoutBehavior.Flyout;
        Items.Add(Create("Basic", "counter", counter));
        Items.Add(Create("Escape hatch", "manual", manual));
        Items.Add(Create("NotifyDependsOn", "computed", computed));
        Items.Add(Create("CRUD", "products", products));
        Items.Add(Create("Edit", "edit", edit));
        Items.Add(Create("Navigation", "home", home));
        Items.Add(Create("Page stack", "stack", pageStack));
        Items.Add(Create("Page scopes", "scopes", scopes));
        Items.Add(Create("Modal", "modal", modal));
        Items.Add(Create("Chat host", "chats", chats));
        Items.Add(Create("Auth", "login", login));
        Items.Add(Create("Offline", "offline", offline));
        Items.Add(Create("Pagination", "pagination", pagination));
        Items.Add(Create("Reactive", "search", search));
        Items.Add(Create("Pipeline", "pipeline", pipeline));
        Items.Add(Create("Generated", "generated", generated));
        Items.Add(Create("Enterprise", "enterprise", enterprise));
        Items.Add(Create("Two windows", "two-window", twoWindow));
        Items.Add(Create("Adapters", "adapters", adapters));
        Items.Add(Create("CommunityToolkit", "toolkit", toolkit));

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
