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
        private record MultiDataTriggerInfo(string? Namespace, string ClassName, string Accessibility, ImmutableArray<TriggerPropertyInfo> Properties, Diagnostic? Diagnostic = null);

        private void RegisterMultiDataTriggerGeneration(IncrementalGeneratorInitializationContext context)
        {
            var multiDataTriggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedMultiDataTriggerAttributeName,
                    predicate: (node, _) => node is ClassDeclarationSyntax,
                    transform: (ctx, _) => GetMultiDataTriggerToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(multiDataTriggers, ExecuteGenerateMultiDataTrigger);
        }

        private ImmutableArray<MultiDataTriggerInfo> GetMultiDataTriggerToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<MultiDataTriggerInfo>();
            var symbol = context.TargetSymbol as INamedTypeSymbol;
            if (symbol == null) return results.ToImmutable();

            var partialDiagnostic = EnsurePartial(symbol, context.TargetNode?.GetLocation() ?? Location.None, "GenerateTypedMultiDataTrigger");
            if (partialDiagnostic != null)
            {
                results.Add(new MultiDataTriggerInfo(null, symbol.Name, "public", ImmutableArray<TriggerPropertyInfo>.Empty, partialDiagnostic));
                return results.ToImmutable();
            }

            var nestedDiagnostic = EnsureNotNested(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (nestedDiagnostic != null)
            {
                results.Add(new MultiDataTriggerInfo(null, symbol.Name, "public", ImmutableArray<TriggerPropertyInfo>.Empty, nestedDiagnostic));
                return results.ToImmutable();
            }

            var accessibilityDiagnostic = ValidateTypeAccessibility(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (accessibilityDiagnostic != null)
            {
                results.Add(new MultiDataTriggerInfo(null, symbol.Name, "public", ImmutableArray<TriggerPropertyInfo>.Empty, accessibilityDiagnostic));
                return results.ToImmutable();
            }

            var validationDiagnostic = ValidateStyledElementTriggerType(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (validationDiagnostic != null)
            {
                results.Add(new MultiDataTriggerInfo(null, symbol.Name, "public", ImmutableArray<TriggerPropertyInfo>.Empty, validationDiagnostic));
                return results.ToImmutable();
            }

            var evaluateDiagnostic = ValidateEvaluateMethod(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (evaluateDiagnostic != null)
            {
                results.Add(new MultiDataTriggerInfo(null, symbol.Name, "public", ImmutableArray<TriggerPropertyInfo>.Empty, evaluateDiagnostic));
                return results.ToImmutable();
            }

            var properties = ImmutableArray.CreateBuilder<TriggerPropertyInfo>();

            foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
            {
                if (member.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == TriggerPropertyAttributeName))
                {
                    var fieldName = member.Name;
                    var propertyName = fieldName.TrimStart('_');
                    if (propertyName.Length > 0)
                    {
                        propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                    }
                    var typeName = ToDisplayStringWithNullable(member.Type);
                    properties.Add(new TriggerPropertyInfo(propertyName, typeName, fieldName));
                }
            }

            if (properties.Count > 0)
            {
                var ns = symbol.ContainingNamespace.ToDisplayString();
                var namespaceName = (symbol.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var className = symbol.Name;
                var accessibility = GetAccessibilityKeyword(symbol);
                results.Add(new MultiDataTriggerInfo(namespaceName, className, accessibility, properties.ToImmutable()));
            }

            return results.ToImmutable();
        }

        private void ExecuteGenerateMultiDataTrigger(SourceProductionContext spc, MultiDataTriggerInfo info)
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
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    {info.Accessibility} partial class {info.ClassName}");
            sb.AppendLine("    {");

            foreach (var prop in info.Properties)
            {
                sb.AppendLine($"        public static readonly StyledProperty<{prop.Type}> {prop.Name}Property =");
                sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {prop.Type}>(nameof({prop.Name}));");
                sb.AppendLine();
                sb.AppendLine($"        public {prop.Type} {prop.Name}");
                sb.AppendLine("        {");
                sb.AppendLine($"            get => GetValue({prop.Name}Property);");
                sb.AppendLine($"            set => SetValue({prop.Name}Property, value);");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnPropertyChanged(change);");

            var conditions = string.Join(" || ", info.Properties.Select(p => $"change.Property == {p.Name}Property"));
            if (!string.IsNullOrEmpty(conditions))
            {
                sb.AppendLine($"            if ({conditions})");
                sb.AppendLine("            {");
                foreach (var prop in info.Properties)
                {
                    sb.AppendLine($"                if (change.Property == {prop.Name}Property) this.{prop.FieldName} = change.GetNewValue<{prop.Type}>();");
                }

                sb.AppendLine("                if (Evaluate())");
                sb.AppendLine("                {");
                sb.AppendLine("                    Interaction.ExecuteActions(AssociatedObject, Actions, null);");
                sb.AppendLine("                }");
                sb.AppendLine("            }");
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
