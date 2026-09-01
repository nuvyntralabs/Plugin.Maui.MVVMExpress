using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Diagnostics;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Navigation.Tests;

public sealed class MauiPageNavigatorTests
{
    [Fact]
    public async Task Unmapped_ReturnsE_ROUTE()
    {
        var navigator = new MauiPageNavigator(new WindowContext("page"));
        var result = await navigator.NavigateToAsync<EmptyViewModel>();
        Assert.Equal("E_ROUTE", result.Error?.Code);
        Assert.Empty(navigator.Stack);
    }

    [Fact]
    public async Task MappedWithoutHost_ReturnsE_PAGE()
    {
        var navigator = new MauiPageNavigator(new WindowContext("page"))
            .Map<EmptyViewModel, DummyPage>("empty");
        Assert.True(navigator.TryResolve("empty?x=1", out var type));
        Assert.Equal(typeof(EmptyViewModel), type);
        var result = await navigator.NavigateToAsync("empty", new Dictionary<string, object> { ["x"] = 1 });
        Assert.Equal("E_PAGE", result.Error?.Code);
        Assert.Empty(navigator.Stack);
        Assert.Equal("page", navigator.Window.WindowId);
    }

    [Fact]
    public async Task StackApisWithoutHost_ReturnE_PAGE()
    {
        var navigator = new MauiPageNavigator().Map<EmptyViewModel, DummyPage>("empty");
        Assert.Equal("E_PAGE", (await navigator.GoBackAsync()).Error?.Code);
        Assert.Equal("E_PAGE", (await navigator.PopToRootAsync()).Error?.Code);
        Assert.Equal("E_PAGE", (await navigator.ReplaceAsync<EmptyViewModel>()).Error?.Code);
        Assert.Equal("E_PAGE", (await navigator.ResetAsync<EmptyViewModel>()).Error?.Code);
        Assert.Equal("E_PAGE", (await navigator.ReplaceRootAsync<EmptyViewModel>()).Error?.Code);
    }

    [Fact]
    public async Task NavigateToAsync_HopsToIMainThread_BeforePageWork()
    {
        var traces = new List<string>();
        var main = new RecordingMainThread();
        var diagnostics = new CallbackDiagnostics((_, message) => traces.Add(message));
        var navigator = new MauiPageNavigator(new WindowContext("page"), mainThread: main, diagnostics: diagnostics)
            .Map<EmptyViewModel, DummyPage>("empty");
        var result = await navigator.NavigateToAsync<EmptyViewModel>();
        Assert.Equal("E_PAGE", result.Error?.Code);
        Assert.True(main.InvokeCount > 0);
        Assert.Contains(traces, item => item.Contains("IMainThread", StringComparison.Ordinal));
    }

    private sealed class RecordingMainThread : IMainThread
    {
        public int InvokeCount { get; private set; }

        public bool IsMainThread { get; private set; }

        public void BeginInvoke(Action action)
        {
            InvokeCount++;
            IsMainThread = true;
            try
            {
                action();
            }
            finally
            {
                IsMainThread = false;
            }
        }

        public Task InvokeAsync(Action action, CancellationToken cancellationToken = default)
        {
            InvokeCount++;
            IsMainThread = true;
            try
            {
                action();
                return Task.CompletedTask;
            }
            finally
            {
                IsMainThread = false;
            }
        }

        public async Task InvokeAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            InvokeCount++;
            IsMainThread = true;
            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                IsMainThread = false;
            }
        }
    }

    private sealed class EmptyViewModel : ViewModel;

    private sealed class DummyPage : ContentPage;
}
