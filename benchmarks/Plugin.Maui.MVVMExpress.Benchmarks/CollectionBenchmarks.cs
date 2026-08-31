using BenchmarkDotNet.Attributes;
using Plugin.Maui.MVVMExpress.Collections;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Benchmarks;

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "Median")]
public class CollectionBenchmarks
{
    private int[] _items = [];

    [Params(ApplicationScale.Small, ApplicationScale.Mid, ApplicationScale.Large)]
    public ApplicationScale Scale { get; set; }

    [GlobalSetup]
    public void Setup() => _items = Enumerable.Range(0, ScaleProfile.ListSize(Scale)).ToArray();

    [Benchmark]
    public int AddRange_SingleReset()
    {
        var collection = new ObservableRangeCollection<int>();
        collection.CollectionChanged += (_, _) => { };
        collection.AddRange(_items);
        return collection.Count;
    }
}
