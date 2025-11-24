using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xaml.Behaviors.SourceGenerators;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

public static class GeneratorTestHelper
{
    public static (ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<string> GeneratedSources) RunGenerator(string source)
    {
        source = NormalizeAssemblyAttributes(source);

        var attributeSource = @"
using System;

namespace Xaml.Behaviors.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedActionAttribute : Attribute
    {
        public bool UseDispatcher { get; set; }

        public GenerateTypedActionAttribute() { }
        public GenerateTypedActionAttribute(Type targetType, string methodName) { }
    }

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedTriggerAttribute : Attribute
    {
        public GenerateTypedTriggerAttribute() { }
        public GenerateTypedTriggerAttribute(Type targetType, string eventName) { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedChangePropertyActionAttribute : Attribute
    {
        public bool UseDispatcher { get; set; }

        public GenerateTypedChangePropertyActionAttribute() { }
        public GenerateTypedChangePropertyActionAttribute(Type targetType, string propertyName) { }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal class GenerateTypedDataTriggerAttribute : Attribute
    {
        public GenerateTypedDataTriggerAttribute(Type type) { }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class GenerateTypedMultiDataTriggerAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class TriggerPropertyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class GenerateTypedInvokeCommandActionAttribute : Attribute
    {
        public bool UseDispatcher { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class ActionCommandAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class ActionParameterAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class GeneratePropertyTriggerAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? SourceName { get; set; }
        public bool UseDispatcher { get; set; }

        public GeneratePropertyTriggerAttribute() { }
        public GeneratePropertyTriggerAttribute(Type targetType, string propertyName) { }
    }

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class GenerateEventCommandAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? ParameterPath { get; set; }
        public bool UseDispatcher { get; set; }

        public GenerateEventCommandAttribute() { }
        public GenerateEventCommandAttribute(Type targetType, string eventName) { }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class GenerateEventArgsActionAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? Project { get; set; }
        public bool UseDispatcher { get; set; }

        public GenerateEventArgsActionAttribute() { }
        public GenerateEventArgsActionAttribute(Type targetType, string methodName) { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class GenerateAsyncTriggerAttribute : Attribute
    {
        public string? Name { get; set; }
        public bool UseDispatcher { get; set; } = true;
        public bool FireOnAttach { get; set; } = true;

        public GenerateAsyncTriggerAttribute() { }
        public GenerateAsyncTriggerAttribute(Type targetType, string propertyName) { }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class GenerateObservableTriggerAttribute : Attribute
    {
        public string? Name { get; set; }
        public bool UseDispatcher { get; set; } = true;
        public bool FireOnAttach { get; set; } = true;

        public GenerateObservableTriggerAttribute() { }
        public GenerateObservableTriggerAttribute(Type targetType, string propertyName) { }
    }
}
";

        var sourceDefinesAttributes = source.Contains("class GenerateTyped", StringComparison.Ordinal);
        var attributeTree = sourceDefinesAttributes ? null : CSharpSyntaxTree.ParseText(attributeSource);
        var sourceTree = CSharpSyntaxTree.ParseText(source);
        var syntaxTrees = attributeTree is null
            ? new[] { sourceTree }
            : new[] { attributeTree, sourceTree };
        
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Avalonia.AvaloniaObject).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Avalonia.Input.InputElement).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Avalonia.Controls.Control).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Avalonia.Interactivity.RoutedEventArgs).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Avalonia.Xaml.Interactivity.Interaction).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.ComponentModel.INotifyPropertyChanged).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Windows.Input.ICommand).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.EventHandler).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.ObjectModel").Location),
        };

        var compilation = CSharpCompilation.Create(
            "Tests",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var attributeInfo = compilation.Assembly.GetAttributes()
            .Select(a => $"{a.AttributeClass?.ToDisplayString()}({string.Join(",", a.ConstructorArguments.Select(c => c.Value?.ToString() ?? "<null>"))})");
        Console.Error.WriteLine("Assembly attributes: " + string.Join(" | ", attributeInfo));

        var generator = new XamlBehaviorsGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

        var result = driver.GetRunResult();
        var generatedAttributeTrees = outputCompilation.SyntaxTrees
            .Where(t => t.FilePath?.Contains("Attribute.g.cs", StringComparison.Ordinal) == true)
            .ToArray();
        outputCompilation = outputCompilation.RemoveSyntaxTrees(generatedAttributeTrees);
        var compilationDiagnostics = outputCompilation
            .GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error && !IsGeneratorAttributeDuplicate(d))
            .ToImmutableArray();
        var generatorErrors = generatorDiagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Where(d => !IsGeneratorAttributeDuplicate(d))
            .ToImmutableArray();
        var diagnostics = generatorErrors.AddRange(compilationDiagnostics);
        
        var generatedSources = result.Results[0].GeneratedSources
            .Select(s => s.SourceText.ToString())
            .ToImmutableArray();

        if (Environment.GetEnvironmentVariable("GENERATOR_TEST_DEBUG") == "1")
        {
            Console.Error.WriteLine("Input source:");
            Console.Error.WriteLine(source);

            var attributeSyntax = syntaxTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>())
                .Where(a => a.Name.ToString().Contains("GenerateTypedDataTrigger", StringComparison.Ordinal))
                .Select(a => a.ToString());
            Console.Error.WriteLine("DataTrigger attribute syntax: " + string.Join(" | ", attributeSyntax));

            var allAttributes = syntaxTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>())
                .Select(a => a.ToString());
            Console.Error.WriteLine("All attributes: " + string.Join(" | ", allAttributes));

            var attributeLists = syntaxTrees
                .SelectMany(t => t.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>())
                .Select(a => a.ToString());
            Console.Error.WriteLine("Attribute lists: " + string.Join(" | ", attributeLists));

            for (var i = 0; i < syntaxTrees.Length; i++)
            {
                var tree = syntaxTrees[i];
                Console.Error.WriteLine($"Syntax tree #{i} diagnostics: {string.Join(", ", tree.GetDiagnostics())}");
                Console.Error.WriteLine($"Syntax tree #{i} text:");
                Console.Error.WriteLine(tree.ToString());
            }

            Console.Error.WriteLine("Diagnostics:");
            foreach (var d in result.Diagnostics)
            {
                Console.Error.WriteLine($"  {d}");
            }
            Console.Error.WriteLine($"Generator exception: {result.Results[0].Exception}");
            Console.Error.WriteLine("Generator diagnostics:");
            foreach (var d in result.Results[0].Diagnostics)
            {
                Console.Error.WriteLine($"  {d}");
            }

            Console.Error.WriteLine($"Generated sources ({generatedSources.Length}):");
            for (var i = 0; i < generatedSources.Length; i++)
            {
                Console.Error.WriteLine($"-- Source #{i} --");
                Console.Error.WriteLine(generatedSources[i]);
                Console.Error.WriteLine("--------------");
            }
        }

        return (diagnostics, generatedSources);
    }

    private static bool IsGeneratorAttributeDuplicate(Diagnostic diagnostic)
    {
        if (diagnostic.Id != "CS0101" || diagnostic.Location == Location.None || diagnostic.Location.IsInMetadata)
        {
            return false;
        }

        var path = diagnostic.Location.SourceTree?.FilePath;
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        return path.Contains("GenerateTyped", StringComparison.Ordinal) && path.Contains("Attribute.g.cs", StringComparison.Ordinal);
    }

    private static string NormalizeAssemblyAttributes(string source)
    {
        var lines = source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var assemblyAttributes = new List<string>();
        var remaining = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith("[assembly:", StringComparison.Ordinal))
            {
                assemblyAttributes.Add(line);
            }
            else
            {
                remaining.Add(line);
            }
        }

        if (assemblyAttributes.Count == 0)
        {
            return source;
        }

        var insertIndex = 0;
        while (insertIndex < remaining.Count)
        {
            var trimmed = remaining[insertIndex].TrimStart();
            if (string.IsNullOrWhiteSpace(trimmed) ||
                trimmed.StartsWith("using ", StringComparison.Ordinal) ||
                trimmed.StartsWith("extern ", StringComparison.Ordinal))
            {
                insertIndex++;
                continue;
            }
            break;
        }

        remaining.InsertRange(insertIndex, assemblyAttributes);
        return string.Join("\n", remaining);
    }
}
