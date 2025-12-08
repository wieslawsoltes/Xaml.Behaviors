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
        private record InvokeCommandActionInfo(string? Namespace, string ClassName, string Accessibility, TriggerPropertyInfo? Command, TriggerPropertyInfo? Parameter, bool UseDispatcher, Diagnostic? Diagnostic = null);

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

            var useDispatcher = false;
            if (context.Attributes.FirstOrDefault() is { } attribute)
            {
                useDispatcher = GetUseDispatcherFlag(attribute, context.SemanticModel);
            }

            var partialDiagnostic = EnsurePartial(symbol, context.TargetNode?.GetLocation() ?? Location.None, "GenerateTypedInvokeCommandAction");
            if (partialDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, useDispatcher, partialDiagnostic));
                return results.ToImmutable();
            }

            var nestedDiagnostic = EnsureNotNested(symbol, context.TargetNode?.GetLocation() ?? Location.None);
            if (nestedDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, useDispatcher, nestedDiagnostic));
                return results.ToImmutable();
            }

            var accessibilityDiagnostic = ValidateTypeAccessibility(symbol, context.TargetNode?.GetLocation() ?? Location.None, context.SemanticModel.Compilation);
            if (accessibilityDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, useDispatcher, accessibilityDiagnostic));
                return results.ToImmutable();
            }

            if (ContainsTypeParameter(symbol))
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, useDispatcher, Diagnostic.Create(GenericMemberNotSupportedDiagnostic, context.TargetNode?.GetLocation() ?? Location.None, symbol.Name)));
                return results.ToImmutable();
            }

            var validationDiagnostic = ValidateStyledElementActionType(symbol, context.TargetNode?.GetLocation() ?? Location.None, context.SemanticModel.Compilation);
            if (validationDiagnostic != null)
            {
                results.Add(new InvokeCommandActionInfo(null, symbol.Name, "public", null, null, useDispatcher, validationDiagnostic));
                return results.ToImmutable();
            }

            TriggerPropertyInfo? commandProp = null;
            TriggerPropertyInfo? parameterProp = null;
            IFieldSymbol? internalField = null;

            foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
            {
                var hasCommandAttribute = member.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ActionCommandAttributeName);
                var hasParameterAttribute = member.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ActionParameterAttributeName);
                if (!hasCommandAttribute && !hasParameterAttribute)
                {
                    continue;
                }

                if (member.IsStatic)
                {
                    var nsStatic = symbol.ContainingNamespace.ToDisplayString();
                    var namespaceStatic = (symbol.ContainingNamespace.IsGlobalNamespace || nsStatic == "<global namespace>") ? null : nsStatic;
                    var classStatic = symbol.Name;
                    var accessibilityStatic = GetAccessibilityKeyword(symbol);
                    var diag = Diagnostic.Create(StaticMemberNotSupportedDiagnostic, member.Locations.FirstOrDefault() ?? context.TargetNode?.GetLocation() ?? Location.None, member.Name);
                    results.Add(new InvokeCommandActionInfo(namespaceStatic, classStatic, accessibilityStatic, null, null, useDispatcher, diag));
                    return results.ToImmutable();
                }

                if (member.IsReadOnly)
                {
                    var nsReadonly = symbol.ContainingNamespace.ToDisplayString();
                    var namespaceReadonly = (symbol.ContainingNamespace.IsGlobalNamespace || nsReadonly == "<global namespace>") ? null : nsReadonly;
                    var classReadonly = symbol.Name;
                    var accessibilityReadonly = GetAccessibilityKeyword(symbol);
                    var diag = Diagnostic.Create(ReadOnlyMemberNotSupportedDiagnostic, member.Locations.FirstOrDefault() ?? context.TargetNode?.GetLocation() ?? Location.None, member.Name);
                    results.Add(new InvokeCommandActionInfo(namespaceReadonly, classReadonly, accessibilityReadonly, null, null, useDispatcher, diag));
                    return results.ToImmutable();
                }

                if (ContainsTypeParameter(member.Type))
                {
                    var nsGeneric = symbol.ContainingNamespace.ToDisplayString();
                    var namespaceGeneric = (symbol.ContainingNamespace.IsGlobalNamespace || nsGeneric == "<global namespace>") ? null : nsGeneric;
                    var classGeneric = symbol.Name;
                    var accessibilityGeneric = GetAccessibilityKeyword(symbol);
                    var diag = Diagnostic.Create(GenericMemberNotSupportedDiagnostic, member.Locations.FirstOrDefault() ?? context.TargetNode?.GetLocation() ?? Location.None, member.Name);
                    results.Add(new InvokeCommandActionInfo(namespaceGeneric, classGeneric, accessibilityGeneric, null, null, useDispatcher, diag));
                    return results.ToImmutable();
                }

                if (!IsAccessibleType(member.Type, context.SemanticModel.Compilation))
                {
                    var nsInaccessible = symbol.ContainingNamespace.ToDisplayString();
                    var namespaceInaccessible = (symbol.ContainingNamespace.IsGlobalNamespace || nsInaccessible == "<global namespace>") ? null : nsInaccessible;
                    var classInaccessible = symbol.Name;
                    var accessibilityInaccessible = GetAccessibilityKeyword(symbol);
                    var diag = Diagnostic.Create(MemberNotAccessibleDiagnostic, member.Locations.FirstOrDefault() ?? context.TargetNode?.GetLocation() ?? Location.None, member.Name, symbol.ToDisplayString());
                    results.Add(new InvokeCommandActionInfo(namespaceInaccessible, classInaccessible, accessibilityInaccessible, null, null, useDispatcher, diag));
                    return results.ToImmutable();
                }

                var requiresInternal = ContainsInternalType(member.Type);
                if (requiresInternal && internalField is null)
                {
                    internalField = member;
                }

                if (hasCommandAttribute)
                {
                    var fieldName = member.Name;
                    var propertyName = fieldName.TrimStart('_');
                    if (propertyName.Length > 0) propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                    var typeName = ToDisplayStringWithNullable(member.Type);
                    var propRequiresInternal = ContainsInternalType(member.Type);
                    commandProp = new TriggerPropertyInfo(propertyName, typeName, fieldName, propRequiresInternal);
                }
                if (hasParameterAttribute)
                {
                    var fieldName = member.Name;
                    var propertyName = fieldName.TrimStart('_');
                    if (propertyName.Length > 0) propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                    var typeName = ToDisplayStringWithNullable(member.Type);
                    var propRequiresInternal = ContainsInternalType(member.Type);
                    parameterProp = new TriggerPropertyInfo(propertyName, typeName, fieldName, propRequiresInternal);
                }
            }

            if (commandProp != null)
            {
                var ns = symbol.ContainingNamespace.ToDisplayString();
                var namespaceName = (symbol.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var className = symbol.Name;
                var accessibility = GetAccessibilityKeyword(symbol);

                if (internalField != null &&
                    symbol.DeclaredAccessibility is not Accessibility.Internal and not Accessibility.ProtectedOrInternal)
                {
                    var diagLocation = internalField.Locations.FirstOrDefault() ?? context.TargetNode?.GetLocation() ?? Location.None;
                    var diag = Diagnostic.Create(MemberNotAccessibleDiagnostic, diagLocation, internalField.Name, symbol.ToDisplayString());
                    results.Add(new InvokeCommandActionInfo(namespaceName, className, accessibility, null, null, useDispatcher, diag));
                    return results.ToImmutable();
                }

                results.Add(new InvokeCommandActionInfo(namespaceName, className, accessibility, commandProp, parameterProp, useDispatcher));
            }
            else
            {
                var ns = symbol.ContainingNamespace.ToDisplayString();
                var namespaceName = (symbol.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var className = symbol.Name;
                var accessibility = GetAccessibilityKeyword(symbol);
                results.Add(new InvokeCommandActionInfo(namespaceName, className, accessibility, null, null, useDispatcher, Diagnostic.Create(InvokeCommandMissingCommandFieldDiagnostic, context.TargetNode?.GetLocation() ?? Location.None, symbol.ToDisplayString())));
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
            sb.AppendLine("using Avalonia.Threading;");
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
            if (info.UseDispatcher)
            {
                sb.AppendLine($"                if (!command.CanExecute({param}))");
                sb.AppendLine("                {");
                sb.AppendLine("                    return false;");
                sb.AppendLine("                }");
                sb.AppendLine("                Avalonia.Threading.Dispatcher.UIThread.Post(() =>");
                sb.AppendLine("                {");
                sb.AppendLine($"                    if (command.CanExecute({param}))");
                sb.AppendLine("                    {");
                sb.AppendLine($"                        command.Execute({param});");
                sb.AppendLine("                    }");
                sb.AppendLine("                });");
                sb.AppendLine("                return true;");
            }
            else
            {
                sb.AppendLine($"                if (command.CanExecute({param}))");
                sb.AppendLine("                {");
                sb.AppendLine($"                    command.Execute({param});");
                sb.AppendLine("                    return true;");
                sb.AppendLine("                }");
            }
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }

    }
}
