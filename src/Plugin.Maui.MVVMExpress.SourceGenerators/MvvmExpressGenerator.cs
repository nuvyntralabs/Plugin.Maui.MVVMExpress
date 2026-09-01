using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Plugin.Maui.MVVMExpress.SourceGenerators;

/// <summary>Emits notify properties, commands, persist methods, and AOT registration from MVVMExpress attributes.</summary>
[Generator]
public sealed class MvvmExpressGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } or ClassDeclarationSyntax { Members.Count: > 0 },
                static (ctx, ct) => Target.From(ctx, ct))
            .Where(static t => t is not null)
            .Select(static (t, _) => t!);

        context.RegisterSourceOutput(classes.Collect(), static (spc, items) => Execute(spc, items));
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<Target> targets)
    {
        foreach (var target in targets)
        {
            if (target.HasMembers)
            {
                context.AddSource($"{Sanitize(target.FullName)}.g.cs", SourceText.From(target.RenderPartial(), Encoding.UTF8));
            }
        }

        var registered = targets.Where(static t => t.RegisterViewModel || t.Route is not null || t.RequiresAuth || t.Role is not null || t.RegisterView).ToArray();
        if (registered.Length > 0)
        {
            context.AddSource("MvvmExpressGeneratedRegistrations.g.cs", SourceText.From(Target.RenderRegistrations(registered), Encoding.UTF8));
        }
    }

    private static string Sanitize(string name)
        => name.Replace("global::", "").Replace('.', '_').Replace('<', '_').Replace('>', '_');

    internal sealed class Target
    {
        public string Namespace { get; init; } = "";
        public string TypeName { get; init; } = "";
        public string FullName { get; init; } = "";
        public string Accessibility { get; init; } = "public";
        public bool RegisterViewModel { get; init; }
        public bool RegisterView { get; init; }
        public string? ViewModelType { get; init; }
        public string? Route { get; init; }
        public bool RequiresAuth { get; init; }
        public string? Role { get; init; }
        public List<NotifyField> Fields { get; } = [];
        public List<CommandMethod> Commands { get; } = [];
        public List<PersistField> Persist { get; } = [];

        public bool HasMembers => Fields.Count > 0 || Commands.Count > 0 || Persist.Count > 0;

        public static Target? From(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            if (context.Node is not ClassDeclarationSyntax syntax)
            {
                return null;
            }

            if (context.SemanticModel.GetDeclaredSymbol(syntax, cancellationToken) is not INamedTypeSymbol type)
            {
                return null;
            }

            var target = new Target
            {
                Namespace = type.ContainingNamespace.IsGlobalNamespace ? "" : type.ContainingNamespace.ToDisplayString(),
                TypeName = type.Name,
                FullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                Accessibility = SyntaxFacts.GetText(syntax.Modifiers.Any(SyntaxKind.InternalKeyword) ? SyntaxKind.InternalKeyword : SyntaxKind.PublicKeyword),
                RegisterViewModel = HasAttribute(type, "RegisterViewModelAttribute"),
                RegisterView = HasAttribute(type, "RegisterViewAttribute"),
                ViewModelType = GetTypeArgument(type, "RegisterViewAttribute"),
                Route = GetStringArgument(type, "RouteAttribute"),
                RequiresAuth = HasAttribute(type, "RequiresAuthAttribute"),
                Role = GetStringArgument(type, "RequiresRoleAttribute")
            };

            foreach (var field in type.GetMembers().OfType<IFieldSymbol>())
            {
                if (HasAttribute(field, "NotifyAttribute"))
                {
                    target.Fields.Add(new NotifyField(
                        field.Name,
                        ToPropertyName(field.Name),
                        field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        GetNotifyAlso(field)));
                }

                if (HasAttribute(field, "PersistStateAttribute") && !IsSensitive(field))
                {
                    target.Persist.Add(new PersistField(
                        field.Name,
                        field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
                }
            }

            foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
            {
                if (method.MethodKind != MethodKind.Ordinary)
                {
                    continue;
                }

                if (HasAttribute(method, "ModelCommandAttribute"))
                {
                    target.Commands.Add(CommandMethod.Sync(method));
                }
                else if (HasAttribute(method, "AsyncModelCommandAttribute"))
                {
                    target.Commands.Add(CommandMethod.Async(method));
                }
            }

            if (!target.HasMembers && !target.RegisterViewModel && !target.RegisterView && target.Route is null && !target.RequiresAuth && target.Role is null)
            {
                return null;
            }

            return target;
        }

        public string RenderPartial()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("#nullable enable");
            if (Namespace.Length > 0)
            {
                sb.Append("namespace ").AppendLine(Namespace).AppendLine("{");
            }

            sb.Append("    ").Append(Accessibility).Append(" partial class ").AppendLine(TypeName);
            if (Persist.Count > 0)
            {
                sb.AppendLine("        : global::Plugin.Maui.MVVMExpress.State.IPersistableViewModel");
            }

            sb.AppendLine("    {");
            foreach (var field in Fields)
            {
                sb.Append("        public ").Append(field.Type).Append(' ').AppendLine(field.PropertyName);
                sb.AppendLine("        {");
                sb.Append("            get => ").Append(field.FieldName).AppendLine(";");
                sb.AppendLine("            set");
                sb.AppendLine("            {");
                if (field.Also.Length == 0)
                {
                    sb.Append("                SetProperty(ref ").Append(field.FieldName).AppendLine(", value);");
                }
                else
                {
                    sb.Append("                if (SetProperty(ref ").Append(field.FieldName).AppendLine(", value))");
                    sb.AppendLine("                {");
                    sb.Append("                    NotifyDependsOn(nameof(").Append(field.PropertyName).Append(')');
                    foreach (var also in field.Also)
                    {
                        sb.Append(", \"").Append(also).Append('"');
                    }

                    sb.AppendLine(");");
                    sb.AppendLine("                }");
                }

                sb.AppendLine("            }");
                sb.AppendLine("        }");
            }

            foreach (var command in Commands)
            {
                var cmdType = command.IsAsync
                    ? "global::Plugin.Maui.MVVMExpress.Input.AsyncModelCommand"
                    : "global::Plugin.Maui.MVVMExpress.Input.ModelCommand";
                sb.Append("        private ").Append(cmdType).Append("? ").Append(command.BackingField).AppendLine(";");
                sb.Append("        public ").Append(cmdType).Append(' ').AppendLine(command.PropertyName);
                sb.Append("            => ").Append(command.BackingField).Append(" ??= new ").Append(cmdType).Append('(').Append(command.MethodName);
                if (command.CanExecute is { } can)
                {
                    sb.Append(", ").Append(can);
                }

                sb.AppendLine(");");
            }

            if (Persist.Count > 0)
            {
                sb.AppendLine("        public async global::System.Threading.Tasks.Task SavePersistedStateAsync(global::Plugin.Maui.MVVMExpress.State.IStateStore store, global::System.Threading.CancellationToken cancellationToken = default)");
                sb.AppendLine("        {");
                foreach (var field in Persist)
                {
                    var key = FullName.Replace("global::", "") + "." + field.FieldName;
                    sb.Append("            await store.SaveAsync(\"").Append(key).Append("\", global::System.Convert.ToString(").Append(field.FieldName).AppendLine(", global::System.Globalization.CultureInfo.InvariantCulture) ?? \"\", cancellationToken).ConfigureAwait(false);");
                }

                sb.AppendLine("        }");
                sb.AppendLine("        public async global::System.Threading.Tasks.Task RestorePersistedStateAsync(global::Plugin.Maui.MVVMExpress.State.IStateStore store, global::System.Threading.CancellationToken cancellationToken = default)");
                sb.AppendLine("        {");
                foreach (var field in Persist)
                {
                    var key = FullName.Replace("global::", "") + "." + field.FieldName;
                    sb.Append("            var ").Append(field.FieldName).Append("Value = await store.LoadAsync(\"").Append(key).AppendLine("\", cancellationToken).ConfigureAwait(false);");
                    sb.Append("            if (").Append(field.FieldName).AppendLine("Value is not null)");
                    sb.AppendLine("            {");
                    sb.Append("                ").Append(field.FieldName).Append(" = (").Append(field.Type).Append(")global::System.Convert.ChangeType(").Append(field.FieldName).Append("Value, typeof(").Append(field.Type).AppendLine("), global::System.Globalization.CultureInfo.InvariantCulture);");
                    sb.AppendLine("            }");
                }

                sb.AppendLine("        }");
            }

            sb.AppendLine("    }");
            if (Namespace.Length > 0)
            {
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        public static string RenderRegistrations(IEnumerable<Target> targets)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("#nullable enable");
            sb.AppendLine("namespace Plugin.Maui.MVVMExpress.Generated;");
            sb.AppendLine();
            sb.AppendLine("public static class MvvmExpressGeneratedRegistrations");
            sb.AppendLine("{");
            sb.AppendLine("    public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddGeneratedViewModels(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
            sb.AppendLine("    {");
            foreach (var target in targets)
            {
                if (target.RegisterViewModel)
                {
                    sb.Append("        global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddTransient(services, typeof(").Append(target.FullName).AppendLine("));");
                }

                if (target.RegisterView)
                {
                    sb.Append("        global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddTransient(services, typeof(").Append(target.FullName).AppendLine("));");
                    if (target.ViewModelType is { } vm)
                    {
                        sb.Append("        global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddTransient(services, typeof(").Append(vm).AppendLine("));");
                    }
                }
            }

            sb.AppendLine("        return services;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public static void ApplyRoutes(global::System.Action<global::System.Type, string> map)");
            sb.AppendLine("    {");
            foreach (var target in targets.Where(static t => t.Route is not null))
            {
                sb.Append("        map(typeof(").Append(target.FullName).Append("), \"").Append(target.Route).AppendLine("\");");
            }

            sb.AppendLine("    }");
            sb.AppendLine();
            sb.Append("    public static global::Plugin.Maui.MVVMExpress.Auth.INavigationAuthPolicy AuthPolicy { get; } = new global::Plugin.Maui.MVVMExpress.Auth.NavigationAuthPolicy(");
            var auth = targets.Where(static t => t.RequiresAuth || t.Role is not null).ToArray();
            if (auth.Length == 0)
            {
                sb.AppendLine(");");
            }
            else
            {
                sb.AppendLine();
                sb.Append("        authRequired: [");
                sb.Append(string.Join(", ", auth.Select(t => $"typeof({t.FullName})")));
                sb.AppendLine("],");
                var roles = auth.Where(static t => t.Role is not null).ToArray();
                if (roles.Length == 0)
                {
                    sb.AppendLine("        roles: null);");
                }
                else
                {
                    sb.AppendLine("        roles: new global::System.Collections.Generic.Dictionary<global::System.Type, string>");
                    sb.AppendLine("        {");
                    foreach (var target in roles)
                    {
                        sb.Append("            [typeof(").Append(target.FullName).Append(")] = \"").Append(target.Role).AppendLine("\",");
                    }

                    sb.AppendLine("        });");
                }
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static bool HasAttribute(ISymbol symbol, string name)
            => symbol.GetAttributes().Any(a => a.AttributeClass?.Name == name);

        private static bool IsSensitive(IFieldSymbol field)
        {
            foreach (var data in field.GetAttributes())
            {
                if (data.AttributeClass?.Name == "PersistStateAttribute")
                {
                    foreach (var named in data.NamedArguments)
                    {
                        if (named.Key == "Sensitive" && named.Value.Value is true)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static string[] GetNotifyAlso(IFieldSymbol field)
            => [.. field.GetAttributes()
                .Where(a => a.AttributeClass?.Name == "NotifyAlsoAttribute")
                .Select(a => a.ConstructorArguments.Length > 0 ? a.ConstructorArguments[0].Value?.ToString() : null)
                .Where(s => !string.IsNullOrEmpty(s))!];

        private static string? GetStringArgument(INamedTypeSymbol type, string attribute)
        {
            foreach (var data in type.GetAttributes())
            {
                if (data.AttributeClass?.Name == attribute && data.ConstructorArguments.Length > 0)
                {
                    return data.ConstructorArguments[0].Value?.ToString();
                }
            }

            return null;
        }

        private static string? GetTypeArgument(INamedTypeSymbol type, string attribute)
        {
            foreach (var data in type.GetAttributes())
            {
                if (data.AttributeClass?.Name == attribute && data.ConstructorArguments.Length > 0
                    && data.ConstructorArguments[0].Value is INamedTypeSymbol named)
                {
                    return named.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
            }

            return null;
        }

        private static string ToPropertyName(string fieldName)
        {
            var name = fieldName.TrimStart('_');
            return name.Length == 0 ? fieldName : char.ToUpperInvariant(name[0]) + name.Substring(1);
        }
    }

    internal readonly record struct NotifyField(string FieldName, string PropertyName, string Type, string[] Also);

    internal readonly record struct PersistField(string FieldName, string Type);

    internal sealed class CommandMethod
    {
        public required string MethodName { get; init; }
        public required string PropertyName { get; init; }
        public required string BackingField { get; init; }
        public required bool IsAsync { get; init; }
        public string? CanExecute { get; init; }

        public static CommandMethod Sync(IMethodSymbol method)
            => Create(method, async: false);

        public static CommandMethod Async(IMethodSymbol method)
            => Create(method, async: true);

        private static CommandMethod Create(IMethodSymbol method, bool async)
        {
            var name = method.Name;
            if (async && name.EndsWith("Async", StringComparison.Ordinal))
            {
                name = name.Substring(0, name.Length - 5);
            }

            var can = method.GetAttributes()
                .First(a => a.AttributeClass?.Name is "ModelCommandAttribute" or "AsyncModelCommandAttribute")
                .NamedArguments.FirstOrDefault(a => a.Key == "CanExecute").Value.Value as string;

            return new CommandMethod
            {
                MethodName = method.Name,
                PropertyName = name + "Command",
                BackingField = "_" + char.ToLowerInvariant(name[0]) + name.Substring(1) + "Command",
                IsAsync = async,
                CanExecute = string.IsNullOrWhiteSpace(can) ? null : can
            };
        }
    }
}
