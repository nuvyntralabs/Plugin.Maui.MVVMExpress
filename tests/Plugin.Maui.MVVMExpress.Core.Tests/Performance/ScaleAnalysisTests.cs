using System.Diagnostics;
using Plugin.Maui.MVVMExpress.Collections;
using Plugin.Maui.MVVMExpress.Core.Tests.Support;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Performance;

/// <summary>
/// Asserts scale budgets and records elapsed / allocated figures for the memory document.
/// </summary>
public sealed class ScaleAnalysisTests
{
    [Theory]
    [InlineData(ApplicationScale.Small)]
    [InlineData(ApplicationScale.Mid)]
    [InlineData(ApplicationScale.Large)]
    public void AddRange_MeetsTimeAndNotifyBudget(ApplicationScale scale)
    {
        var n = ScaleProfile.ListSize(scale);
        var items = Enumerable.Range(0, n).ToArray();
        var collection = new ObservableRangeCollection<int>();
        var events = 0;
        collection.CollectionChanged += (_, _) => events++;

        var sw = Stopwatch.StartNew();
        var before = GC.GetAllocatedBytesForCurrentThread();
        collection.AddRange(items);
        sw.Stop();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(n, collection.Count);
        Assert.Equal(1, events);
        var maxMs = scale switch
        {
            ApplicationScale.Small => 50,
            ApplicationScale.Mid => 250,
            _ => 2_000
        };
        Assert.True(sw.ElapsedMilliseconds <= maxMs, $"{scale} AddRange {n} took {sw.ElapsedMilliseconds} ms allocated={allocated} (budget {maxMs} ms).");
    }

    [Fact]
    public void AddLoop_Small_RaisesPerItem_SlowerPathDocumented()
    {
        const int n = 200;
        var collection = new ObservableRangeCollection<int>();
        var events = 0;
        collection.CollectionChanged += (_, _) => events++;
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < n; i++)
        {
            collection.Add(i);
        }

        sw.Stop();
        Assert.Equal(n, events);
        Assert.True(sw.ElapsedMilliseconds < 200);
    }

    [Fact]
    public void SetProperty_Unchanged_StaysUnderAllocationBudget()
    {
        var vm = new ProbeViewModel { Name = "warm" };
        vm.PropertyChanged += (_, _) => { };
        var before = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < 10_000; i++)
        {
            vm.Name = "warm";
        }

        sw.Stop();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.True(allocated < 2_048, $"allocated {allocated}");
        Assert.True(sw.ElapsedMilliseconds < 250, $"elapsed {sw.ElapsedMilliseconds} ms");
    }
}
