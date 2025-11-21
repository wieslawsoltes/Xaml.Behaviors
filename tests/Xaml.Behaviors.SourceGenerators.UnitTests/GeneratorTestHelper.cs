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
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
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
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new XamlBehaviorsGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        var result = driver.GetRunResult();
        
        var generatedSources = result.Results[0].GeneratedSources
            .Select(s => s.SourceText.ToString())
            .ToImmutableArray();

        return (result.Diagnostics, generatedSources);
    }
}
