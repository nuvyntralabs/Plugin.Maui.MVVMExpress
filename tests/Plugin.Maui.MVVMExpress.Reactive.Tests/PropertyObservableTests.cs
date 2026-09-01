using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Reactive;

namespace Plugin.Maui.MVVMExpress.Reactive.Tests;

public sealed class PropertyObservableTests
{
    [Fact]
    public void CombineLatest_ProjectsWhenEitherChanges()
    {
        var model = new NameModel();
        using var combined = PropertyObservable.CombineLatest(
            PropertyObservable.Observe(model, nameof(NameModel.First), () => model.First),
            PropertyObservable.Observe(model, nameof(NameModel.Last), () => model.Last),
            static (first, last) => $"{first} {last}".Trim());

        var seen = new List<string>();
        using var sub = combined.Subscribe(seen.Add);
        Assert.Contains("", seen);
        model.First = "Ada";
        model.Last = "Lovelace";
        Assert.Equal("Ada Lovelace", combined.Value);
        Assert.Contains("Ada Lovelace", seen);
    }

    [Fact]
    public void Observe_Subscribe_GetsCurrentThenChanges()
    {
        var model = new NameModel { First = "A" };
        using var observed = PropertyObservable.Observe(model, nameof(NameModel.First), () => model.First);
        var seen = new List<string>();
        using var sub = observed.Subscribe(seen.Add);
        model.First = "B";
        Assert.Equal(["A", "B"], seen);
    }

    private sealed class NameModel : ObservableModel
    {
        private string _first = "";
        private string _last = "";

        public string First
        {
            get => _first;
            set => SetProperty(ref _first, value);
        }

        public string Last
        {
            get => _last;
            set => SetProperty(ref _last, value);
        }
    }
}
