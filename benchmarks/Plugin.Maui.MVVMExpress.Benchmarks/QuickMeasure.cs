using System.Diagnostics;
using Plugin.Maui.MVVMExpress.Collections;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Benchmarks;

/// <summary>
/// One-shot measurements for MEMORY-AND-PERFORMANCE.md (not BenchmarkDotNet).
/// Run: dotnet run --project benchmarks/Plugin.Maui.MVVMExpress.Benchmarks -c Release -- --quick
/// </summary>
internal static class QuickMeasure
{
    public static void Run()
    {
        Console.WriteLine("MVVMExpress Core quick measure (Release, net10.0, after warmup)");
        Console.WriteLine($"Runtime: {Environment.Version}  OS: {Environment.OSVersion}");
        RunOnce(write: false);
        RunOnce(write: true);
    }

    private static void RunOnce(bool write)
    {
        MeasureUnchangedSetProperty(write);
        MeasureChangedSetProperty(write);
        foreach (var scale in new[] { ApplicationScale.Small, ApplicationScale.Mid, ApplicationScale.Large })
        {
            MeasureAddRange(scale, write);
        }

        MeasureViewModelCreateDispose(ApplicationScale.Small, write);
        MeasureViewModelCreateDispose(ApplicationScale.Mid, write);
        MeasureCommandExecute(write);
    }

    private static void MeasureUnchangedSetProperty(bool write)
    {
        var vm = new MeasureModel { Name = "warm" };
        vm.PropertyChanged += (_, _) => { };
        _ = vm.Name;
        GC.Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < 10_000; i++)
        {
            vm.Name = "warm";
        }

        sw.Stop();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        if (write)
        {
            Console.WriteLine($"SetProperty unchanged x10000: {sw.Elapsed.TotalMilliseconds:F3} ms, allocated={allocated} B");
        }
    }

    private static void MeasureChangedSetProperty(bool write)
    {
        var vm = new MeasureModel();
        vm.PropertyChanged += (_, _) => { };
        GC.Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < 10_000; i++)
        {
            vm.Name = i % 2 == 0 ? "a" : "b";
        }

        sw.Stop();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        if (write)
        {
            Console.WriteLine($"SetProperty change x10000: {sw.Elapsed.TotalMilliseconds:F3} ms, allocated={allocated} B");
        }
    }

    private static void MeasureAddRange(ApplicationScale scale, bool write)
    {
        var n = ScaleProfile.ListSize(scale);
        var items = Enumerable.Range(0, n).ToArray();
        var collection = new ObservableRangeCollection<int>();
        var events = 0;
        collection.CollectionChanged += (_, _) => events++;
        GC.Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        collection.AddRange(items);
        sw.Stop();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        if (write)
        {
            Console.WriteLine($"AddRange {scale} n={n}: {sw.Elapsed.TotalMilliseconds:F3} ms, events={events}, allocated={allocated} B");
        }
    }

    private static void MeasureViewModelCreateDispose(ApplicationScale scale, bool write)
    {
        var n = ScaleProfile.ViewModelBatch(scale);
        GC.Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < n; i++)
        {
            var vm = new MeasureViewModel { Name = i.ToString() };
            vm.Dispose();
        }

        sw.Stop();
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        if (write)
        {
            Console.WriteLine($"ViewModel create+dispose {scale} n={n}: {sw.Elapsed.TotalMilliseconds:F3} ms, allocated={allocated} B");
        }
    }

    private static void MeasureCommandExecute(bool write)
    {
        var n = 0;
        var command = new ModelCommand(() => n++);
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < 10_000; i++)
        {
            command.Execute(null);
        }

        sw.Stop();
        if (write)
        {
            Console.WriteLine($"ModelCommand.Execute x10000: {sw.Elapsed.TotalMilliseconds:F3} ms (n={n})");
        }
    }

    private sealed class MeasureModel : ObservableModel
    {
        private string? _name;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }

    private sealed class MeasureViewModel : Plugin.Maui.MVVMExpress.ComponentModel.ViewModel
    {
        private string? _name;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
