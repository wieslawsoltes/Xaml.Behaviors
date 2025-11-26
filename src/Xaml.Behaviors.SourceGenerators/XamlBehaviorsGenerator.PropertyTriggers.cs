// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

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
        private record PropertyTriggerInfo(
            string? Namespace,
            string ClassName,
            string Accessibility,
            string TargetTypeName,
            string PropertyOwnerTypeName,
            string PropertyFieldName,
            string ValueTypeName,
            bool UseDispatcher,
            string? DefaultSourceName,
            ImmutableArray<Diagnostic> Warnings,
            Diagnostic? Diagnostic = null);

        private void RegisterPropertyTriggerGeneration(IncrementalGeneratorInitializationContext context)
        {
            var memberTriggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GeneratePropertyTriggerAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetPropertyTriggerFromSymbol(ctx))
                .SelectMany((x, _) => x);

            var assemblyTriggers = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node),
                    transform: (ctx, _) => GetAssemblyPropertyTriggerFromAttributeSyntax(ctx))
                .SelectMany((x, _) => x);

            var uniqueTriggers = memberTriggers
                .Collect()
                .Combine(assemblyTriggers.Collect())
                .SelectMany((data, _) => EnsureUniquePropertyTriggers(data.Left.Concat(data.Right)));

            context.RegisterSourceOutput(uniqueTriggers, ExecuteGeneratePropertyTrigger);
        }

        private ImmutableArray<PropertyTriggerInfo> GetPropertyTriggerFromSymbol(GeneratorAttributeSyntaxContext context)
        {
            var builder = ImmutableArray.CreateBuilder<PropertyTriggerInfo>();
            var symbol = context.TargetSymbol;
            if (symbol is IAssemblySymbol)
            {
                return builder.ToImmutable();
            }

            foreach (var attribute in context.Attributes)
            {
                var useDispatcher = GetUseDispatcherFlag(attribute, context.SemanticModel);
                var nameOverride = GetNameOverride(attribute, context.SemanticModel);
                var sourceName = GetSourceName(attribute, context.SemanticModel);
                var location = attribute.ApplicationSyntaxReference?.GetSyntax()?.GetLocation() ?? context.TargetNode?.GetLocation();

                if (symbol is IFieldSymbol fieldSymbol)
                {
                    var info = CreatePropertyTriggerInfo(fieldSymbol, location, context.SemanticModel.Compilation, useDispatcher, nameOverride, sourceName);
                    builder.Add(info);
                }
                else if (symbol is IPropertySymbol propertySymbol)
                {
                    var info = CreatePropertyTriggerInfo(propertySymbol, location, context.SemanticModel.Compilation, useDispatcher, nameOverride, sourceName);
                    builder.Add(info);
                }
            }

            return builder.ToImmutable();
        }

        private ImmutableArray<PropertyTriggerInfo> GetAssemblyPropertyTriggerFromAttributeSyntax(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return ImmutableArray<PropertyTriggerInfo>.Empty;

            var attributeType = context.SemanticModel.GetTypeInfo(attributeSyntax).Type;
            if (!IsAttributeType(attributeType, GeneratePropertyTriggerAttributeName))
                return ImmutableArray<PropertyTriggerInfo>.Empty;

            if (attributeSyntax.ArgumentList?.Arguments == null)
                return ImmutableArray<PropertyTriggerInfo>.Empty;

            var positionalArguments = attributeSyntax.ArgumentList.Arguments
                .Where(a => a.NameEquals is null && a.NameColon is null)
                .ToList();

            if (positionalArguments.Count < 2)
                return ImmutableArray<PropertyTriggerInfo>.Empty;

            if (positionalArguments[0].Expression is not TypeOfExpressionSyntax typeOfExpression)
                return ImmutableArray<PropertyTriggerInfo>.Empty;

            if (positionalArguments[1].Expression is not LiteralExpressionSyntax propertyLiteral)
                return ImmutableArray<PropertyTriggerInfo>.Empty;

            var propertyName = propertyLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(propertyName))
                return ImmutableArray<PropertyTriggerInfo>.Empty;

            var useDispatcher = GetBoolNamedArgument(attributeSyntax, context.SemanticModel, "UseDispatcher");
            var nameOverride = GetNameOverride(attributeSyntax, context.SemanticModel);
            var sourceName = GetSourceName(attributeSyntax, context.SemanticModel);

            return CreatePropertyTriggerInfos(targetType, propertyName, context.Node.GetLocation(), includeTypeNamePrefix: true, context.SemanticModel.Compilation, useDispatcher, nameOverride, sourceName);
        }

        private void ExecuteGeneratePropertyTrigger(SourceProductionContext spc, PropertyTriggerInfo info)
        {
            foreach (var warning in info.Warnings)
            {
                spc.ReportDiagnostic(warning);
            }

            if (info.Diagnostic != null)
            {
                spc.ReportDiagnostic(info.Diagnostic);
                if (info.Diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    return;
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("#nullable enable");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Avalonia;");
            sb.AppendLine("using Avalonia.Controls;");
            sb.AppendLine("using Avalonia.LogicalTree;");
            sb.AppendLine("using Avalonia.Xaml.Interactivity;");
            sb.AppendLine("using Avalonia.Reactive;");
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    {info.Accessibility} partial class {info.ClassName} : Avalonia.Xaml.Interactivity.StyledElementTrigger");
            sb.AppendLine("    {");
            sb.AppendLine("        public static readonly StyledProperty<object?> SourceObjectProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, object?>(nameof(SourceObject));");
            sb.AppendLine();
            var sourceNameDefault = info.DefaultSourceName is null
                ? "default(string?)"
                : $"\"{info.DefaultSourceName.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
            sb.AppendLine("        public static readonly StyledProperty<string?> SourceNameProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, string?>(nameof(SourceName), {sourceNameDefault});");
            sb.AppendLine();
            sb.AppendLine($"        public static readonly StyledProperty<ComparisonConditionType> ComparisonConditionProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, ComparisonConditionType>(nameof(ComparisonCondition));");
            sb.AppendLine();
            sb.AppendLine($"        public static readonly StyledProperty<{info.ValueTypeName}> ValueProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {info.ValueTypeName}>(nameof(Value));");
            sb.AppendLine();
            sb.AppendLine("        private IDisposable? _subscription;");
            sb.AppendLine("        private object? _resolvedSource;");
            sb.AppendLine();
            sb.AppendLine("        public object? SourceObject");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(SourceObjectProperty);");
            sb.AppendLine("            set => SetValue(SourceObjectProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public string? SourceName");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(SourceNameProperty);");
            sb.AppendLine("            set => SetValue(SourceNameProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public ComparisonConditionType ComparisonCondition");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(ComparisonConditionProperty);");
            sb.AppendLine("            set => SetValue(ComparisonConditionProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {info.ValueTypeName} Value");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(ValueProperty);");
            sb.AppendLine("            set => SetValue(ValueProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnAttached()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnAttached();");
            sb.AppendLine("            UpdateSource();");
            sb.AppendLine("            EvaluateCurrentValue();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnDetaching()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnDetaching();");
            sb.AppendLine("            Unsubscribe();");
            sb.AppendLine("            _resolvedSource = null;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnPropertyChanged(change);");
            sb.AppendLine("            if (change.Property == SourceObjectProperty || change.Property == SourceNameProperty)");
            sb.AppendLine("            {");
            sb.AppendLine("                UpdateSource();");
            sb.AppendLine("                EvaluateCurrentValue();");
            sb.AppendLine("            }");
            sb.AppendLine("            else if (change.Property == ComparisonConditionProperty || change.Property == ValueProperty)");
            sb.AppendLine("            {");
            sb.AppendLine("                EvaluateCurrentValue();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void UpdateSource()");
            sb.AppendLine("        {");
            sb.AppendLine("            var newSource = ResolveSource();");
            sb.AppendLine("            if (!ReferenceEquals(newSource, _resolvedSource))");
            sb.AppendLine("            {");
            sb.AppendLine("                Unsubscribe();");
            sb.AppendLine("                _resolvedSource = newSource;");
            sb.AppendLine("                Subscribe();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private object? ResolveSource()");
            sb.AppendLine("        {");
            sb.AppendLine("            if (!string.IsNullOrEmpty(SourceName))");
            sb.AppendLine("            {");
            sb.AppendLine("                var named = FindInNameScope(AssociatedObject, SourceName!) ?? FindInNameScope(AssociatedStyledElement, SourceName!);");
            sb.AppendLine("                if (named is not null) return named;");
            sb.AppendLine("            }");
            sb.AppendLine("            return SourceObject ?? AssociatedObject;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Subscribe()");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (_resolvedSource is {info.TargetTypeName} typed)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _subscription = typed.GetObservable({info.PropertyOwnerTypeName}.{info.PropertyFieldName})");
            sb.AppendLine($"                    .Subscribe(new AnonymousObserver<{info.ValueTypeName}>(OnObserved));");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Unsubscribe()");
            sb.AppendLine("        {");
            sb.AppendLine("            _subscription?.Dispose();");
            sb.AppendLine("            _subscription = null;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private void OnObserved({info.ValueTypeName} value)");
            sb.AppendLine("        {");
            if (info.UseDispatcher)
            {
                sb.AppendLine("            Avalonia.Threading.Dispatcher.UIThread.Post(() => Evaluate(value));");
            }
            else
            {
                sb.AppendLine("            Evaluate(value);");
            }
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void EvaluateCurrentValue()");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (_resolvedSource is {info.TargetTypeName} typed)");
            sb.AppendLine("            {");
            sb.AppendLine($"                var current = typed.GetValue({info.PropertyOwnerTypeName}.{info.PropertyFieldName});");
            sb.AppendLine("                var value = current;");
            if (info.UseDispatcher)
            {
                sb.AppendLine("                Avalonia.Threading.Dispatcher.UIThread.Post(() => Evaluate(value));");
            }
            else
            {
                sb.AppendLine("                Evaluate(value);");
            }
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private void Evaluate({info.ValueTypeName} current)");
            sb.AppendLine("        {");
            sb.AppendLine("            var left = current;");
            sb.AppendLine("            var right = Value;");
            sb.AppendLine("            bool result = false;");
            sb.AppendLine("            switch (ComparisonCondition)");
            sb.AppendLine("            {");
            sb.AppendLine("                case ComparisonConditionType.Equal:");
            sb.AppendLine($"                    result = EqualityComparer<{info.ValueTypeName}>.Default.Equals(left, right);");
            sb.AppendLine("                    break;");
            sb.AppendLine("                case ComparisonConditionType.NotEqual:");
            sb.AppendLine($"                    result = !EqualityComparer<{info.ValueTypeName}>.Default.Equals(left, right);");
            sb.AppendLine("                    break;");
            sb.AppendLine("                default:");
            sb.AppendLine("                    var leftObj = (object?)left;");
            sb.AppendLine("                    if (leftObj is IComparable cmp)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        var diff = cmp.CompareTo(right);");
            sb.AppendLine("                        switch (ComparisonCondition)");
            sb.AppendLine("                        {");
            sb.AppendLine("                            case ComparisonConditionType.LessThan: result = diff < 0; break;");
            sb.AppendLine("                            case ComparisonConditionType.LessThanOrEqual: result = diff <= 0; break;");
            sb.AppendLine("                            case ComparisonConditionType.GreaterThan: result = diff > 0; break;");
            sb.AppendLine("                            case ComparisonConditionType.GreaterThanOrEqual: result = diff >= 0; break;");
            sb.AppendLine("                        }");
            sb.AppendLine("                    }");
            sb.AppendLine("                    break;");
            sb.AppendLine("            }");
            sb.AppendLine("            if (result)");
            sb.AppendLine("            {");
            sb.AppendLine("                Interaction.ExecuteActions(AssociatedObject, Actions, null);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private static AvaloniaObject? FindInNameScope(AvaloniaObject? source, string sourceName)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (source is not ILogical logical)");
            sb.AppendLine("            {");
            sb.AppendLine("                return null;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            foreach (var ancestor in logical.GetSelfAndLogicalAncestors())");
            sb.AppendLine("            {");
            sb.AppendLine("                if (ancestor is StyledElement styled)");
            sb.AppendLine("                {");
            sb.AppendLine("                    var scope = NameScope.GetNameScope(styled);");
            sb.AppendLine("                    if (scope?.Find(sourceName) is AvaloniaObject found)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        return found;");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine("}");
            }

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private ImmutableArray<PropertyTriggerInfo> CreatePropertyTriggerInfos(INamedTypeSymbol targetType, string propertyPattern, Location? diagnosticLocation, bool includeTypeNamePrefix, Compilation? compilation, bool useDispatcher, string? nameOverride, string? sourceName)
        {
            var results = ImmutableArray.CreateBuilder<PropertyTriggerInfo>();
            foreach (var member in targetType.GetMembers().OfType<IFieldSymbol>())
            {
                if (!NameMatchesPattern(member.Name, propertyPattern))
                    continue;

                var info = CreatePropertyTriggerInfo(member, diagnosticLocation, compilation, useDispatcher, nameOverride, sourceName, includeTypeNamePrefix);
                results.Add(info);
            }

            foreach (var prop in FindMatchingProperties(targetType, propertyPattern))
            {
                var info = CreatePropertyTriggerInfo(prop, diagnosticLocation, compilation, useDispatcher, nameOverride, sourceName, includeTypeNamePrefix);
                results.Add(info);
            }

            return results.ToImmutable();
        }

        private PropertyTriggerInfo CreatePropertyTriggerInfo(IFieldSymbol fieldSymbol, Location? diagnosticLocation, Compilation? compilation, bool useDispatcher, string? nameOverride, string? sourceName, bool includeTypeNamePrefix = false)
        {
            var location = diagnosticLocation ?? Location.None;

            if (fieldSymbol.IsStatic == false)
            {
                return new PropertyTriggerInfo(
                    null,
                    fieldSymbol.Name,
                    "public",
                    "",
                    "",
                    "",
                    "object?",
                    useDispatcher,
                    sourceName,
                    ImmutableArray<Diagnostic>.Empty,
                    Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, fieldSymbol.Name));
            }

            if (!IsAvaloniaPropertyType(fieldSymbol.Type))
            {
                return new PropertyTriggerInfo(null, fieldSymbol.Name, "public", "", "", "", "object?", useDispatcher, sourceName, ImmutableArray<Diagnostic>.Empty, Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, fieldSymbol.Name));
            }

            var valueType = GetAvaloniaPropertyValueType(fieldSymbol.Type) ?? "object?";
            var targetType = fieldSymbol.ContainingType;
            var ns = targetType?.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType?.ContainingNamespace.IsGlobalNamespace == true || ns == "<global namespace>") ? null : ns;
            var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType!) : string.Empty;
            var baseName = nameOverride ?? $"{TrimPropertySuffix(fieldSymbol.Name)}PropertyTrigger";
            var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
            var targetTypeName = ToDisplayStringWithNullable(targetType!);
            var accessibility = GetPropertyTriggerAccessibility(fieldSymbol, targetType!, valueType);
            var ownerTypeName = ToDisplayStringWithNullable(fieldSymbol.ContainingType);

            var validation = ValidatePropertyTrigger(fieldSymbol, location, compilation);
            var warnings = CreateSourceNameWarnings(targetType!, sourceName, location, compilation);
            if (validation != null)
            {
                return new PropertyTriggerInfo(namespaceName, className, accessibility, targetTypeName, ownerTypeName, fieldSymbol.Name, valueType, useDispatcher, sourceName, warnings, validation);
            }

            return new PropertyTriggerInfo(namespaceName, className, accessibility, targetTypeName, ownerTypeName, fieldSymbol.Name, valueType, useDispatcher, sourceName, warnings);
        }

        private PropertyTriggerInfo CreatePropertyTriggerInfo(IPropertySymbol propertySymbol, Location? diagnosticLocation, Compilation? compilation, bool useDispatcher, string? nameOverride, string? sourceName, bool includeTypeNamePrefix = false)
        {
            var location = diagnosticLocation ?? Location.None;
            if (propertySymbol.IsStatic)
            {
                return new PropertyTriggerInfo(null, propertySymbol.Name, "public", "", "", "", "object?", useDispatcher, sourceName, ImmutableArray<Diagnostic>.Empty, Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, propertySymbol.Name));
            }

            var backingField = FindPropertyField(propertySymbol.ContainingType, propertySymbol.Name + "Property");
            if (backingField == null)
            {
                return new PropertyTriggerInfo(null, propertySymbol.Name, "public", "", "", "", "object?", useDispatcher, sourceName, ImmutableArray<Diagnostic>.Empty, Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, propertySymbol.Name));
            }

            return CreatePropertyTriggerInfo(backingField, diagnosticLocation, compilation, useDispatcher, nameOverride, sourceName, includeTypeNamePrefix);
        }

        private static IFieldSymbol? FindPropertyField(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var field = type.GetMembers(name).OfType<IFieldSymbol>().FirstOrDefault();
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }

        private static bool IsAvaloniaPropertyType(ITypeSymbol typeSymbol)
        {
            var display = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return display.StartsWith("global::Avalonia.StyledProperty", System.StringComparison.Ordinal) ||
                   display.StartsWith("global::Avalonia.DirectProperty", System.StringComparison.Ordinal);
        }

        private static string? GetAvaloniaPropertyValueType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol named)
            {
                if (named.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::Avalonia.StyledProperty<T>")
                {
                    return ToDisplayStringWithNullable(named.TypeArguments[0]);
                }
                if (named.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::Avalonia.DirectProperty<TOwner, TValue>")
                {
                    return ToDisplayStringWithNullable(named.TypeArguments.Last());
                }
                if (named.Name == "StyledProperty" && named.TypeArguments.Length == 1)
                {
                    return ToDisplayStringWithNullable(named.TypeArguments[0]);
                }
                if (named.Name == "DirectProperty" && named.TypeArguments.Length == 2)
                {
                    return ToDisplayStringWithNullable(named.TypeArguments[1]);
                }
            }
            return null;
        }

        private static string TrimPropertySuffix(string name)
        {
            return name.EndsWith("Property", System.StringComparison.Ordinal) ? name.Substring(0, name.Length - "Property".Length) : name;
        }

        private Diagnostic? ValidatePropertyTrigger(IFieldSymbol fieldSymbol, Location? location, Compilation? compilation)
        {
            if (ContainsTypeParameter(fieldSymbol.Type))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location ?? Location.None, fieldSymbol.Name);
            }

            if (ContainsTypeParameter(fieldSymbol.ContainingType))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location ?? Location.None, fieldSymbol.Name);
            }

            var containingTypeDiagnostic = ValidateTypeAccessibility(fieldSymbol.ContainingType, location, compilation);
            if (containingTypeDiagnostic != null)
            {
                return containingTypeDiagnostic;
            }

            if (!IsAccessibleToGenerator(fieldSymbol, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location ?? Location.None, fieldSymbol.Name, fieldSymbol.ContainingType.ToDisplayString());
            }

            var valueType = GetAvaloniaPropertyValueType(fieldSymbol.Type);
            if (valueType != null && !IsAccessibleType(fieldSymbol.Type, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location ?? Location.None, fieldSymbol.Name, fieldSymbol.ContainingType.ToDisplayString());
            }

            return null;
        }

        private ImmutableArray<Diagnostic> CreateSourceNameWarnings(INamedTypeSymbol targetType, string? sourceName, Location? location, Compilation? compilation)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return ImmutableArray<Diagnostic>.Empty;
            }

            if (IsLogicalType(targetType))
            {
                return ImmutableArray<Diagnostic>.Empty;
            }

            return ImmutableArray.Create(Diagnostic.Create(PropertyTriggerSourceNameNotLogicalDiagnostic, location ?? Location.None, targetType.ToDisplayString()));
        }

        private static bool IsLogicalType(INamedTypeSymbol typeSymbol)
        {
            if (InheritsFrom(typeSymbol, "Avalonia.LogicalTree.ILogical"))
            {
                return true;
            }

            foreach (var iface in typeSymbol.AllInterfaces)
            {
                var name = iface.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if (name.StartsWith("global::", System.StringComparison.Ordinal))
                {
                    name = name.Substring("global::".Length);
                }
                if (name == "Avalonia.LogicalTree.ILogical")
                {
                    return true;
                }
            }

            return false;
        }

        private string GetPropertyTriggerAccessibility(IFieldSymbol fieldSymbol, INamedTypeSymbol containingType, string valueTypeName)
        {
            var requiresInternal = containingType.DeclaredAccessibility == Accessibility.Internal ||
                                   fieldSymbol.DeclaredAccessibility == Accessibility.Internal ||
                                   ContainsInternalType(fieldSymbol.Type) ||
                                   valueTypeName.StartsWith("global::", System.StringComparison.Ordinal) && valueTypeName.Contains("Internal");

            return requiresInternal ? "internal" : "public";
        }

        private static string? GetNameOverride(AttributeData attributeData, SemanticModel semanticModel)
        {
            foreach (var namedArgument in attributeData.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, "Name", System.StringComparison.Ordinal) && namedArgument.Value.Value is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return s;
                }
            }

            if (attributeData.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax)
            {
                return GetNameOverride(attributeSyntax, semanticModel);
            }

            return null;
        }

        private static string? GetNameOverride(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            if (attributeSyntax.ArgumentList == null)
            {
                return null;
            }

            foreach (var argument in attributeSyntax.ArgumentList.Arguments)
            {
                if (argument.NameEquals?.Name.Identifier.Text == "Name")
                {
                    var constant = semanticModel.GetConstantValue(argument.Expression);
                    if (constant.HasValue && constant.Value is string s && !string.IsNullOrWhiteSpace(s))
                    {
                        return s;
                    }
                }
            }

            return null;
        }

        private static string? GetSourceName(AttributeData attributeData, SemanticModel semanticModel)
        {
            foreach (var namedArgument in attributeData.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, "SourceName", System.StringComparison.Ordinal) && namedArgument.Value.Value is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return s;
                }
            }

            if (attributeData.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax attributeSyntax)
            {
                return GetSourceName(attributeSyntax, semanticModel);
            }

            return null;
        }

        private static string? GetSourceName(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            if (attributeSyntax.ArgumentList == null)
            {
                return null;
            }

            foreach (var argument in attributeSyntax.ArgumentList.Arguments)
            {
                if (argument.NameEquals?.Name.Identifier.Text == "SourceName")
                {
                    var constant = semanticModel.GetConstantValue(argument.Expression);
                    if (constant.HasValue && constant.Value is string s && !string.IsNullOrWhiteSpace(s))
                    {
                        return s;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<PropertyTriggerInfo> EnsureUniquePropertyTriggers(IEnumerable<PropertyTriggerInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => (info.Namespace, info.ClassName)))
            {
                var distinct = group
                    .GroupBy(info => (info.TargetTypeName, info.PropertyFieldName))
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
