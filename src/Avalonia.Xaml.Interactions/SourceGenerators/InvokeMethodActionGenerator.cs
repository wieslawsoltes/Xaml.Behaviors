using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Avalonia.Xaml.Interactions.SourceGenerators;

[Generator]
public sealed class InvokeMethodActionGenerator : ISourceGenerator
{
    private const string AttributeName = "Avalonia.Xaml.Interactions.Core.GenerateInvokeAttribute";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
        {
            return;
        }

        var attributeSymbol = context.Compilation.GetTypeByMetadataName(AttributeName);
        if (attributeSymbol is null)
        {
            return;
        }

        var methods = new List<(IMethodSymbol Method, ClassDeclarationSyntax ClassSyntax)>();
        foreach (var method in receiver.CandidateMethods)
        {
            var model = context.Compilation.GetSemanticModel(method.SyntaxTree);
            if (model.GetDeclaredSymbol(method) is IMethodSymbol symbol &&
                symbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
            {
                if (method.Parent is ClassDeclarationSyntax cds)
                {
                    methods.Add((symbol, cds));
                }
            }
        }

        foreach (var group in methods.GroupBy(m => m.Method.ContainingType, SymbolEqualityComparer.Default))
        {
            var classSymbol = group.Key;
            var classSyntax = group.First().ClassSyntax;
            if (!classSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
            {
                continue;
            }

            var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : $"namespace {classSymbol.ContainingNamespace.ToDisplayString()};";

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine(ns);
            }

            sb.AppendLine($"partial class {classSymbol.Name} : Avalonia.Xaml.Interactions.Core.IInvokeMethodGenerated");
            sb.AppendLine("{");
            sb.AppendLine("    public bool InvokeGenerated(string methodName, object? sender, object? parameter)");
            sb.AppendLine("    {");
            sb.AppendLine("        switch (methodName)");
            sb.AppendLine("        {");

            foreach (var (methodSymbol, _) in group)
            {
                var name = methodSymbol.Name;
                var parameters = methodSymbol.Parameters;
                if (parameters.Length == 0)
                {
                    sb.AppendLine($"            case \"{name}\":");
                    sb.AppendLine($"                {name}();");
                    sb.AppendLine("                return true;");
                }
                else if (parameters.Length == 2 && parameters[0].Type.SpecialType == SpecialType.System_Object)
                {
                    var secondType = parameters[1].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    sb.AppendLine($"            case \"{name}\":");
                    sb.AppendLine($"                {name}(sender, ({secondType})parameter!);");
                    sb.AppendLine("                return true;");
                }
            }

            sb.AppendLine("            default:");
            sb.AppendLine("                return false;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            context.AddSource($"{classSymbol.Name}.InvokeMethod.g.cs", sb.ToString());
        }
    }

    private sealed class SyntaxReceiver : ISyntaxReceiver
    {
        public List<MethodDeclarationSyntax> CandidateMethods { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is MethodDeclarationSyntax method && method.AttributeLists.Count > 0)
            {
                CandidateMethods.Add(method);
            }
        }
    }
}
