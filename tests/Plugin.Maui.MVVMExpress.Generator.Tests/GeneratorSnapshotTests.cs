using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Plugin.Maui.MVVMExpress.SourceGenerators;

namespace Plugin.Maui.MVVMExpress.Generator.Tests;

public sealed class GeneratorSnapshotTests
{
    [Fact]
    public void Notify_AndCommand_AndRegistration_AreGenerated()
    {
        const string source = """
            using Plugin.Maui.MVVMExpress.Auth;
            using Plugin.Maui.MVVMExpress.ComponentModel;
            using Plugin.Maui.MVVMExpress.Hosting;
            using Plugin.Maui.MVVMExpress.Input;
            using Plugin.Maui.MVVMExpress.State;

            namespace Demo;

            [RegisterViewModel]
            [Route("demo")]
            [RequiresAuth]
            [RequiresRole("admin")]
            public partial class DemoViewModel : ViewModel
            {
                [Notify]
                [NotifyAlso(nameof(Label))]
                private int _count;

                [PersistState]
                private string _draft = "";

                public string Label => Count.ToString();

                [ModelCommand(CanExecute = nameof(CanClear))]
                private void Clear() => Count = 0;

                private bool CanClear() => Count > 0;
            }
            """;

        var sources = Run(source);
        var members = sources.Single(s => s.HintName.Contains("DemoViewModel"));
        Assert.Contains("public int Count", members.Source);
        Assert.Contains("NotifyDependsOn(nameof(Count), \"Label\")", members.Source);
        Assert.Contains("public global::Plugin.Maui.MVVMExpress.Input.ModelCommand ClearCommand", members.Source);
        Assert.Contains("IPersistableViewModel", members.Source);

        var registration = sources.Single(s => s.HintName.Contains("MvvmExpressGeneratedRegistrations"));
        Assert.Contains("AddGeneratedViewModels", registration.Source);
        Assert.Contains("ApplyRoutes", registration.Source);
        Assert.Contains("\"demo\"", registration.Source);
        Assert.Contains("AuthPolicy", registration.Source);
        Assert.Contains("\"admin\"", registration.Source);
        Assert.Contains("typeof(global::Demo.DemoViewModel)", registration.Source);
    }

    [Fact]
    public void EmptyCompilation_EmitsNothing()
    {
        var sources = Run("namespace Demo { public class Empty {} }");
        Assert.Empty(sources);
    }

    private static List<(string HintName, string Source)> Run(string source)
    {
        var compilation = CSharpCompilation.Create(
            "Tests",
            [CSharpSyntaxTree.ParseText(source)],
            TrustedReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new MvvmExpressGenerator();
        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);

        Assert.DoesNotContain(diagnostics, d => d.Severity == DiagnosticSeverity.Error);

        var result = CSharpGeneratorDriver.Create(generator).RunGenerators(compilation).GetRunResult();
        return [.. result.GeneratedTrees.Select(t => (System.IO.Path.GetFileName(t.FilePath), t.GetText().ToString()))];
    }

    private static MetadataReference[] TrustedReferences()
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
            "Plugin.Maui.MVVMExpress.Core"
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
