using Plugin.Maui.MVVMExpress.Pagination;

namespace Plugin.Maui.MVVMExpress.Pagination.Tests;

public sealed class SearchQueryTests
{
    [Fact]
    public async Task WhenReady_ReturnsFalse_WhenSuperseded()
    {
        var query = new SearchQuery(TimeSpan.FromMilliseconds(40));
        query.Text = "a";
        var first = query.WhenReadyAsync();
        query.Text = "ab";
        Assert.False(await first);
        Assert.True(await query.WhenReadyAsync());
        Assert.Equal("ab", query.Text);
    }

    [Fact]
    public async Task ZeroDebounce_IsImmediatelyReady()
    {
        var query = new SearchQuery(TimeSpan.Zero);
        query.Text = "x";
        Assert.True(await query.WhenReadyAsync());
    }
}
