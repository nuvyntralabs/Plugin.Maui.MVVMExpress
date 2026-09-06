using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Plugin.Maui.MVVMExpress.Templates.Tests;

public sealed class TemplateSmokeTests
{
    [Fact]
    public void Template_ContainsExpectedFiles()
    {
        var root = FindRepoRoot();
        var template = Path.Combine(root, "templates", "maui-app");
        Assert.True(File.Exists(Path.Combine(template, ".template.config", "template.json")));
        Assert.True(File.Exists(Path.Combine(template, "MauiApp1.sln")));
        Assert.True(File.Exists(Path.Combine(template, "MauiApp1", "MauiProgram.cs")));
        Assert.True(File.Exists(Path.Combine(template, "MauiApp1.Core", "MainPageViewModel.cs")));
        Assert.True(File.Exists(Path.Combine(template, "MauiApp1", "Pages", "MainPage.xaml")));
        Assert.True(File.Exists(Path.Combine(template, "MauiApp1.Core", "LoginViewModel.cs")));
        Assert.True(File.Exists(Path.Combine(root, "templates", "page", ".template.config", "template.json")));
        Assert.True(File.Exists(Path.Combine(template, "MauiApp1.Tests", "AppViewModelTests.cs")));
        Assert.True(File.Exists(Path.Combine(template, "Directory.Packages.props")));
    }

    [Fact]
    public void Instantiated_CoreAndTests_Pass()
    {
        var root = FindRepoRoot();
        var source = Path.Combine(root, "templates", "maui-app");
        var output = Path.Combine(root, "artifacts", "template-smoke", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(output);
        try
        {
            CopyTemplate(source, output);
            RewriteToProjectReferences(output, root);
            File.Copy(
                Path.Combine(root, "nuget.config"),
                Path.Combine(output, "nuget.config"),
                overwrite: true);
            File.WriteAllText(
                Path.Combine(output, "Directory.Build.props"),
                """
                <Project>
                  <PropertyGroup>
                    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
                  </PropertyGroup>
                </Project>
                """);
            File.WriteAllText(
                Path.Combine(output, "Directory.Packages.props"),
                """
                <Project>
                  <PropertyGroup>
                    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                  </PropertyGroup>
                </Project>
                """);
            var testProject = Path.Combine(output, "MauiApp1.Tests", "MauiApp1.Tests.csproj");
            RunDotNet(["test", testProject, "-c", "Release", "--nologo"], output);
        }
        finally
        {
            TryDelete(output);
        }
    }

    private static void CopyTemplate(string source, string destination)
    {
        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(source, file);
            if (relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Any(part => part is "bin" or "obj" or ".template.config"))
            {
                continue;
            }

            var target = Path.Combine(destination, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target);
        }
    }

    private static void RewriteToProjectReferences(string output, string repoRoot)
    {
        var core = Path.Combine(output, "MauiApp1.Core", "MauiApp1.Core.csproj");
        var tests = Path.Combine(output, "MauiApp1.Tests", "MauiApp1.Tests.csproj");
        File.WriteAllText(core, ReplacePackages(File.ReadAllText(core), Path.GetDirectoryName(core)!, repoRoot, includeGenerators: true));
        File.WriteAllText(tests, ReplacePackages(File.ReadAllText(tests), Path.GetDirectoryName(tests)!, repoRoot, includeGenerators: false));
    }

    private static string ReplacePackages(string csproj, string projectDir, string repoRoot, bool includeGenerators)
    {
        string Ref(string project) => Path.GetRelativePath(projectDir, Path.Combine(repoRoot, "src", project, $"{project}.csproj"));

        csproj = Regex.Replace(
            csproj,
            """\s*<PackageReference Include="Plugin\.Maui\.MVVMExpress\.Core" Version="[^"]+" />""",
            $"""{Environment.NewLine}    <ProjectReference Include="{Ref("Plugin.Maui.MVVMExpress.Core")}" />""");
        csproj = Regex.Replace(
            csproj,
            """\s*<PackageReference Include="Plugin\.Maui\.MVVMExpress\.Pagination" Version="[^"]+" />""",
            $"""{Environment.NewLine}    <ProjectReference Include="{Ref("Plugin.Maui.MVVMExpress.Pagination")}" />""");
        csproj = Regex.Replace(
            csproj,
            """\s*<PackageReference Include="Plugin\.Maui\.MVVMExpress\.Testing" Version="[^"]+" />""",
            $"""{Environment.NewLine}    <ProjectReference Include="{Ref("Plugin.Maui.MVVMExpress.Testing")}" />""");
        if (includeGenerators)
        {
            csproj = Regex.Replace(
                csproj,
                """\s*<PackageReference Include="Plugin\.Maui\.MVVMExpress\.SourceGenerators" Version="[^"]+">\s*<PrivateAssets>all</PrivateAssets>\s*<IncludeAssets>[^<]+</IncludeAssets>\s*</PackageReference>""",
                $"""{Environment.NewLine}    <ProjectReference Include="{Ref("Plugin.Maui.MVVMExpress.SourceGenerators")}" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />""",
                RegexOptions.Singleline);
        }

        return csproj;
    }

    private static void RunDotNet(string[] args, string workingDirectory)
    {
        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (var arg in args)
        {
            start.ArgumentList.Add(arg);
        }

        using var process = Process.Start(start) ?? throw new InvalidOperationException("dotnet failed to start.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"dotnet {string.Join(' ', args)} failed ({process.ExitCode}).{Environment.NewLine}{stdout}{Environment.NewLine}{stderr}");
        }
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "templates", "maui-app", ".template.config", "template.json")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not find the MVVMExpress repository root.");
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
