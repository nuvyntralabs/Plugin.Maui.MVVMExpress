using Plugin.Maui.MVVMExpress.Pagination;

namespace Plugin.Maui.MVVMExpress.Pagination.Tests;

public sealed class PagedCollectionTests
{
    [Fact]
    public async Task LoadMore_AppendsUntilExhausted()
    {
        var source = Enumerable.Range(1, 5).ToArray();
        var pages = new DelegatePagedCollection<int>(
            (skip, take, _) => Task.FromResult<IReadOnlyList<int>>(source.Skip(skip).Take(take).ToArray()),
            pageSize: 2);
        await pages.RefreshAsync();
        Assert.Equal([1, 2], pages.Items.ToArray());
        Assert.True(pages.HasMore);
        await pages.LoadMoreAsync();
        await pages.LoadMoreAsync();
        Assert.Equal([1, 2, 3, 4, 5], pages.Items.ToArray());
        Assert.False(pages.HasMore);
    }

    [Fact]
    public async Task Refresh_ResetsFromStart()
    {
        var source = Enumerable.Range(1, 3).ToArray();
        var pages = new DelegatePagedCollection<int>(
            (skip, take, _) => Task.FromResult<IReadOnlyList<int>>(source.Skip(skip).Take(take).ToArray()),
            pageSize: 2);
        await pages.RefreshAsync();
        await pages.LoadMoreAsync();
        await pages.RefreshAsync();
        Assert.Equal([1, 2], pages.Items.ToArray());
        Assert.True(pages.HasMore);
    }
}
