// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
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
        private record EventArgsActionInfo(
            string? Namespace,
            string ClassName,
            string Accessibility,
            string TargetTypeName,
            string MethodName,
            string EventArgsTypeName,
            ImmutableArray<ProjectionInfo> Projections,
            bool UseDispatcher,
            Diagnostic? Diagnostic = null);

        private record ProjectionInfo(string Name, string TypeName, string SourceMemberName);

        private void RegisterEventArgsActionGeneration(IncrementalGeneratorInitializationContext context)
        {
            var actions = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateEventArgsActionAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetEventArgsActions(ctx))
                .SelectMany((x, _) => x);

            var assemblyActions = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node, "GenerateEventArgsAction"),
                    transform: (ctx, _) => GetAssemblyEventArgsActions(ctx))
                .SelectMany((x, _) => x);

            var unique = actions
                .Collect()
                .Combine(assemblyActions.Collect())
                .SelectMany((data, _) => EnsureUniqueEventArgsActions(data.Left.Concat(data.Right)));

            context.RegisterSourceOutput(unique, ExecuteGenerateEventArgsAction);
        }

        private ImmutableArray<EventArgsActionInfo> GetEventArgsActions(GeneratorAttributeSyntaxContext context)
        {
            var builder = ImmutableArray.CreateBuilder<EventArgsActionInfo>();
            if (context.TargetSymbol is IAssemblySymbol)
            {
                return builder.ToImmutable();
            }

            if (context.TargetSymbol is IMethodSymbol methodSymbol)
            {
                var useDispatcher = GetUseDispatcherFlag(context.Attributes.First(), context.SemanticModel);
                var nameOverride = GetNameOverride(context.Attributes.First(), context.SemanticModel);
                var project = GetProjectList(context.Attributes.First(), context.SemanticModel);
                var info = CreateEventArgsActionInfo(methodSymbol, context.TargetNode?.GetLocation() ?? Location.None, includeTypeNamePrefix: false, context.SemanticModel.Compilation, useDispatcher, nameOverride, project);
                builder.Add(info);
            }

            return builder.ToImmutable();
        }

        private ImmutableArray<EventArgsActionInfo> GetAssemblyEventArgsActions(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return ImmutableArray<EventArgsActionInfo>.Empty;

            if (attributeSyntax.ArgumentList?.Arguments == null)
                return ImmutableArray<EventArgsActionInfo>.Empty;

            var positionalArguments = attributeSyntax.ArgumentList.Arguments
                .Where(a => a.NameEquals is null && a.NameColon is null)
                .ToList();

            if (positionalArguments.Count < 2)
                return ImmutableArray<EventArgsActionInfo>.Empty;

            if (positionalArguments[0].Expression is not TypeOfExpressionSyntax typeOfExpression)
                return ImmutableArray<EventArgsActionInfo>.Empty;

            if (positionalArguments[1].Expression is not LiteralExpressionSyntax methodLiteral)
                return ImmutableArray<EventArgsActionInfo>.Empty;

            var methodName = methodLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(methodName))
                return ImmutableArray<EventArgsActionInfo>.Empty;

            var useDispatcher = GetBoolNamedArgument(attributeSyntax, context.SemanticModel, "UseDispatcher");
            var nameOverride = GetNameOverride(attributeSyntax, context.SemanticModel);
            var project = GetProjectList(attributeSyntax, context.SemanticModel);

            return CreateEventArgsActionInfos(targetType, methodName, context.Node.GetLocation(), includeTypeNamePrefix: true, context.SemanticModel.Compilation, useDispatcher, nameOverride, project);
        }

        private void ExecuteGenerateEventArgsAction(SourceProductionContext spc, EventArgsActionInfo info)
        {
            if (info.Diagnostic != null)
            {
                spc.ReportDiagnostic(info.Diagnostic);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("#nullable enable");
            sb.AppendLine("using System;");
            sb.AppendLine("using Avalonia;");
            sb.AppendLine("using Avalonia.Controls;");
            sb.AppendLine("using Avalonia.Xaml.Interactivity;");
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    {info.Accessibility} partial class {info.ClassName} : Avalonia.Xaml.Interactivity.StyledElementAction");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static readonly StyledProperty<object?> TargetObjectProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, object?>(nameof(TargetObject));");
            sb.AppendLine();
            sb.AppendLine("        public object? TargetObject");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(TargetObjectProperty);");
            sb.AppendLine("            set => SetValue(TargetObjectProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();

            foreach (var proj in info.Projections)
            {
                sb.AppendLine($"        public static readonly StyledProperty<{proj.TypeName}> {proj.Name}Property =");
                sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {proj.TypeName}>(nameof({proj.Name}));");
                sb.AppendLine();
                sb.AppendLine($"        public {proj.TypeName} {proj.Name}");
                sb.AppendLine("        {");
                sb.AppendLine($"            get => GetValue({proj.Name}Property);");
                sb.AppendLine($"            set => SetValue({proj.Name}Property, value);");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("        public override object Execute(object? sender, object? parameter)");
            sb.AppendLine("        {");
            sb.AppendLine("            var target = TargetObject ?? sender;");
            sb.AppendLine($"            if (target is {info.TargetTypeName} typedTarget)");
            sb.AppendLine("            {");
            sb.AppendLine($"                if (parameter is not {info.EventArgsTypeName} args)");
            sb.AppendLine("                {");
            sb.AppendLine("                    return false;");
            sb.AppendLine("                }");
            if (info.Projections.Length > 0)
            {
                foreach (var proj in info.Projections)
                {
                    sb.AppendLine($"                {proj.Name} = args.{proj.SourceMemberName};");
                }
                sb.AppendLine();
            }
            var invocation = $"typedTarget.{info.MethodName}(args)";
            if (info.UseDispatcher)
            {
                sb.AppendLine("                Avalonia.Threading.Dispatcher.UIThread.Post(() =>");
                sb.AppendLine("                {");
                sb.AppendLine($"                    {invocation};");
                sb.AppendLine("                });");
                sb.AppendLine("                return true;");
            }
            else
            {
                sb.AppendLine($"                {invocation};");
                sb.AppendLine("                return true;");
            }
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine("}");
            }

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private ImmutableArray<EventArgsActionInfo> CreateEventArgsActionInfos(INamedTypeSymbol targetType, string methodPattern, Location? diagnosticLocation, bool includeTypeNamePrefix, Compilation? compilation, bool useDispatcher, string? nameOverride, ImmutableArray<string> project)
        {
            var methods = FindMatchingMethods(targetType, methodPattern, compilation: compilation);
            if (methods.Length == 0)
            {
                var diagnostic = Diagnostic.Create(ActionMethodNotFoundDiagnostic, diagnosticLocation ?? Location.None, methodPattern, targetType.Name);
                var ns = targetType.ContainingNamespace.ToDisplayString();
                var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType) : string.Empty;
                var baseName = nameOverride ?? $"{CreateSafeIdentifier(methodPattern)}EventArgsAction";
                var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
                var accessibility = GetAccessibilityKeyword(targetType);
                var targetTypeName = ToDisplayStringWithNullable(targetType);
                return ImmutableArray.Create(new EventArgsActionInfo(namespaceName, className, accessibility, targetTypeName, methodPattern, "", ImmutableArray<ProjectionInfo>.Empty, useDispatcher, diagnostic));
            }

            var builder = ImmutableArray.CreateBuilder<EventArgsActionInfo>();
            foreach (var method in methods)
            {
                var info = CreateEventArgsActionInfo(method, diagnosticLocation, includeTypeNamePrefix, compilation, useDispatcher, nameOverride, project);
                builder.Add(info);
            }
            return builder.ToImmutable();
        }

        private EventArgsActionInfo CreateEventArgsActionInfo(IMethodSymbol methodSymbol, Location? diagnosticLocation, bool includeTypeNamePrefix, Compilation? compilation, bool useDispatcher, string? nameOverride, ImmutableArray<string> project)
        {
            var targetType = methodSymbol.ContainingType;
            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType) : string.Empty;
            var baseName = nameOverride ?? $"{methodSymbol.Name}EventArgsAction";
            var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
            var targetTypeName = ToDisplayStringWithNullable(targetType);
            var accessibility = GetAccessibilityKeyword(targetType);

            var validation = ValidateEventArgsMethod(methodSymbol, diagnosticLocation, compilation);
            if (validation != null)
            {
                return new EventArgsActionInfo(namespaceName, className, accessibility, targetTypeName, methodSymbol.Name, "", ImmutableArray<ProjectionInfo>.Empty, useDispatcher, validation);
            }

            var eventArgsType = methodSymbol.Parameters.First().Type;
            var eventArgsTypeName = ToDisplayStringWithNullable(eventArgsType);

            var projections = ImmutableArray<ProjectionInfo>.Empty;
            if (!project.IsDefaultOrEmpty)
            {
                var projBuilder = ImmutableArray.CreateBuilder<ProjectionInfo>();
                foreach (var projName in project)
                {
                    var member = FindProperty(eventArgsType, projName);
                    if (member == null)
                    {
                        var diag = Diagnostic.Create(EventArgsProjectionNotFoundDiagnostic, diagnosticLocation ?? Location.None, projName, eventArgsTypeName);
                        return new EventArgsActionInfo(namespaceName, className, accessibility, targetTypeName, methodSymbol.Name, eventArgsTypeName, ImmutableArray<ProjectionInfo>.Empty, useDispatcher, diag);
                    }
                    var memberAccessible =
                        member.DeclaredAccessibility == Accessibility.Public &&
                        (member.GetMethod is null || member.GetMethod.DeclaredAccessibility == Accessibility.Public) &&
                        (member.SetMethod is null || member.SetMethod.DeclaredAccessibility == Accessibility.Public);

                    if (!memberAccessible || !AccessibilityHelper.IsPubliclyAccessibleType(member.Type))
                    {
                        var diag = Diagnostic.Create(EventArgsProjectionNotAccessibleDiagnostic, diagnosticLocation ?? Location.None, projName, eventArgsTypeName);
                        return new EventArgsActionInfo(namespaceName, className, accessibility, targetTypeName, methodSymbol.Name, eventArgsTypeName, ImmutableArray<ProjectionInfo>.Empty, useDispatcher, diag);
                    }
                    var typeName = ToDisplayStringWithNullable(member.Type);
                    projBuilder.Add(new ProjectionInfo(projName, typeName, member.Name));
                }
                projections = projBuilder.ToImmutable();
            }

            return new EventArgsActionInfo(namespaceName, className, accessibility, targetTypeName, methodSymbol.Name, eventArgsTypeName, projections, useDispatcher);
        }

        private Diagnostic? ValidateEventArgsMethod(IMethodSymbol methodSymbol, Location? location, Compilation? compilation)
        {
            var loc = location ?? Location.None;

            if (methodSymbol.IsStatic)
            {
                return Diagnostic.Create(StaticMemberNotSupportedDiagnostic, loc, methodSymbol.Name);
            }

            if (ContainsTypeParameter(methodSymbol.ContainingType))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, loc, methodSymbol.Name);
            }

            if (!IsAccessibleToGenerator(methodSymbol, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, loc, methodSymbol.Name, methodSymbol.ContainingType.ToDisplayString());
            }

            if (methodSymbol.IsGenericMethod || methodSymbol.TypeParameters.Length > 0)
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, loc, methodSymbol.Name);
            }

            if (methodSymbol.Parameters.Length != 1)
            {
                return Diagnostic.Create(ActionMethodAmbiguousDiagnostic, loc, methodSymbol.Name, methodSymbol.ContainingType.Name);
            }

            var argsType = methodSymbol.Parameters[0].Type;
            if (!IsAccessibleType(argsType, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, loc, methodSymbol.Name, methodSymbol.ContainingType.ToDisplayString());
            }

            if (methodSymbol.ContainingType.DeclaredAccessibility == Accessibility.Public && !AccessibilityHelper.IsPubliclyAccessibleType(argsType))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, loc, methodSymbol.Name, methodSymbol.ContainingType.ToDisplayString());
            }

            return ValidateTypeAccessibility(methodSymbol.ContainingType, loc, compilation);
        }

        private static ImmutableArray<string> GetProjectList(AttributeData attributeData, SemanticModel semanticModel)
        {
            foreach (var namedArgument in attributeData.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, "Project", StringComparison.Ordinal) && namedArgument.Value.Value is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return SplitProjectList(s);
                }
            }

            if (attributeData.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax syntax)
            {
                return GetProjectList(syntax, semanticModel);
            }

            return ImmutableArray<string>.Empty;
        }

        private static ImmutableArray<string> GetProjectList(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            if (attributeSyntax.ArgumentList == null)
            {
                return ImmutableArray<string>.Empty;
            }

            foreach (var argument in attributeSyntax.ArgumentList.Arguments)
            {
                if (argument.NameEquals?.Name.Identifier.Text == "Project")
                {
                    var constant = semanticModel.GetConstantValue(argument.Expression);
                    if (constant.HasValue && constant.Value is string s && !string.IsNullOrWhiteSpace(s))
                    {
                        return SplitProjectList(s);
                    }
                }
            }

            return ImmutableArray<string>.Empty;
        }

        private static ImmutableArray<string> SplitProjectList(string value)
        {
            var parts = value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.Ordinal)
                .ToImmutableArray();
            return parts;
        }

        private static IPropertySymbol? FindProperty(ITypeSymbol typeSymbol, string name)
        {
            var current = typeSymbol;
            while (current != null)
            {
                var match = current.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.Ordinal));
                if (match != null)
                {
                    return match;
                }
                current = current.BaseType;
            }

            return null;
        }

        private static IEnumerable<EventArgsActionInfo> EnsureUniqueEventArgsActions(IEnumerable<EventArgsActionInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => (info.Namespace, info.ClassName)))
            {
                var distinct = group
                    .GroupBy(info => (info.TargetTypeName, info.MethodName))
                    .Select(g => g.FirstOrDefault(info => info.Diagnostic is null) ?? g.First())
                    .ToList();

                if (distinct.Count == 1)
                {
                    yield return distinct[0];
                    continue;
                }

                foreach (var info in distinct)
                {
                    yield return info with { ClassName = MakeUniqueName(info.ClassName, info.TargetTypeName) };
                }
            }
        }
    }
}
