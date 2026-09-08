using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Plugin.Maui.MVVMExpress.SourceGenerators;

/// <summary>Reports MVVME001–003 and Phase 10 list / convention diagnostics.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MvvmExpressDiagnosticAnalyzer : DiagnosticAnalyzer
{
    /// <summary>Shell.Current or DisplayAlert inside a ViewModel.</summary>
    public const string ShellOrAlertId = "MVVME001";

    /// <summary>this captured on a weak IMessageHub handler.</summary>
    public const string WeakHubThisId = "MVVME002";

    /// <summary>FormField created without Bind.</summary>
    public const string UnboundFormFieldId = "MVVME003";

    /// <summary>Convention GetTypes registration (DEBUG).</summary>
    public const string ConventionScanId = "MVVME010";

    /// <summary>ObservableRangeCollection.Add in a loop.</summary>
    public const string AddInLoopId = "MVVME011";

    /// <summary>SnapshotCollection assigned to BindableLayout.</summary>
    public const string SnapshotBindableLayoutId = "MVVME012";

    /// <summary>PagedCollection + RemainingItemsThreshold.</summary>
    public const string SyncPagedThresholdId = "MVVME013";

    private static readonly DiagnosticDescriptor ShellOrAlert = Create(
        ShellOrAlertId,
        "ViewModels must not call Shell.Current or Page.DisplayAlert",
        "Do not use {0} in a ViewModel. Inject INavigator or IDialogs.",
        DiagnosticSeverity.Error);

    private static readonly DiagnosticDescriptor WeakHubThis = Create(
        WeakHubThisId,
        "Do not capture this on a weak IMessageHub handler",
        "This Subscribe handler captures 'this'. Unsubscribe in Dispose or use a static handler.",
        DiagnosticSeverity.Warning);

    private static readonly DiagnosticDescriptor UnboundFormField = Create(
        UnboundFormFieldId,
        "FormField should be Bind()ed",
        "FormField '{0}' is created without Bind. Call Bind(field, propertyName) so XAML can see the value.",
        DiagnosticSeverity.Warning);

    private static readonly DiagnosticDescriptor ConventionScan = Create(
        ConventionScanId,
        "Convention scanning is not a supported registration path",
        "Do not use GetTypes() to register ViewModels. Use [RegisterViewModel] / [Route] generators or Map().",
        DiagnosticSeverity.Error);

    private static readonly DiagnosticDescriptor AddInLoop = Create(
        AddInLoopId,
        "Do not Add items in a loop to ObservableRangeCollection",
        "ObservableRangeCollection.Add in a loop raises per-item change. Use AddRange.",
        DiagnosticSeverity.Warning);

    private static readonly DiagnosticDescriptor SnapshotBindable = Create(
        SnapshotBindableLayoutId,
        "Do not bind SnapshotCollection to BindableLayout",
        "SnapshotCollection is for CollectionView / virtualized lists, not BindableLayout.",
        DiagnosticSeverity.Warning);

    private static readonly DiagnosticDescriptor PagedThreshold = Create(
        SyncPagedThresholdId,
        "RemainingItemsThreshold requires an async PagedCollection fetch",
        "Do not pair PagedCollection with RemainingItemsThreshold when the fetch is synchronous. Use SnapshotCollection.",
        DiagnosticSeverity.Error);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [ShellOrAlert, WeakHubThis, UnboundFormField, ConventionScan, AddInLoop, SnapshotBindable, PagedThreshold];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax syntax)
        {
            return;
        }

        var model = context.SemanticModel;
        if (model.GetDeclaredSymbol(syntax, context.CancellationToken) is not INamedTypeSymbol type)
        {
            return;
        }

        var isViewModel = Inherits(type, "ObservableModel", "ViewModel", "PageViewModel", "FormViewModel");
        var isForm = Inherits(type, "FormViewModel");
        var text = syntax.ToString();

        if (isViewModel)
        {
            foreach (var access in syntax.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
            {
                if (access.Expression is IdentifierNameSyntax { Identifier.ValueText: "Shell" }
                    && access.Name.Identifier.ValueText == "Current")
                {
                    context.ReportDiagnostic(Diagnostic.Create(ShellOrAlert, access.GetLocation(), "Shell.Current"));
                }
            }

            foreach (var invocation in syntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (NameOf(invocation) == "DisplayAlert")
                {
                    context.ReportDiagnostic(Diagnostic.Create(ShellOrAlert, invocation.GetLocation(), "DisplayAlert"));
                }

                if (NameOf(invocation) == "Subscribe" && CapturesThis(invocation))
                {
                    context.ReportDiagnostic(Diagnostic.Create(WeakHubThis, invocation.GetLocation()));
                }
            }
        }

        if (isForm)
        {
            var hasBind = syntax.DescendantNodes().OfType<InvocationExpressionSyntax>().Any(node => NameOf(node) == "Bind");
            foreach (var invocation in syntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (NameOf(invocation) != "Field" || hasBind || IsAssignedToMember(invocation))
                {
                    continue;
                }

                var name = invocation.ArgumentList.Arguments.FirstOrDefault()?.ToString() ?? "field";
                context.ReportDiagnostic(Diagnostic.Create(UnboundFormField, invocation.GetLocation(), name));
            }
        }

        if (text.Contains("GetTypes()") && (text.Contains("AddTransient") || text.Contains("AddSingleton") || text.Contains("RegisterViewModel")))
        {
            var getTypes = syntax.DescendantNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault(node => NameOf(node) == "GetTypes");
            if (getTypes is not null)
            {
                context.ReportDiagnostic(Diagnostic.Create(ConventionScan, getTypes.GetLocation()));
            }
        }

        if (text.Contains("ObservableRangeCollection"))
        {
            foreach (var loop in syntax.DescendantNodes().OfType<ForStatementSyntax>())
            {
                foreach (var invocation in loop.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    if (NameOf(invocation) == "Add")
                    {
                        context.ReportDiagnostic(Diagnostic.Create(AddInLoop, invocation.GetLocation()));
                    }
                }
            }
        }

        if (text.Contains("SnapshotCollection") && text.Contains("BindableLayout"))
        {
            context.ReportDiagnostic(Diagnostic.Create(SnapshotBindable, syntax.Identifier.GetLocation()));
        }

        if (text.Contains("PagedCollection") && text.Contains("RemainingItemsThreshold"))
        {
            context.ReportDiagnostic(Diagnostic.Create(PagedThreshold, syntax.Identifier.GetLocation()));
        }
    }

    private static bool IsAssignedToMember(InvocationExpressionSyntax invocation)
        => invocation.Parent is AssignmentExpressionSyntax
            || invocation.Parent is EqualsValueClauseSyntax { Parent: PropertyDeclarationSyntax or FieldDeclarationSyntax or VariableDeclaratorSyntax { Parent: FieldDeclarationSyntax } };

    private static bool CapturesThis(InvocationExpressionSyntax invocation)
        => invocation.ArgumentList.Arguments.Any(argument =>
            argument.Expression is LambdaExpressionSyntax lambda
            && lambda.DescendantNodes().OfType<ThisExpressionSyntax>().Any());

    private static string? NameOf(InvocationExpressionSyntax invocation)
        => invocation.Expression switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
            _ => null
        };

    private static bool Inherits(INamedTypeSymbol type, params string[] names)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (names.Contains(current.Name))
            {
                return true;
            }
        }

        return false;
    }

    private static DiagnosticDescriptor Create(string id, string title, string message, DiagnosticSeverity severity)
        => new(
            id,
            title,
            message,
            "MVVMExpress",
            severity,
            isEnabledByDefault: true);
}
