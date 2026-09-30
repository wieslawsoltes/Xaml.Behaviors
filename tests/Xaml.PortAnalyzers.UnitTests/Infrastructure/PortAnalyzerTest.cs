// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Xaml.PortAnalyzers.UnitTests.Infrastructure;

/// <summary>
/// Runs analyzers and code fixes over an in-memory project.
/// </summary>
internal sealed class PortAnalyzerTest
{
    public const string ProjectDirectory = "/repo/src/Project";

    private readonly List<(string Path, string Source)> _sources = new();
    private readonly List<AdditionalText> _additionalFiles = new();
    private readonly Dictionary<string, string> _globalOptions = new()
    {
        ["build_property.MSBuildProjectDirectory"] = ProjectDirectory,
    };
    private readonly Dictionary<string, IReadOnlyDictionary<string, string>> _fileOptions = new();
    private readonly List<string> _preprocessorSymbols = new();

    /// <summary>
    /// Gets or sets a value indicating whether compiler errors fail the test.
    /// </summary>
    public bool AllowCompilerErrors { get; set; }

    public static string PathOf(string relativePath) => ProjectDirectory + "/" + relativePath;

    public PortAnalyzerTest WithSource(string source) => WithSource("Test0.cs", source);

    public PortAnalyzerTest WithSource(string relativePath, string source)
    {
        _sources.Add((PathOf(relativePath), source));
        return this;
    }

    public PortAnalyzerTest WithPortMap(string json, string relativePath = "port.xamlport.json")
    {
        _additionalFiles.Add(new TestAdditionalText(PathOf(relativePath), json));
        return this;
    }

    public PortAnalyzerTest WithAdditionalFile(string relativePath, string text)
    {
        _additionalFiles.Add(new TestAdditionalText(PathOf(relativePath), text));
        return this;
    }

    public PortAnalyzerTest WithGlobalOption(string key, string value)
    {
        _globalOptions[key] = value;
        return this;
    }

    public PortAnalyzerTest WithoutGlobalOption(string key)
    {
        _globalOptions.Remove(key);
        return this;
    }

    public PortAnalyzerTest WithFileOption(string relativePath, string key, string value)
    {
        var path = PathOf(relativePath);
        var options = _fileOptions.TryGetValue(path, out var existing)
            ? new Dictionary<string, string>(existing)
            : new Dictionary<string, string>();
        options[key] = value;
        _fileOptions[path] = options;
        return this;
    }

    public PortAnalyzerTest WithPreprocessorSymbols(params string[] symbols)
    {
        _preprocessorSymbols.AddRange(symbols);
        return this;
    }

    public async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(params DiagnosticAnalyzer[] analyzers)
    {
        var parseOptions = CreateParseOptions();
        var trees = _sources.Select(s => CSharpSyntaxTree.ParseText(s.Source, parseOptions, s.Path, Encoding.UTF8)).ToArray();
        var compilation = CSharpCompilation.Create("TestAssembly", trees, TestReferences.All, CreateCompilationOptions());
        return await RunAnalyzersAsync(compilation, analyzers, CancellationToken.None);
    }

    public async Task<string> ApplyCodeFixesAsync(DiagnosticAnalyzer analyzer, CodeFixProvider codeFix, string relativePath = "Test0.cs")
    {
        using var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var solution = workspace.CurrentSolution.AddProject(ProjectInfo.Create(
            projectId,
            VersionStamp.Default,
            "Project",
            "TestAssembly",
            LanguageNames.CSharp,
            compilationOptions: CreateCompilationOptions(),
            parseOptions: CreateParseOptions(),
            metadataReferences: TestReferences.All));

        var documentIds = new Dictionary<string, DocumentId>();
        foreach (var (path, source) in _sources)
        {
            var id = DocumentId.CreateNewId(projectId);
            documentIds[path] = id;
            solution = solution.AddDocument(id, System.IO.Path.GetFileName(path), SourceText.From(source, Encoding.UTF8), filePath: path);
        }

        var targetPath = PathOf(relativePath);
        var targetId = documentIds[targetPath];
        for (var iteration = 0; iteration < 20; iteration++)
        {
            var compilation = await solution.GetProject(projectId)!.GetCompilationAsync();
            var diagnostics = await RunAnalyzersAsync(compilation!, [analyzer], CancellationToken.None);
            var diagnostic = diagnostics.FirstOrDefault(d =>
                codeFix.FixableDiagnosticIds.Contains(d.Id) && d.Location.SourceTree?.FilePath == targetPath);
            if (diagnostic is null)
            {
                break;
            }

            var document = solution.GetDocument(targetId)!;
            var actions = new List<CodeAction>();
            await codeFix.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));
            Assert.NotEmpty(actions);
            var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
            solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
        }

        var text = await solution.GetDocument(targetId)!.GetTextAsync();
        return text.ToString();
    }

    private async Task<ImmutableArray<Diagnostic>> RunAnalyzersAsync(Compilation compilation, DiagnosticAnalyzer[] analyzers, CancellationToken cancellationToken)
    {
        if (!AllowCompilerErrors)
        {
            var errors = compilation.GetDiagnostics(cancellationToken).Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
            Assert.True(errors.Length == 0, "Test source does not compile:\n" + string.Join("\n", errors.Select(e => e.ToString())));
        }

        var exceptions = new List<Exception>();
        var options = new AnalyzerOptions(
            _additionalFiles.ToImmutableArray(),
            new TestAnalyzerConfigOptionsProvider(_globalOptions, _fileOptions));
        var withAnalyzers = compilation.WithAnalyzers(
            analyzers.ToImmutableArray(),
            new CompilationWithAnalyzersOptions(
                options,
                onAnalyzerException: (exception, _, _) => { lock (exceptions) { exceptions.Add(exception); } },
                concurrentAnalysis: true,
                logAnalyzerExecutionTime: false));
        var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken);
        Assert.True(exceptions.Count == 0, "Analyzer threw:\n" + string.Join("\n", exceptions));
        return diagnostics
            .OrderBy(d => d.Location.SourceTree?.FilePath ?? d.Location.GetLineSpan().Path, StringComparer.Ordinal)
            .ThenBy(d => d.Location.SourceSpan.Start)
            .ToImmutableArray();
    }

    private CSharpParseOptions CreateParseOptions()
        => new(LanguageVersion.Latest, DocumentationMode.Parse, SourceCodeKind.Regular, _preprocessorSymbols);

    private static CSharpCompilationOptions CreateCompilationOptions()
        => new(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable);
}
