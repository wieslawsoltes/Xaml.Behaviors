// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xaml.Behaviors.SourceGenerators.Platforms;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        private const string WinUIDependencyPropertyTypeName = "global::Microsoft.UI.Xaml.DependencyProperty";
        private const string WinUIDependencyObjectTypeName = "Microsoft.UI.Xaml.DependencyObject";
        private const string WinUIFrameworkElementTypeName = "Microsoft.UI.Xaml.FrameworkElement";
        private const string NotifyPropertyChangedTypeName = "System.ComponentModel.INotifyPropertyChanged";

        private static readonly NameScopeLookupNames PropertyTriggerNameScopeLookupNames = new("logical", "ancestor", "styled", "scope");

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
            PlatformDiagnostics PlatformDiagnostics,
            Diagnostic? Diagnostic = null,
            PropertyObservationKind ObservationKind = PropertyObservationKind.AvaloniaProperty,
            string? ClrPropertyName = null,
            string? ValueTypeOf = null)
        {
            public PropertyObservation ToObservation() =>
                new(ObservationKind, TargetTypeName, PropertyOwnerTypeName, PropertyFieldName, ValueTypeName, ClrPropertyName);
        }

        private void RegisterPropertyTriggerGeneration(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<TargetPlatform> platform)
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

            context.RegisterSourceOutput(uniqueTriggers.Combine(platform), (spc, source) => ExecuteGeneratePropertyTrigger(spc, source.Left, XamlPlatforms.Get(source.Right)));
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

        private void ExecuteGeneratePropertyTrigger(SourceProductionContext spc, PropertyTriggerInfo info, IXamlPlatform platform)
        {
            if (ReportPlatformDiagnostics(spc, info.PlatformDiagnostics, platform.Kind))
            {
                return;
            }

            if (info.Diagnostic != null)
            {
                spc.ReportDiagnostic(info.Diagnostic);
                if (info.Diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    return;
                }
            }

            var properties = platform.Properties;
            var observation = info.ToObservation();
            var sourceNameDefault = info.DefaultSourceName is null
                ? "default(string?)"
                : $"\"{info.DefaultSourceName.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
            var propertySpecs = new[]
            {
                new PropertySpec("SourceObject", "object?", "object"),
                new PropertySpec("SourceName", "string?", "string", sourceNameDefault),
                new PropertySpec("ComparisonCondition", "ComparisonConditionType", "ComparisonConditionType"),
                new PropertySpec("Value", info.ValueTypeName, info.ValueTypeOf ?? TrimNullableAnnotation(info.ValueTypeName)),
            };

            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("#nullable enable");
            platform.Usings.Append(sb, GeneratedSourceKind.PropertyTrigger);
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    {info.Accessibility} partial class {info.ClassName} : {platform.Types.StyledElementTrigger}");
            sb.AppendLine("    {");
            foreach (var property in propertySpecs)
            {
                properties.AppendField(sb, info.ClassName, property);
                sb.AppendLine();
            }
            platform.PropertyObservation.AppendFields(sb, observation);
            sb.AppendLine("        private object? _resolvedSource;");
            sb.AppendLine();
            foreach (var property in propertySpecs)
            {
                properties.AppendAccessor(sb, property);
                sb.AppendLine();
            }
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
            sb.AppendLine($"        protected override void OnPropertyChanged({properties.ChangedEventArgsType} change)");
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
            platform.PropertyObservation.AppendSubscribe(sb, observation);
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Unsubscribe()");
            sb.AppendLine("        {");
            platform.PropertyObservation.AppendUnsubscribe(sb, observation);
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private void OnObserved({info.ValueTypeName} value)");
            sb.AppendLine("        {");
            if (info.UseDispatcher)
            {
                sb.AppendLine($"            {platform.Dispatcher.PostMethod}(() => Evaluate(value));");
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
            sb.AppendLine($"                var current = {platform.PropertyObservation.CurrentValue(observation, "typed")};");
            sb.AppendLine("                var value = current;");
            if (info.UseDispatcher)
            {
                sb.AppendLine($"                {platform.Dispatcher.PostMethod}(() => Evaluate(value));");
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
            platform.NameScope.AppendFindInNameScope(sb, PropertyTriggerNameScopeLookupNames);
            platform.PropertyObservation.AppendHelpers(sb, observation);
            platform.Dispatcher.AppendHelpers(sb, info.UseDispatcher ? DispatcherFeatures.Post : DispatcherFeatures.None);
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

        private static PropertyTriggerInfo CreateInvalidPropertyTriggerInfo(string memberName, bool useDispatcher, string? sourceName, Diagnostic? diagnostic, PlatformDiagnostics? platformDiagnostics = null)
        {
            return new PropertyTriggerInfo(null, memberName, "public", "", "", "", "object?", useDispatcher, sourceName, platformDiagnostics ?? PlatformDiagnostics.Empty, diagnostic);
        }

        private PropertyTriggerInfo CreatePropertyTriggerInfo(IFieldSymbol fieldSymbol, Location? diagnosticLocation, Compilation? compilation, bool useDispatcher, string? nameOverride, string? sourceName, bool includeTypeNamePrefix = false, ITypeSymbol? dependencyPropertyValueType = null)
        {
            var location = diagnosticLocation ?? Location.None;

            if (fieldSymbol.IsStatic == false)
            {
                return CreateInvalidPropertyTriggerInfo(fieldSymbol.Name, useDispatcher, sourceName, Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, fieldSymbol.Name));
            }

            PropertyObservationKind kind;
            string valueType;
            string? valueTypeOf;
            if (IsAvaloniaPropertyType(fieldSymbol.Type))
            {
                kind = PropertyObservationKind.AvaloniaProperty;
                valueType = GetAvaloniaPropertyValueType(fieldSymbol.Type) ?? "object?";
                valueTypeOf = GetAvaloniaPropertyValueTypeOf(fieldSymbol.Type);
            }
            else if (IsWinUIDependencyPropertyType(fieldSymbol.Type))
            {
                kind = PropertyObservationKind.DependencyProperty;
                var clrType = dependencyPropertyValueType ?? FindDependencyPropertyValueType(fieldSymbol.ContainingType, TrimPropertySuffix(fieldSymbol.Name));
                valueType = clrType is null ? "object?" : ToDisplayStringWithNullable(clrType);
                valueTypeOf = clrType is null ? "object" : ToTypeOfString(clrType);
            }
            else
            {
                return CreateInvalidPropertyTriggerInfo(fieldSymbol.Name, useDispatcher, sourceName, Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, fieldSymbol.Name));
            }

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
            var platformDiagnostics = kind == PropertyObservationKind.AvaloniaProperty
                ? new PlatformDiagnostics(
                    CreateSourceNameWarnings(targetType!, sourceName, location, compilation),
                    ImmutableArray.Create(Diagnostic.Create(PropertyTriggerNotObservableDiagnostic, location, fieldSymbol.Name, targetTypeName)))
                : new PlatformDiagnostics(
                    ImmutableArray.Create(Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, fieldSymbol.Name)),
                    CreateWinUIDependencyPropertyDiagnostics(targetType!, fieldSymbol.Name, sourceName, location));

            return new PropertyTriggerInfo(namespaceName, className, accessibility, targetTypeName, ownerTypeName, fieldSymbol.Name, valueType, useDispatcher, sourceName, platformDiagnostics, validation, kind, ValueTypeOf: valueTypeOf);
        }

        private PropertyTriggerInfo CreatePropertyTriggerInfo(IPropertySymbol propertySymbol, Location? diagnosticLocation, Compilation? compilation, bool useDispatcher, string? nameOverride, string? sourceName, bool includeTypeNamePrefix = false)
        {
            var location = diagnosticLocation ?? Location.None;
            if (propertySymbol.IsStatic)
            {
                return CreateInvalidPropertyTriggerInfo(propertySymbol.Name, useDispatcher, sourceName, Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, propertySymbol.Name));
            }

            var backingField = FindPropertyField(propertySymbol.ContainingType, propertySymbol.Name + "Property");
            if (backingField != null)
            {
                return CreatePropertyTriggerInfo(backingField, diagnosticLocation, compilation, useDispatcher, nameOverride, sourceName, includeTypeNamePrefix, propertySymbol.Type);
            }

            var avaloniaDiagnostic = Diagnostic.Create(PropertyTriggerInvalidPropertyTypeDiagnostic, location, propertySymbol.Name);
            if (!ImplementsInterface(propertySymbol.ContainingType, NotifyPropertyChangedTypeName))
            {
                // Avalonia requires a styled/direct property; WinUI additionally accepts INotifyPropertyChanged sources.
                var winUIDiagnostic = Diagnostic.Create(PropertyTriggerNotObservableDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
                return CreateInvalidPropertyTriggerInfo(
                    propertySymbol.Name,
                    useDispatcher,
                    sourceName,
                    diagnostic: null,
                    new PlatformDiagnostics(ImmutableArray.Create(avaloniaDiagnostic), ImmutableArray.Create(winUIDiagnostic)));
            }

            return CreateNotifyPropertyTriggerInfo(propertySymbol, location, compilation, useDispatcher, nameOverride, sourceName, includeTypeNamePrefix, avaloniaDiagnostic);
        }

        private PropertyTriggerInfo CreateNotifyPropertyTriggerInfo(IPropertySymbol propertySymbol, Location location, Compilation? compilation, bool useDispatcher, string? nameOverride, string? sourceName, bool includeTypeNamePrefix, Diagnostic avaloniaDiagnostic)
        {
            var targetType = propertySymbol.ContainingType;
            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType) : string.Empty;
            var baseName = nameOverride ?? $"{propertySymbol.Name}PropertyTrigger";
            var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
            var targetTypeName = ToDisplayStringWithNullable(targetType);
            var valueType = ToDisplayStringWithNullable(propertySymbol.Type);
            var requiresInternal = targetType.DeclaredAccessibility == Accessibility.Internal ||
                                   propertySymbol.DeclaredAccessibility == Accessibility.Internal ||
                                   ContainsInternalType(propertySymbol.Type);
            var accessibility = requiresInternal ? "internal" : "public";

            var validation = ValidateNotifyPropertyTrigger(propertySymbol, location, compilation);
            var winUIDiagnostics = CreateWinUISourceNameWarnings(targetType, sourceName, location);
            var platformDiagnostics = new PlatformDiagnostics(ImmutableArray.Create(avaloniaDiagnostic), winUIDiagnostics);

            return new PropertyTriggerInfo(
                namespaceName,
                className,
                accessibility,
                targetTypeName,
                targetTypeName,
                string.Empty,
                valueType,
                useDispatcher,
                sourceName,
                platformDiagnostics,
                validation,
                PropertyObservationKind.NotifyPropertyChanged,
                propertySymbol.Name,
                ToTypeOfString(propertySymbol.Type));
        }

        private static Diagnostic? ValidateNotifyPropertyTrigger(IPropertySymbol propertySymbol, Location location, Compilation? compilation)
        {
            if (ContainsTypeParameter(propertySymbol.ContainingType) || ContainsTypeParameter(propertySymbol.Type))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location, propertySymbol.Name);
            }

            var containingTypeDiagnostic = ValidateTypeAccessibility(propertySymbol.ContainingType, location, compilation);
            if (containingTypeDiagnostic != null)
            {
                return containingTypeDiagnostic;
            }

            if (!IsAccessibleToGenerator(propertySymbol, compilation) ||
                propertySymbol.GetMethod is null ||
                !IsAccessibleToGenerator(propertySymbol.GetMethod, compilation) ||
                !IsAccessibleType(propertySymbol.Type, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
            }

            return null;
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

        private static bool IsWinUIDependencyPropertyType(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == WinUIDependencyPropertyTypeName;
        }

        /// <summary>
        /// Finds the value type of a WinUI dependency property from its CLR accessor property or its attached <c>Get{Name}</c> method.
        /// </summary>
        private static ITypeSymbol? FindDependencyPropertyValueType(INamedTypeSymbol? type, string propertyName)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                var property = current.GetMembers(propertyName).OfType<IPropertySymbol>().FirstOrDefault(p => !p.IsStatic && !p.IsIndexer);
                if (property != null)
                {
                    return property.Type;
                }

                var getter = current.GetMembers("Get" + propertyName).OfType<IMethodSymbol>().FirstOrDefault(m => m.IsStatic && m.Parameters.Length == 1 && !m.ReturnsVoid);
                if (getter != null)
                {
                    return getter.ReturnType;
                }
            }

            return null;
        }

        private static string? GetAvaloniaPropertyValueType(ITypeSymbol typeSymbol)
        {
            var valueType = GetAvaloniaPropertyValueTypeSymbol(typeSymbol);
            return valueType is null ? null : ToDisplayStringWithNullable(valueType);
        }

        private static string? GetAvaloniaPropertyValueTypeOf(ITypeSymbol typeSymbol)
        {
            var valueType = GetAvaloniaPropertyValueTypeSymbol(typeSymbol);
            return valueType is null ? null : ToTypeOfString(valueType);
        }

        private static ITypeSymbol? GetAvaloniaPropertyValueTypeSymbol(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is INamedTypeSymbol named)
            {
                if (named.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::Avalonia.StyledProperty<T>")
                {
                    return named.TypeArguments[0];
                }
                if (named.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::Avalonia.DirectProperty<TOwner, TValue>")
                {
                    return named.TypeArguments.Last();
                }
                if (named.Name == "StyledProperty" && named.TypeArguments.Length == 1)
                {
                    return named.TypeArguments[0];
                }
                if (named.Name == "DirectProperty" && named.TypeArguments.Length == 2)
                {
                    return named.TypeArguments[1];
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

        private static ImmutableArray<Diagnostic> CreateWinUISourceNameWarnings(INamedTypeSymbol targetType, string? sourceName, Location location)
        {
            if (string.IsNullOrWhiteSpace(sourceName) || InheritsFrom(targetType, WinUIFrameworkElementTypeName))
            {
                return ImmutableArray<Diagnostic>.Empty;
            }

            return ImmutableArray.Create(Diagnostic.Create(PropertyTriggerSourceNameNotLogicalDiagnostic, location, targetType.ToDisplayString()));
        }

        private static ImmutableArray<Diagnostic> CreateWinUIDependencyPropertyDiagnostics(INamedTypeSymbol ownerType, string fieldName, string? sourceName, Location location)
        {
            if (!InheritsFrom(ownerType, WinUIDependencyObjectTypeName) && !ImplementsInterface(ownerType, WinUIDependencyObjectTypeName))
            {
                return ImmutableArray.Create(Diagnostic.Create(PropertyTriggerOwnerNotDependencyObjectDiagnostic, location, fieldName, ownerType.ToDisplayString()));
            }

            return CreateWinUISourceNameWarnings(ownerType, sourceName, location);
        }

        private static bool ImplementsInterface(INamedTypeSymbol typeSymbol, string interfaceName)
        {
            if (typeSymbol.TypeKind == TypeKind.Interface && typeSymbol.ToDisplayString() == interfaceName)
            {
                return true;
            }

            return typeSymbol.AllInterfaces.Any(i => i.ToDisplayString() == interfaceName);
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
                    .GroupBy(info => (info.TargetTypeName, info.PropertyFieldName, info.ClrPropertyName ?? string.Empty, info.UseDispatcher, info.DefaultSourceName ?? string.Empty))
                    .Select(g => g.FirstOrDefault(info => info.Diagnostic is null) ?? g.First())
                    .ToList();

                if (distinct.Count == 1)
                {
                    yield return distinct[0];
                    continue;
                }

                foreach (var info in distinct)
                {
                    var scope = $"{info.TargetTypeName}|{(info.ClrPropertyName ?? info.PropertyFieldName)}|{info.UseDispatcher}|{info.DefaultSourceName ?? string.Empty}";
                    yield return info with { ClassName = MakeUniqueName(info.ClassName, scope) };
                }
            }
        }
    }
}
