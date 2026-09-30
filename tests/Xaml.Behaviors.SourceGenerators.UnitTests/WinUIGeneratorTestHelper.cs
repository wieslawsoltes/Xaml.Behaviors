using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xaml.Behaviors.SourceGenerators;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

/// <summary>
/// The result of running the generator on a WinUI (Uno Platform) compilation.
/// </summary>
/// <param name="GeneratorDiagnostics">Diagnostics reported by the generator.</param>
/// <param name="CompilationErrors">Errors of the compilation including the generated sources.</param>
/// <param name="GeneratedCodeWarnings">Compiler warnings reported in generated sources.</param>
/// <param name="GeneratedSources">The generated sources (attribute sources excluded), keyed by hint name.</param>
public sealed record WinUIGeneratorRun(
    ImmutableArray<Diagnostic> GeneratorDiagnostics,
    ImmutableArray<Diagnostic> CompilationErrors,
    ImmutableArray<Diagnostic> GeneratedCodeWarnings,
    ImmutableDictionary<string, string> GeneratedSources)
{
    /// <summary>Gets all generated sources concatenated.</summary>
    public string AllSources => string.Join("\n", GeneratedSources.OrderBy(static s => s.Key, StringComparer.Ordinal).Select(static s => s.Value));
}

/// <summary>
/// Runs <see cref="XamlBehaviorsGenerator"/> on sources compiled against the Uno Platform WinUI reference assemblies
/// and the Uno Interactivity assembly (compile-only: nothing is loaded into the test process).
/// </summary>
public static class WinUIGeneratorTestHelper
{
    private const string OptionPrefix = "Xaml.Behaviors.SourceGenerators.UnitTests.";

    private static readonly CSharpParseOptions s_parseOptions = new(LanguageVersion.Preview);

    /// <summary>Runs the generator on <paramref name="source"/> with WinUI references.</summary>
    /// <param name="source">The user source.</param>
    /// <param name="platformOverride">The value of the <c>XamlBehaviorsSourceGeneratorPlatform</c> MSBuild property, if any.</param>
    public static WinUIGeneratorRun Run([StringSyntax("C#")] string source, string? platformOverride = null)
    {
        var compilation = CSharpCompilation.Create(
            "WinUIGeneratorTests",
            [CSharpSyntaxTree.ParseText(source, s_parseOptions)],
            GetReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var optionsProvider = new TestAnalyzerConfigOptionsProvider(
            platformOverride is null
                ? ImmutableDictionary<string, string>.Empty
                : ImmutableDictionary<string, string>.Empty.Add("build_property.XamlBehaviorsSourceGeneratorPlatform", platformOverride));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new XamlBehaviorsGenerator().AsSourceGenerator()],
            parseOptions: s_parseOptions,
            optionsProvider: optionsProvider);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
        var result = driver.GetRunResult();
        var generated = result.Results
            .SelectMany(static r => r.GeneratedSources)
            .Where(static s => !s.HintName.EndsWith("Attribute.g.cs", StringComparison.Ordinal))
            .ToImmutableDictionary(static s => s.HintName, static s => s.SourceText.ToString());

        var outputDiagnostics = output.GetDiagnostics();
        var errors = outputDiagnostics
            .Where(static d => d.Severity == DiagnosticSeverity.Error)
            .ToImmutableArray();
        var generatedWarnings = outputDiagnostics
            .Where(static d => d.Severity == DiagnosticSeverity.Warning &&
                               d.Location.SourceTree?.FilePath.EndsWith(".g.cs", StringComparison.Ordinal) == true)
            .ToImmutableArray();

        return new WinUIGeneratorRun(diagnostics.AddRange(result.Diagnostics).Distinct().ToImmutableArray(), errors, generatedWarnings, generated);
    }

    /// <summary>Formats diagnostics and generated sources for assertion messages.</summary>
    public static string Describe(WinUIGeneratorRun run)
    {
        return string.Join(Environment.NewLine, run.GeneratorDiagnostics.Concat(run.CompilationErrors).Concat(run.GeneratedCodeWarnings).Select(static d => d.ToString()))
               + Environment.NewLine + run.AllSources;
    }

    private static IEnumerable<MetadataReference> GetReferences()
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

        var root = GetOption("NuGetPackageRoot");
        var version = GetOption("UnoWinUIVersion");
        foreach (var (package, file) in new[]
                 {
                     ("uno.winui", "Uno.UI.dll"),
                     ("uno.winui", "Uno.UI.Composition.dll"),
                     ("uno.foundation", "Uno.Foundation.dll"),
                     ("uno.winrt", "Uno.dll"),
                     ("uno.winrt", "Uno.UI.Dispatching.dll"),
                 })
        {
            yield return MetadataReference.CreateFromFile(Path.Combine(root, package, version, "lib", "net10.0", file));
        }

        yield return MetadataReference.CreateFromFile(GetOption("UnoInteractivityAssembly"));
    }

    private static string GetOption(string name)
    {
        return AppContext.GetData(OptionPrefix + name) as string
               ?? throw new InvalidOperationException($"Runtime option '{OptionPrefix}{name}' is missing; rebuild the test project.");
    }

    private sealed class TestAnalyzerConfigOptionsProvider(ImmutableDictionary<string, string> globalOptions) : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions { get; } = new TestAnalyzerConfigOptions(globalOptions);

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => TestAnalyzerConfigOptions.Empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => TestAnalyzerConfigOptions.Empty;
    }

    private sealed class TestAnalyzerConfigOptions(ImmutableDictionary<string, string> options) : AnalyzerConfigOptions
    {
        public static TestAnalyzerConfigOptions Empty { get; } = new(ImmutableDictionary<string, string>.Empty);

        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => options.TryGetValue(key, out value);
    }
}
