using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CheckDocs;

class Program
{
    static async Task Main(string[] args)
    {
        // Register MSBuild
        MSBuildLocator.RegisterDefaults();

        var rootDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var solutionPath = ResolveSolutionPath(rootDir);
        var docsPath = Path.Combine(rootDir, "docfx/articles");

        if (solutionPath is null)
        {
            Console.WriteLine($"Error: Solution not found at {Path.Combine(rootDir, "AvaloniaBehaviors.sln")} or {Path.Combine(rootDir, "AvaloniaBehaviors.slnx")}");
            return;
        }

        Console.WriteLine($"Loading solution: {solutionPath}");
        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(solutionPath);

        var classesToCheck = new HashSet<string>();

        Console.WriteLine("Scanning solution for Behaviors, Actions, and Triggers...");

        foreach (var project in solution.Projects)
        {
            // Skip test projects and tools
            if (project.Name.Contains("Test") || project.Name.Contains("Demo") || project.Name.Contains("Sample") || project.Name == "CheckDocs")
                continue;

            var compilation = await project.GetCompilationAsync();
            if (compilation == null) continue;

            // Find base types
            var iBehavior = compilation.GetTypeByMetadataName("Avalonia.Xaml.Interactivity.IBehavior");
            var iAction = compilation.GetTypeByMetadataName("Avalonia.Xaml.Interactivity.IAction");

            if (iBehavior == null && iAction == null)
            {
                // Maybe this project doesn't reference Interactivity, skip
                continue;
            }

            foreach (var document in project.Documents)
            {
                var semanticModel = await document.GetSemanticModelAsync();
                if (semanticModel == null) continue;

                var root = await document.GetSyntaxRootAsync();
                if (root == null) continue;

                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classDecl in classDeclarations)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(classDecl);
                    if (symbol == null || symbol.IsAbstract || symbol.DeclaredAccessibility != Accessibility.Public)
                        continue;

                    bool isMatch = false;

                    if (iBehavior != null && symbol.AllInterfaces.Contains(iBehavior))
                    {
                        isMatch = true;
                    }
                    else if (iAction != null && symbol.AllInterfaces.Contains(iAction))
                    {
                        isMatch = true;
                    }

                    if (isMatch)
                    {
                        classesToCheck.Add(symbol.Name);
                    }
                }
            }
        }

        Console.WriteLine($"Found {classesToCheck.Count} classes.");

        Console.WriteLine("Scanning documentation...");
        var documentedClasses = ScanDocs(docsPath, classesToCheck);

        foreach (var kvp in documentedClasses.OrderBy(x => x.Key))
        {
             Console.WriteLine($"[OK] {kvp.Key} -> {string.Join(", ", kvp.Value.Select(p => Path.GetFileName(p)))}");
        }

        var missingClasses = classesToCheck
            .Where(c => !documentedClasses.ContainsKey(c))
            .OrderBy(c => c)
            .ToList();

        if (missingClasses.Any())
        {
            Console.WriteLine($"\nMissing documentation for {missingClasses.Count} classes:");
            foreach (var cls in missingClasses)
            {
                Console.WriteLine($"- {cls}");
            }
            Environment.Exit(1);
        }
        else
        {
            Console.WriteLine("\nAll classes appear to be documented!");
        }
    }

    static Dictionary<string, List<string>> ScanDocs(string docsPath, HashSet<string> classesToCheck)
    {
        var documentedClasses = new Dictionary<string, List<string>>();

        if (!Directory.Exists(docsPath))
        {
            Console.WriteLine($"Warning: Docs path not found: {docsPath}");
            return documentedClasses;
        }

        var files = Directory.GetFiles(docsPath, "*.md", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var fileNameNoExt = Path.GetFileNameWithoutExtension(file);
            var content = File.ReadAllText(file);

            // Check filename (kebab-case match)
            foreach (var cls in classesToCheck)
            {
                bool found = false;
                if (ToKebabCase(cls) == fileNameNoExt)
                {
                    found = true;
                }
                // Check content
                else if (content.Contains(cls))
                {
                    found = true;
                }

                if (found)
                {
                    if (!documentedClasses.ContainsKey(cls))
                    {
                        documentedClasses[cls] = new List<string>();
                    }
                    documentedClasses[cls].Add(file);
                }
            }
        }

        return documentedClasses;
    }

    static string ToKebabCase(string name)
    {
        return Regex.Replace(name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1").ToLower();
    }

    static string? ResolveSolutionPath(string rootDir)
    {
        var slnPath = Path.Combine(rootDir, "AvaloniaBehaviors.sln");
        if (File.Exists(slnPath))
        {
            return slnPath;
        }

        var slnxPath = Path.Combine(rootDir, "AvaloniaBehaviors.slnx");
        if (File.Exists(slnxPath))
        {
            return slnxPath;
        }

        return null;
    }
}
