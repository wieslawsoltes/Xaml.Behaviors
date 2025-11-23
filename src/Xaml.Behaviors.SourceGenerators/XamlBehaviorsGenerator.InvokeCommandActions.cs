// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        private record InvokeCommandActionInfo(string? Namespace, string ClassName, string Accessibility, TriggerPropertyInfo? Command, TriggerPropertyInfo? Parameter, Diagnostic? Diagnostic = null);

        private void RegisterInvokeCommandActionGeneration(IncrementalGeneratorInitializationContext context)
        {
            var invokeCommandActions = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedInvokeCommandActionAttributeName,
                    predicate: (node, _) => node is ClassDeclarationSyntax,
                    transform: (ctx, _) => GetInvokeCommandActionToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(invokeCommandActions, ExecuteGenerateInvokeCommandAction);
        }

        private ImmutableArray<InvokeCommandActionInfo> GetInvokeCommandActionToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<InvokeCommandActionInfo>();
            var symbol = context.TargetSymbol as INamedTypeSymbol;
            if (symbol == null) return results.ToImmutable();

            var partialDiagnostic = EnsurePartial(symbol, context.TargetNode?.GetLocation() ?? Location.None, "GenerateTypedInvokeCommandAction");
            if (partialDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, partialDiagnostic));
                return results.ToImmutable();
            }

            var nestedDiagnostic = EnsureNotNested(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (nestedDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, nestedDiagnostic));
                return results.ToImmutable();
            }

            var accessibilityDiagnostic = ValidateTypeAccessibility(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (accessibilityDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, accessibilityDiagnostic));
                return results.ToImmutable();
            }

            var validationDiagnostic = ValidateStyledElementActionType(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (validationDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, validationDiagnostic));
                return results.ToImmutable();
            }

            TriggerPropertyInfo? commandProp = null;
            TriggerPropertyInfo? parameterProp = null;

            foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
            {
                if (member.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ActionCommandAttributeName))
                {
                    var fieldName = member.Name;
                    var propertyName = fieldName.TrimStart('_');
                    if (propertyName.Length > 0) propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                    var typeName = ToDisplayStringWithNullable(member.Type);
                    commandProp = new TriggerPropertyInfo(propertyName, typeName, fieldName);
                }
                if (member.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ActionParameterAttributeName))
                {
                    var fieldName = member.Name;
                    var propertyName = fieldName.TrimStart('_');
                    if (propertyName.Length > 0) propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                    var typeName = ToDisplayStringWithNullable(member.Type);
                    parameterProp = new TriggerPropertyInfo(propertyName, typeName, fieldName);
                }
            }

            if (commandProp != null)
            {
                var ns = symbol.ContainingNamespace.ToDisplayString();
                var namespaceName = (symbol.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var className = symbol.Name;
                var accessibility = GetAccessibilityKeyword(symbol);
                results.Add(new InvokeCommandActionInfo(namespaceName, className, accessibility, commandProp, parameterProp));
            }

            return results.ToImmutable();
        }

        private void ExecuteGenerateInvokeCommandAction(SourceProductionContext spc, InvokeCommandActionInfo info)
        {
            if (info.Diagnostic != null)
            {
                spc.ReportDiagnostic(info.Diagnostic);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("#nullable enable");
            sb.AppendLine("using Avalonia;");
            sb.AppendLine("using Avalonia.Xaml.Interactivity;");
            sb.AppendLine("using Avalonia.Controls;");
            sb.AppendLine("using System.Windows.Input;");
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    {info.Accessibility} partial class {info.ClassName}");
            sb.AppendLine("    {");

            if (info.Command != null)
            {
                sb.AppendLine($"        public static readonly StyledProperty<{info.Command.Type}> {info.Command.Name}Property =");
                sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {info.Command.Type}>(nameof({info.Command.Name}));");
                sb.AppendLine();
                sb.AppendLine($"        public {info.Command.Type} {info.Command.Name}");
                sb.AppendLine("        {");
                sb.AppendLine($"            get => GetValue({info.Command.Name}Property);");
                sb.AppendLine($"            set => SetValue({info.Command.Name}Property, value);");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            if (info.Parameter != null)
            {
                sb.AppendLine($"        public static readonly StyledProperty<{info.Parameter.Type}> {info.Parameter.Name}Property =");
                sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {info.Parameter.Type}>(nameof({info.Parameter.Name}));");
                sb.AppendLine();
                sb.AppendLine($"        public {info.Parameter.Type} {info.Parameter.Name}");
                sb.AppendLine("        {");
                sb.AppendLine($"            get => GetValue({info.Parameter.Name}Property);");
                sb.AppendLine($"            set => SetValue({info.Parameter.Name}Property, value);");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("        public override object? Execute(object? sender, object? parameter)");
            sb.AppendLine("        {");
            if (info.Command != null) sb.AppendLine($"            this.{info.Command.FieldName} = this.{info.Command.Name};");
            if (info.Parameter != null) sb.AppendLine($"            this.{info.Parameter.FieldName} = this.{info.Parameter.Name};");

            sb.AppendLine($"            if (this.{info.Command!.FieldName} is ICommand command)");
            sb.AppendLine("            {");
            var param = info.Parameter != null ? $"this.{info.Parameter.FieldName}" : "parameter";
            sb.AppendLine($"                if (command.CanExecute({param}))");
            sb.AppendLine("                {");
            sb.AppendLine($"                    command.Execute({param});");
            sb.AppendLine("                    return true;");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
