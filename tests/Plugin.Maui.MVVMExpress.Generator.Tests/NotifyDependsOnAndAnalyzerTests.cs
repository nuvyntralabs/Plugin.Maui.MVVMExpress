using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Plugin.Maui.MVVMExpress.SourceGenerators;

namespace Plugin.Maui.MVVMExpress.Generator.Tests;

public sealed class NotifyDependsOnAndAnalyzerTests
{
    [Fact]
    public void NotifyDependsOn_WiresComputedProperty()
    {
        const string source = """
            using Plugin.Maui.MVVMExpress.ComponentModel;

            namespace Demo;

            public partial class NameViewModel : ObservableModel
            {
                [Notify] private string _first = "";
                [Notify] private string _last = "";

                [NotifyDependsOn(nameof(First), nameof(Last))]
                public string FullName => $"{First} {Last}";
            }
            """;

        var members = RunGenerator(source).Single(item => item.HintName.Contains("NameViewModel"));
        Assert.Contains("NotifyDependsOn(nameof(First), \"FullName\")", members.Source);
        Assert.Contains("NotifyDependsOn(nameof(Last), \"FullName\")", members.Source);
    }

    [Fact]
    public void RegisterView_EmitsApplyPageMaps()
    {
        const string source = """
            using Plugin.Maui.MVVMExpress.ComponentModel;
            using Plugin.Maui.MVVMExpress.Hosting;

            namespace Demo;

            [RegisterViewModel, Route("home")]
            public partial class HomeViewModel : ViewModel;

            [RegisterView(typeof(HomeViewModel))]
            public class HomePage { }
            """;

        var registration = RunGenerator(source).Single(item => item.HintName.Contains("MvvmExpressGeneratedRegistrations"));
        Assert.Contains("ApplyPageMaps", registration.Source);
        Assert.Contains("typeof(global::Demo.HomeViewModel)", registration.Source);
        Assert.Contains("typeof(global::Demo.HomePage)", registration.Source);
        Assert.Contains("\"home\"", registration.Source);
        Assert.Contains("AddGeneratedViewModels", registration.Source);
    }

    [Fact]
    public async Task Analyzer_ReportsViewModelShellAndAlert()
    {
        const string bad = """
            using Plugin.Maui.MVVMExpress.ComponentModel;

            namespace Demo;

            public class BadViewModel : ViewModel
            {
                public void Go()
                {
                    var _ = Shell.Current;
                    DisplayAlert("t", "m", "ok");
                }
            }

            public class Shell { public static Shell Current { get; } = new(); }
            public static class PageExtensions { public static void DisplayAlert(string t, string m, string ok) { } }
            """;

        const string page = """
            using Microsoft.Maui.Controls;

            namespace Demo;

            public class GoodPage : ContentPage
            {
                public void Go() => _ = Shell.Current;
            }
            """;

        var diagnostics = await RunAnalyzer(bad);
        Assert.Contains(diagnostics, item => item.Id == MvvmExpressDiagnosticAnalyzer.ShellOrAlertId);
        var pageDiagnostics = await RunAnalyzer(page);
        Assert.DoesNotContain(pageDiagnostics, item => item.Id == MvvmExpressDiagnosticAnalyzer.ShellOrAlertId);
    }

    [Fact]
    public async Task Analyzer_ReportsUnboundFormField_AndQuietWhenBound()
    {
        const string unbound = """
            using Plugin.Maui.MVVMExpress.Forms;

            namespace Demo;

            public class EditViewModel : FormViewModel
            {
                public EditViewModel()
                {
                    Field("Name", "");
                }
            }
            """;

        const string bound = """
            using Plugin.Maui.MVVMExpress.Forms;

            namespace Demo;

            public class EditViewModel : FormViewModel
            {
                public EditViewModel()
                {
                    var name = Field("Name", "");
                    Bind(name, "Name");
                }
            }
            """;

        Assert.Contains(await RunAnalyzer(unbound), item => item.Id == MvvmExpressDiagnosticAnalyzer.UnboundFormFieldId);
        Assert.DoesNotContain(await RunAnalyzer(bound), item => item.Id == MvvmExpressDiagnosticAnalyzer.UnboundFormFieldId);
    }

    [Fact]
    public async Task Analyzer_ReportsWeakHubThis()
    {
        const string source = """
            using Plugin.Maui.MVVMExpress.ComponentModel;
            using Plugin.Maui.MVVMExpress.Messaging;

            namespace Demo;

            public class InboxViewModel : ViewModel
            {
                public InboxViewModel(IMessageHub hub)
                {
                    hub.Subscribe<InboxViewModel, string>(this, (_, _) => this.ToString());
                }
            }
            """;

        Assert.Contains(await RunAnalyzer(source), item => item.Id == MvvmExpressDiagnosticAnalyzer.WeakHubThisId);
    }

    [Fact]
    public async Task Analyzer_ReportsPhase10Rules()
    {
        const string scan = """
            namespace Demo;
            public class Registrar
            {
                public void Register() { foreach (var t in GetType().Assembly.GetTypes()) { AddTransient(t); } }
                static void AddTransient(System.Type t) { }
            }
            """;
        const string loop = """
            using Plugin.Maui.MVVMExpress.Collections;
            namespace Demo;
            public class ListVm
            {
                public ObservableRangeCollection<int> Items { get; } = new();
                public void Load() { for (var i = 0; i < 3; i++) Items.Add(i); }
            }
            """;
        const string snapshot = """
            using Plugin.Maui.MVVMExpress.Pagination;
            namespace Demo;
            public class LayoutVm
            {
                public SnapshotCollection<int> Items { get; } = new();
                public void Bind() { BindableLayout.SetItemsSource(null, Items); }
            }
            public static class BindableLayout { public static void SetItemsSource(object? v, object i) { } }
            """;
        const string paged = """
            using Plugin.Maui.MVVMExpress.Pagination;
            namespace Demo;
            public class PageVm
            {
                public PagedCollection<int> Items { get; } = null!;
                public int RemainingItemsThreshold { get; set; }
            }
            """;

        Assert.Contains(await RunAnalyzer(scan), item => item.Id == MvvmExpressDiagnosticAnalyzer.ConventionScanId);
        Assert.Contains(await RunAnalyzer(loop), item => item.Id == MvvmExpressDiagnosticAnalyzer.AddInLoopId);
        Assert.Contains(await RunAnalyzer(snapshot), item => item.Id == MvvmExpressDiagnosticAnalyzer.SnapshotBindableLayoutId);
        Assert.Contains(await RunAnalyzer(paged), item => item.Id == MvvmExpressDiagnosticAnalyzer.SyncPagedThresholdId);
    }

    private static List<(string HintName, string Source)> RunGenerator(string source)
    {
        var compilation = CreateCompilation(source);
        var result = CSharpGeneratorDriver.Create(new MvvmExpressGenerator()).RunGenerators(compilation).GetRunResult();
        return [.. result.GeneratedTrees.Select(tree => (System.IO.Path.GetFileName(tree.FilePath), tree.GetText().ToString()))];
    }

    private static async Task<List<Diagnostic>> RunAnalyzer(string source)
    {
        var compilation = CreateCompilation(source);
        var analyzers = new DiagnosticAnalyzer[] { new MvvmExpressDiagnosticAnalyzer() };
        var diagnostics = await compilation.WithAnalyzers(System.Collections.Immutable.ImmutableArray.CreateRange(analyzers))
            .GetAnalyzerDiagnosticsAsync();
        return [.. diagnostics];
    }

    private static CSharpCompilation CreateCompilation(string source)
        => CSharpCompilation.Create(
            "Tests",
            [CSharpSyntaxTree.ParseText(source)],
            GeneratorSnapshotTestsReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private static MetadataReference[] GeneratorSnapshotTestsReferences()
    {
        var needed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "System.Runtime",
            "System.Private.CoreLib",
            "netstandard",
            "System.Linq",
            "System.Collections",
            "System.ComponentModel",
            "System.Console",
            "System.Threading",
            "System.Threading.Tasks",
            "Microsoft.Extensions.DependencyInjection.Abstractions",
            "Plugin.Maui.MVVMExpress.Core",
            "Plugin.Maui.MVVMExpress.Pagination"
        };
        var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        var refs = paths
            .Where(path => needed.Contains(Path.GetFileNameWithoutExtension(path)))
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();
        refs.Add(MetadataReference.CreateFromFile(typeof(Plugin.Maui.MVVMExpress.ComponentModel.ViewModel).Assembly.Location));
        refs.Add(MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly.Location));
        return [.. refs];
    }
}
