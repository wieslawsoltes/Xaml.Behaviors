using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xaml.PropertyGenerator.CodeFixes;
using Xaml.PropertyGenerator.Migration;
using Xunit;

namespace Xaml.PropertyGenerator.UnitTests;

public class MigrationTests
{
    private const string Source = """
        // Header comment.
        using System.Collections.Generic;
        using Avalonia;
        using Avalonia.Data;
        using Avalonia.Metadata;

        namespace Sample;

        public class Owner : AvaloniaObject
        {
            /// <summary>
            /// Identifies the <seealso cref="IsActive"/> avalonia property.
            /// </summary>
        #if UNO
            public static readonly DependencyProperty IsActiveProperty =
        #else
            public static readonly StyledProperty<bool> IsActiveProperty =
        #endif
                AvaloniaProperty.Register<Owner, bool>(nameof(IsActive), defaultValue: true);

            public static readonly StyledProperty<string> TextProperty =
                AvaloniaProperty.Register<Owner, string>(nameof(Text), string.Empty, defaultBindingMode: BindingMode.TwoWay);

            public static readonly StyledProperty<System.TimeSpan> DelayProperty =
                AvaloniaProperty.Register<Owner, System.TimeSpan>(nameof(Delay), System.TimeSpan.FromSeconds(1));

            public static readonly StyledProperty<object?> TargetProperty =
                AvaloniaProperty.Register<Owner, object?>(nameof(Target));

            public static readonly DirectProperty<Owner, bool> CanExecuteProperty =
                AvaloniaProperty.RegisterDirect<Owner, bool>(nameof(CanExecute), o => o.CanExecute);

            public static readonly DirectProperty<Owner, List<int>> ItemsProperty =
                AvaloniaProperty.RegisterDirect<Owner, List<int>>(nameof(Items), o => o.Items);

            public static readonly StyledProperty<int> ValidatedProperty =
                AvaloniaProperty.Register<Owner, int>(nameof(Validated), validate: v => v > 0);

            public static readonly DirectProperty<Owner, int> SharedProperty =
                AvaloniaProperty.RegisterDirect<Owner, int>(nameof(Shared), o => o.Shared, (o, v) => o.Shared = v);

            private bool _canExecute = true;
            private List<int>? _items;
            private int _shared;

            /// <summary>Gets or sets a value indicating whether the owner is active.</summary>
            public bool IsActive
            {
                get => (bool)GetValue(IsActiveProperty);
                set => SetValue(IsActiveProperty, value);
            }

            public string Text
            {
                get => GetValue(TextProperty);
                set => SetValue(TextProperty, value);
            }

            public System.TimeSpan Delay
            {
                get { return GetValue(DelayProperty); }
                set { SetValue(DelayProperty, value); }
            }

            [Content]
            public object? Target
            {
                get => GetValue(TargetProperty);
                set => SetValue(TargetProperty, value);
            }

            public bool CanExecute
            {
                get => _canExecute;
                private set => SetAndRaise(CanExecuteProperty, ref _canExecute, value);
            }

            public List<int> Items => _items ??= [];

            public int Validated
            {
                get => GetValue(ValidatedProperty);
                set => SetValue(ValidatedProperty, value);
            }

            public int Shared
            {
                get => _shared;
                set => SetAndRaise(SharedProperty, ref _shared, value);
            }

            public void Touch() => _shared++;
        }
        """;

    [Fact]
    public async Task Analyzer_Reports_Only_Convertible_Properties()
    {
        var document = CreateDocument(Source);
        var diagnostics = await GetDiagnosticsAsync(document);

        var names = diagnostics.Select(static d => d.Properties[MigrationAnalyzer.PropertyNameKey]!).OrderBy(static n => n).ToArray();
        Assert.Equal(["CanExecute", "Delay", "IsActive", "Items", "Target", "Text"], names);
    }

    [Fact]
    public async Task FixAll_Converts_Properties_And_Preserves_Public_Api()
    {
        var document = CreateDocument(Source);
        var before = await GetPublicApiAsync(document.Project);

        var fixedDocument = await FixAllAsync(document);
        var text = (await fixedDocument.GetTextAsync()).ToString();

        Assert.Contains("public partial class Owner : AvaloniaObject", text);
        Assert.Contains("[StyledProperty(DefaultValue = true)]", text);
        Assert.Contains("[StyledProperty(DefaultValue = \"\", DefaultBindingMode = PropertyBindingMode.TwoWay)]", text);
        Assert.Contains("[StyledProperty(DefaultValueExpression = \"System.TimeSpan.FromSeconds(1)\")]", text);
        Assert.Contains("[StyledProperty(Content = true)]", text);
        Assert.Contains("[DirectProperty(DefaultValue = true)]", text);
        Assert.Contains("public partial bool CanExecute { get; private set; }", text);
        Assert.Contains("[DirectProperty(Lazy = true)]", text);
        Assert.Contains("/// <summary>Gets or sets a value indicating whether the owner is active.</summary>", text);
        Assert.Contains("using Xaml.PropertyGenerator;", text);
        Assert.StartsWith("// Header comment.", text);
        Assert.DoesNotContain("#if UNO", text);
        Assert.DoesNotContain("_canExecute", text);
        Assert.DoesNotContain("_items", text);
        Assert.Contains("ValidatedProperty =", text);
        Assert.Contains("_shared", text);

        var after = await GetPublicApiAsync(fixedDocument.Project);
        Assert.Equal(before, after);
    }

    private static Document CreateDocument(string source)
    {
        var workspace = new AdhocWorkspace();
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        var project = workspace.AddProject(ProjectInfo.Create(
            ProjectId.CreateNewId(),
            VersionStamp.Default,
            "Migration",
            "Migration",
            LanguageNames.CSharp,
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable),
            parseOptions: parseOptions,
            metadataReferences: GetReferences(),
            analyzerReferences: [new GeneratorReference()]));
        return project.AddDocument("Owner.cs", SourceText.From(source));
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(Document document)
    {
        var compilation = (await document.Project.GetCompilationAsync())!;
        var withAnalyzers = compilation.WithAnalyzers([new MigrationAnalyzer()]);
        return (await withAnalyzers.GetAnalyzerDiagnosticsAsync()).Where(static d => d.Id == MigrationAnalyzer.DiagnosticId).ToImmutableArray();
    }

    private static async Task<Document> FixAllAsync(Document document)
    {
        var provider = new MigrationCodeFixProvider();
        var diagnostics = await GetDiagnosticsAsync(document);
        var context = new FixAllContext(
            document,
            provider,
            FixAllScope.Document,
            MigrationAnalyzer.DiagnosticId,
            [MigrationAnalyzer.DiagnosticId],
            new FixedDiagnosticProvider(diagnostics),
            CancellationToken.None);
        var action = await provider.GetFixAllProvider().GetFixAsync(context);
        Assert.NotNull(action);
        var operations = await action!.GetOperationsAsync(CancellationToken.None);
        var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
        return solution.GetDocument(document.Id)!;
    }

    private static async Task<string[]> GetPublicApiAsync(Project project)
    {
        var compilation = (await project.GetCompilationAsync())!;
        var errors = compilation.GetDiagnostics().Where(static d => d.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.Empty(errors);

        var owner = compilation.GetTypeByMetadataName("Sample.Owner")!;
        return owner.GetMembers()
            .Where(static m => m.DeclaredAccessibility is Accessibility.Public or Accessibility.Protected && !m.IsImplicitlyDeclared)
            .Select(static m => m.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) + " : " + m.Kind + (m is IPropertySymbol { SetMethod: { } set } ? " set:" + set.DeclaredAccessibility : string.Empty))
            .OrderBy(static s => s, StringComparer.Ordinal)
            .ToArray();
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

        yield return MetadataReference.CreateFromFile(typeof(Avalonia.AvaloniaObject).Assembly.Location);
        yield return MetadataReference.CreateFromFile(typeof(Avalonia.Controls.Control).Assembly.Location);
    }

    private sealed class GeneratorReference : AnalyzerReference
    {
        private static readonly ImmutableArray<DiagnosticAnalyzer> s_analyzers = [new MigrationAnalyzer()];
        private static readonly ImmutableArray<ISourceGenerator> s_generators = [new PropertyGenerator().AsSourceGenerator()];

        public override string FullPath => "Xaml.PropertyGenerator";

        public override object Id => FullPath;

        public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(string language) => s_analyzers;

        public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzersForAllLanguages() => s_analyzers;

        public override ImmutableArray<ISourceGenerator> GetGenerators(string language) => s_generators;

        public override ImmutableArray<ISourceGenerator> GetGeneratorsForAllLanguages() => s_generators;
    }

    private sealed class FixedDiagnosticProvider(ImmutableArray<Diagnostic> diagnostics) : FixAllContext.DiagnosticProvider
    {
        public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            => Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);

        public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
            => Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            => Task.FromResult<IEnumerable<Diagnostic>>([]);
    }
}
