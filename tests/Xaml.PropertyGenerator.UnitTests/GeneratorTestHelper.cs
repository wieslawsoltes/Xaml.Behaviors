using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Xaml.PropertyGenerator.UnitTests;

internal enum TestPlatform
{
    Avalonia,
    WinUI,
}

internal sealed record GeneratorRun(
    ImmutableArray<Diagnostic> GeneratorDiagnostics,
    ImmutableArray<Diagnostic> CompilationErrors,
    string GeneratedSource);

internal static class GeneratorTestHelper
{
    private static readonly CSharpParseOptions s_parseOptions = new(LanguageVersion.Preview);

    public static GeneratorRun Run(string source, TestPlatform platform) => Run(source, platform, out _);

    /// <summary>
    /// Runs the generator and the trim analyzer of the .NET SDK (ILLink.RoslynAnalyzer, the analyzer behind
    /// <c>EnableTrimAnalyzer</c>) on the output and returns the trimming warnings (IL*).
    /// </summary>
    public static ImmutableArray<Diagnostic> GetTrimWarnings(string source, TestPlatform platform, bool runGenerator = true)
    {
        Compilation compilation = CreateCompilation(source, platform);
        if (runGenerator)
        {
            Run(source, platform, out compilation);
        }

        var analyzers = new AnalyzerFileReference(GetMetadata("ILLinkAnalyzerPath"), new AnalyzerLoader())
            .GetAnalyzers(LanguageNames.CSharp);
        var options = new AnalyzerOptions([], new TrimAnalyzerOptionsProvider());
        var diagnostics = compilation.WithAnalyzers(analyzers, options).GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();
        return diagnostics.Where(static d => d.Id.StartsWith("IL", StringComparison.Ordinal)).ToImmutableArray();
    }

    private static CSharpCompilation CreateCompilation(string source, TestPlatform platform)
        => CSharpCompilation.Create(
            "GeneratorTests",
            [CSharpSyntaxTree.ParseText(source, s_parseOptions)],
            GetReferences(platform),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

    private static GeneratorRun Run(string source, TestPlatform platform, out Compilation output)
    {
        var compilation = CreateCompilation(source, platform);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new PropertyGenerator().AsSourceGenerator()],
            parseOptions: s_parseOptions);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out output, out var diagnostics);
        var result = driver.GetRunResult();
        var generated = string.Join(
            "\n",
            result.Results.SelectMany(static r => r.GeneratedSources)
                .Where(static s => !s.HintName.StartsWith("Xaml.PropertyGenerator.Attributes", StringComparison.Ordinal) &&
                                   !s.HintName.Contains("Embedded", StringComparison.Ordinal))
                .Select(static s => s.SourceText.ToString()));

        var errors = output.GetDiagnostics().Where(static d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();
        return new GeneratorRun(diagnostics.AddRange(result.Diagnostics).Distinct().ToImmutableArray(), errors, generated);
    }

    private static IEnumerable<MetadataReference> GetReferences(TestPlatform platform)
    {
        var trusted = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        foreach (var path in trusted)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (name.StartsWith("System.", StringComparison.Ordinal) || name is "System" or "mscorlib" or "netstandard")
            {
                yield return MetadataReference.CreateFromFile(path);
            }
        }

        if (platform == TestPlatform.Avalonia)
        {
            foreach (var assembly in new[] { typeof(Avalonia.AvaloniaObject).Assembly, typeof(Avalonia.Controls.Control).Assembly })
            {
                yield return MetadataReference.CreateFromFile(assembly.Location);
            }

            yield break;
        }

        var root = GetMetadata("NuGetPackageRoot");
        var version = GetMetadata("UnoWinUIVersion");
        foreach (var (package, file) in new[]
                 {
                     ("uno.winui", "Uno.UI.dll"),
                     ("uno.foundation", "Uno.Foundation.dll"),
                     ("uno.winrt", "Uno.dll"),
                     ("uno.winrt", "Uno.UI.Dispatching.dll"),
                 })
        {
            yield return MetadataReference.CreateFromFile(Path.Combine(root, package, version, "lib", "net10.0", file));
        }
    }

    private sealed class AnalyzerLoader : IAnalyzerAssemblyLoader
    {
        public void AddDependencyLocation(string fullPath)
        {
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Test code loading the trim analyzer of the SDK; never trimmed.")]
        public Assembly LoadFromPath(string fullPath) => AssemblyLoadContext.Default.LoadFromAssemblyPath(fullPath);
    }

    private sealed class TrimAnalyzerOptionsProvider : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions { get; } = new TrimAnalyzerOptions();

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;
    }

    private sealed class TrimAnalyzerOptions : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            value = key == "build_property.EnableTrimAnalyzer" ? "true" : null;
            return value is not null;
        }
    }

    private static string GetMetadata(string key)
        => typeof(GeneratorTestHelper).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().First(a => a.Key == key).Value!;
}
