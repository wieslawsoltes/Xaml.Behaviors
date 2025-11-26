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
        private record ParameterPathSegment(string Name, bool IsNullableValueType, bool NeedsNullCheck);

        private record EventCommandInfo(
            string? Namespace,
            string ClassName,
            string Accessibility,
            string TargetTypeName,
            string EventName,
            string EventHandlerType,
            ImmutableArray<TriggerParameter> Parameters,
            bool UseDispatcher,
            string? DefaultParameterPath,
            ImmutableArray<ParameterPathSegment> ParameterPathSegments,
            Diagnostic? Diagnostic = null);

        private void RegisterEventCommandGeneration(IncrementalGeneratorInitializationContext context)
        {
            var eventCommands = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateEventCommandAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetEventCommandToGenerate(ctx))
                .SelectMany((x, _) => x);

            var assemblyCommands = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node, "GenerateEventCommand"),
                    transform: (ctx, _) => GetAssemblyEventCommandFromAttributeSyntax(ctx))
                .SelectMany((x, _) => x);

            var unique = eventCommands
                .Collect()
                .Combine(assemblyCommands.Collect())
                .SelectMany((data, _) => EnsureUniqueEventCommands(data.Left.Concat(data.Right)));

            context.RegisterSourceOutput(unique, ExecuteGenerateEventCommand);
        }

        private ImmutableArray<EventCommandInfo> GetEventCommandToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var builder = ImmutableArray.CreateBuilder<EventCommandInfo>();
            if (context.TargetSymbol is IAssemblySymbol)
            {
                return builder.ToImmutable();
            }

            var eventSymbol = context.TargetSymbol as IEventSymbol;
            if (eventSymbol == null && context.TargetSymbol is IFieldSymbol fieldSymbol)
            {
                eventSymbol = FindEvent(fieldSymbol.ContainingType, fieldSymbol.Name);
            }

            if (eventSymbol != null)
            {
                foreach (var attribute in context.Attributes)
                {
                    var useDispatcher = GetUseDispatcherFlag(attribute, context.SemanticModel);
                    var nameOverride = GetNameOverride(attribute, context.SemanticModel);
                    var parameterPath = GetParameterPath(attribute, context.SemanticModel);
                    var diagnosticLocation = attribute.ApplicationSyntaxReference?.GetSyntax()?.GetLocation() ?? context.TargetNode?.GetLocation() ?? Location.None;
                    var info = CreateEventCommandInfo(eventSymbol, diagnosticLocation, includeTypeNamePrefix: false, context.SemanticModel.Compilation, useDispatcher, nameOverride, parameterPath);
                    builder.Add(info);
                }
            }

            return builder.ToImmutable();
        }

        private ImmutableArray<EventCommandInfo> GetAssemblyEventCommandFromAttributeSyntax(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return ImmutableArray<EventCommandInfo>.Empty;

            if (attributeSyntax.ArgumentList is null || attributeSyntax.ArgumentList.Arguments.Count < 2)
                return ImmutableArray<EventCommandInfo>.Empty;

            if (attributeSyntax.ArgumentList.Arguments[0].Expression is not TypeOfExpressionSyntax typeOfExpression)
                return ImmutableArray<EventCommandInfo>.Empty;

            if (attributeSyntax.ArgumentList.Arguments[1].Expression is not LiteralExpressionSyntax eventLiteral)
                return ImmutableArray<EventCommandInfo>.Empty;

            var eventName = eventLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(eventName))
                return ImmutableArray<EventCommandInfo>.Empty;

            var useDispatcher = GetBoolNamedArgument(attributeSyntax, context.SemanticModel, "UseDispatcher");
            var nameOverride = GetNameOverride(attributeSyntax, context.SemanticModel);
            var parameterPath = GetParameterPath(attributeSyntax, context.SemanticModel);

            return CreateEventCommandInfos(targetType, eventName, context.Node.GetLocation(), includeTypeNamePrefix: true, context.SemanticModel.Compilation, useDispatcher, nameOverride, parameterPath);
        }

        private void ExecuteGenerateEventCommand(SourceProductionContext spc, EventCommandInfo info)
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
            sb.AppendLine("using System.Windows.Input;");
            sb.AppendLine("using Avalonia;");
            sb.AppendLine("using Avalonia.Controls;");
            sb.AppendLine("using Avalonia.LogicalTree;");
            sb.AppendLine("using Avalonia.Xaml.Interactivity;");
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
            sb.AppendLine("        public static readonly StyledProperty<string?> SourceNameProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, string?>(nameof(SourceName));");
            sb.AppendLine();
            sb.AppendLine("        public static readonly StyledProperty<ICommand?> CommandProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, ICommand?>(nameof(Command));");
            sb.AppendLine();
            var parameterPathDefault = info.DefaultParameterPath is null
                ? "default(string?)"
                : $"\"{info.DefaultParameterPath.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
            sb.AppendLine("        public static readonly StyledProperty<string?> ParameterPathProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, string?>(nameof(ParameterPath), {parameterPathDefault});");
            sb.AppendLine();
            sb.AppendLine("        public static readonly StyledProperty<object?> ParameterProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, object?>(nameof(Parameter));");
            sb.AppendLine();
            sb.AppendLine("        private EventProxy? _proxy;");
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
            sb.AppendLine("        public ICommand? Command");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(CommandProperty);");
            sb.AppendLine("            set => SetValue(CommandProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public string? ParameterPath");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(ParameterPathProperty);");
            sb.AppendLine("            set => SetValue(ParameterPathProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public object? Parameter");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(ParameterProperty);");
            sb.AppendLine("            set => SetValue(ParameterProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnAttached()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnAttached();");
            sb.AppendLine("            UpdateSource();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnDetaching()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnDetaching();");
            sb.AppendLine("            if (_resolvedSource is not null)");
            sb.AppendLine("            {");
            sb.AppendLine("                 UnregisterEvent(_resolvedSource);");
            sb.AppendLine("                 _resolvedSource = null;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnPropertyChanged(change);");
            sb.AppendLine("            if (change.Property == SourceObjectProperty || change.Property == SourceNameProperty)");
            sb.AppendLine("            {");
            sb.AppendLine("                UpdateSource();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void UpdateSource()");
            sb.AppendLine("        {");
            sb.AppendLine("            var newSource = ResolveSource();");
            sb.AppendLine("            if (!ReferenceEquals(newSource, _resolvedSource))");
            sb.AppendLine("            {");
            sb.AppendLine("                if (_resolvedSource is not null) UnregisterEvent(_resolvedSource);");
            sb.AppendLine("                _resolvedSource = newSource;");
            sb.AppendLine("                if (_resolvedSource is not null) RegisterEvent(_resolvedSource);");
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
            sb.AppendLine("        private void RegisterEvent(object source)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (source is {info.TargetTypeName} typedSource)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _proxy = new EventProxy(this, typedSource);");
            sb.AppendLine($"                typedSource.{info.EventName} += _proxy.OnEvent;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void UnregisterEvent(object source)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (source is {info.TargetTypeName} typedSource && _proxy != null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                typedSource.{info.EventName} -= _proxy.OnEvent;");
            sb.AppendLine($"                _proxy = null;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void OnEvent(object? sender, object? args)");
            sb.AppendLine("        {");
            sb.AppendLine("            var command = Command;");
            sb.AppendLine("            if (command is null)");
            sb.AppendLine("            {");
            sb.AppendLine("                return;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            object? parameter = ResolveParameter(args);");
            sb.AppendLine();
            if (info.UseDispatcher)
            {
                sb.AppendLine("            Avalonia.Threading.Dispatcher.UIThread.Post(() => ExecuteCommand(command, parameter));");
            }
            else
            {
                sb.AppendLine("            ExecuteCommand(command, parameter);");
            }
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void ExecuteCommand(ICommand command, object? parameter)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (command.CanExecute(parameter))");
            sb.AppendLine("            {");
            sb.AppendLine("                command.Execute(parameter);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private object? ResolveParameter(object? eventArgs)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (IsSet(ParameterProperty))");
            sb.AppendLine("            {");
            sb.AppendLine("                return Parameter;");
            sb.AppendLine("            }");
            sb.AppendLine();
            if (info.ParameterPathSegments.Length > 0 && info.DefaultParameterPath is not null && info.Parameters.Length > 0)
            {
                var parameterPathLiteral = info.DefaultParameterPath.Replace("\\", "\\\\").Replace("\"", "\\\"");
                var eventArgsType = TrimNullableAnnotation(info.Parameters.Last().Type);
                sb.AppendLine("            if (!string.IsNullOrEmpty(ParameterPath) &&");
                sb.AppendLine($"                string.Equals(ParameterPath, \"{parameterPathLiteral}\", StringComparison.Ordinal) &&");
                sb.AppendLine($"                eventArgs is {eventArgsType} typedArgs &&");
                sb.AppendLine("                TryResolveParameterPath(typedArgs, out var parameterFromPath))");
                sb.AppendLine("            {");
                sb.AppendLine("                return parameterFromPath;");
                sb.AppendLine("            }");
                sb.AppendLine();
            }
            sb.AppendLine("            return eventArgs;");
            sb.AppendLine("        }");
            if (info.ParameterPathSegments.Length > 0 && info.Parameters.Length > 0)
            {
                var eventArgsType = TrimNullableAnnotation(info.Parameters.Last().Type);
                sb.AppendLine();
                sb.AppendLine($"        private static bool TryResolveParameterPath({eventArgsType} args, out object? result)");
                sb.AppendLine("        {");
                sb.AppendLine("            result = null;");
                var currentIdentifier = "args";
                for (int i = 0; i < info.ParameterPathSegments.Length; i++)
                {
                    var segment = info.ParameterPathSegments[i];
                    var segmentName = EscapeIdentifier(segment.Name);
                    var variableName = $"p{i}";
                    sb.AppendLine($"            var {variableName} = {currentIdentifier}.{segmentName};");
                    if (segment.IsNullableValueType)
                    {
                        sb.AppendLine($"            if (!{variableName}.HasValue)");
                        sb.AppendLine("            {");
                        sb.AppendLine("                result = null;");
                        sb.AppendLine("                return true;");
                        sb.AppendLine("            }");
                        var valueName = $"{variableName}Value";
                        sb.AppendLine($"            var {valueName} = {variableName}.Value;");
                        currentIdentifier = valueName;
                    }
                    else
                    {
                        if (segment.NeedsNullCheck)
                        {
                            sb.AppendLine($"            if ({variableName} is null)");
                            sb.AppendLine("            {");
                            sb.AppendLine("                result = null;");
                            sb.AppendLine("                return true;");
                            sb.AppendLine("            }");
                        }
                        currentIdentifier = variableName;
                    }
                }
                sb.AppendLine($"            result = {currentIdentifier};");
                sb.AppendLine("            return true;");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            sb.AppendLine("        private class EventProxy");
            sb.AppendLine("        {");
            sb.AppendLine($"            private readonly WeakReference<{info.ClassName}> _trigger;");
            sb.AppendLine($"            private readonly WeakReference<{TrimNullableAnnotation(info.TargetTypeName)}> _source;");
            sb.AppendLine();
            sb.AppendLine($"            public EventProxy({info.ClassName} trigger, {TrimNullableAnnotation(info.TargetTypeName)} source)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _trigger = new WeakReference<{info.ClassName}>(trigger);");
            sb.AppendLine($"                _source = new WeakReference<{TrimNullableAnnotation(info.TargetTypeName)}>(source);");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            public void OnEvent({FormatParameters(info.Parameters)})");
            sb.AppendLine("            {");
            sb.AppendLine("                if (_trigger.TryGetTarget(out var trigger))");
            sb.AppendLine("                {");
            var senderArg = info.Parameters.Length > 0 ? info.Parameters[0].Name : "null";
            var argsArg = info.Parameters.Length > 1 ? info.Parameters.Last().Name : (info.Parameters.Length > 0 ? info.Parameters[0].Name : "null");
            sb.AppendLine($"                    trigger.OnEvent({senderArg}, {argsArg});");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                {");
            sb.AppendLine("                    if (_source.TryGetTarget(out var typedSource))");
            sb.AppendLine("                    {");
            sb.AppendLine($"                        typedSource.{info.EventName} -= OnEvent;");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private static AvaloniaObject? FindInNameScope(AvaloniaObject? source, string sourceName)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (source is not ILogical logicalSource)");
            sb.AppendLine("            {");
            sb.AppendLine("                return null;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            foreach (var logical in logicalSource.GetSelfAndLogicalAncestors())");
            sb.AppendLine("            {");
            sb.AppendLine("                if (logical is StyledElement styledElement)");
            sb.AppendLine("                {");
            sb.AppendLine("                    var nameScope = NameScope.GetNameScope(styledElement);");
            sb.AppendLine("                    if (nameScope?.Find(sourceName) is AvaloniaObject found)");
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

        private ImmutableArray<EventCommandInfo> CreateEventCommandInfos(INamedTypeSymbol targetType, string eventPattern, Location? diagnosticLocation, bool includeTypeNamePrefix, Compilation? compilation, bool useDispatcher, string? nameOverride, string? parameterPath)
        {
            var matchingEvents = FindMatchingEvents(targetType, eventPattern);
            if (matchingEvents.Length == 0)
            {
                var diagnostic = Diagnostic.Create(TriggerEventNotFoundDiagnostic, diagnosticLocation ?? Location.None, eventPattern, targetType.Name);
                return ImmutableArray.Create(new EventCommandInfo("", "", "public", eventPattern, "", "", ImmutableArray<TriggerParameter>.Empty, useDispatcher, parameterPath, ImmutableArray<ParameterPathSegment>.Empty, diagnostic));
            }

            var builder = ImmutableArray.CreateBuilder<EventCommandInfo>();
            var invalidBuilder = ImmutableArray.CreateBuilder<EventCommandInfo>();
            foreach (var evt in matchingEvents)
            {
                var info = CreateEventCommandInfo(evt, diagnosticLocation, includeTypeNamePrefix, compilation, useDispatcher, nameOverride, parameterPath);
                if (info.Diagnostic is null)
                {
                    builder.Add(info);
                }
                else
                {
                    invalidBuilder.Add(info);
                }
            }

            if (builder.Count > 0)
            {
                return builder.ToImmutable();
            }

            if (invalidBuilder.Count > 0)
            {
                return invalidBuilder.ToImmutable();
            }

            var fallback = Diagnostic.Create(TriggerEventNotFoundDiagnostic, diagnosticLocation ?? Location.None, eventPattern, targetType.Name);
            return ImmutableArray.Create(new EventCommandInfo("", "", "public", eventPattern, "", "", ImmutableArray<TriggerParameter>.Empty, useDispatcher, parameterPath, ImmutableArray<ParameterPathSegment>.Empty, fallback));
        }

        private EventCommandInfo CreateEventCommandInfo(IEventSymbol eventSymbol, Location? diagnosticLocation, bool includeTypeNamePrefix, Compilation? compilation, bool useDispatcher, string? nameOverride, string? parameterPath)
        {
            var targetType = eventSymbol.ContainingType;
            var ns = targetType?.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType?.ContainingNamespace.IsGlobalNamespace == true || ns == "<global namespace>") ? null : ns;
            var baseName = nameOverride ?? $"{eventSymbol.Name}EventCommandTrigger";
            var typePrefix = includeTypeNamePrefix && targetType != null ? GetTypeNamePrefix(targetType) : string.Empty;
            var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
            var targetTypeName = targetType != null ? ToDisplayStringWithNullable(targetType) : string.Empty;
            var eventName = eventSymbol.Name;
            var eventHandlerType = ToDisplayStringWithNullable(eventSymbol.Type);
            if (targetType?.ContainingType != null)
            {
                var nestedAccessibility = targetType.DeclaredAccessibility == Accessibility.Internal || ContainsInternalType(targetType)
                    ? "internal"
                    : "public";
                return new EventCommandInfo(namespaceName, className, nestedAccessibility, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, useDispatcher, parameterPath, ImmutableArray<ParameterPathSegment>.Empty, Diagnostic.Create(NestedTypeNotSupportedDiagnostic, diagnosticLocation ?? Location.None, targetType.ToDisplayString()));
            }
            var requiresInternal = targetType?.DeclaredAccessibility == Accessibility.Internal ||
                                   eventSymbol.DeclaredAccessibility == Accessibility.Internal ||
                                   ContainsInternalType(eventSymbol.Type);
            var validationDiagnostic = ValidateEventSymbol(eventSymbol, diagnosticLocation, compilation);
            if (validationDiagnostic != null)
            {
                var accessibility = requiresInternal ? "internal" : "public";
                return new EventCommandInfo(namespaceName, className, accessibility, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, useDispatcher, parameterPath, ImmutableArray<ParameterPathSegment>.Empty, validationDiagnostic);
            }

            var invokeMethod = (eventSymbol.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
            if (invokeMethod == null)
            {
                var accessibility = requiresInternal ? "internal" : "public";
                return new EventCommandInfo(namespaceName, className, accessibility, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, useDispatcher, parameterPath, ImmutableArray<ParameterPathSegment>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateDiagnostic, diagnosticLocation ?? Location.None, eventName, eventHandlerType));
            }

            if (!invokeMethod.ReturnsVoid)
            {
                var accessibility = requiresInternal ? "internal" : "public";
                return new EventCommandInfo(namespaceName, className, accessibility, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, useDispatcher, parameterPath, ImmutableArray<ParameterPathSegment>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateReturnTypeDiagnostic, diagnosticLocation ?? Location.None, eventName, eventHandlerType));
            }

            var refParam = invokeMethod.Parameters.FirstOrDefault(p => p.RefKind != RefKind.None);
            if (refParam != null)
            {
                var accessibility = requiresInternal ? "internal" : "public";
                var modifier = FormatRefKindKeyword(refParam.RefKind);
                return new EventCommandInfo(
                    namespaceName,
                    className,
                    accessibility,
                    targetTypeName,
                    eventName,
                    eventHandlerType,
                    ImmutableArray<TriggerParameter>.Empty,
                    useDispatcher,
                    parameterPath,
                    ImmutableArray<ParameterPathSegment>.Empty,
                    Diagnostic.Create(EventCommandParameterModifierNotSupportedDiagnostic, diagnosticLocation ?? Location.None, eventName, eventHandlerType, refParam.Name, modifier));
            }

            var parameterPathSegments = ImmutableArray<ParameterPathSegment>.Empty;
            if (parameterPath != null)
            {
                var parameterPathDiagnostic = ValidateParameterPath(invokeMethod, eventName, parameterPath, diagnosticLocation, compilation, ref requiresInternal, out parameterPathSegments);
                if (parameterPathDiagnostic != null)
                {
                    var accessibility = requiresInternal ? "internal" : "public";
                    return new EventCommandInfo(namespaceName, className, accessibility, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, useDispatcher, parameterPath, ImmutableArray<ParameterPathSegment>.Empty, parameterPathDiagnostic);
                }
            }

            var parameters = invokeMethod.Parameters.Select(p =>
            {
                var name = EscapeIdentifier(p.Name);
                var typeName = ToDisplayStringWithNullable(p.Type);
                if (ContainsInternalType(p.Type))
                {
                    requiresInternal = true;
                }
                return new TriggerParameter(name, typeName, p.RefKind);
            }).ToImmutableArray();

            var finalAccessibility = requiresInternal ? "internal" : "public";
            return new EventCommandInfo(namespaceName, className, finalAccessibility, targetTypeName, eventName, eventHandlerType, parameters, useDispatcher, parameterPath, parameterPathSegments);
        }

        private static IEnumerable<EventCommandInfo> EnsureUniqueEventCommands(IEnumerable<EventCommandInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => (info.Namespace, info.ClassName)))
            {
                var distinct = group
                    .GroupBy(info => (info.TargetTypeName, info.EventName, info.UseDispatcher, info.DefaultParameterPath ?? string.Empty))
                    .Select(g => g.FirstOrDefault(info => info.Diagnostic is null) ?? g.First())
                    .ToList();

                if (distinct.Count == 1)
                {
                    yield return distinct[0];
                    continue;
                }

                foreach (var info in distinct)
                {
                    yield return info with { ClassName = MakeUniqueName(info.ClassName, info.TargetTypeName + "|" + (info.DefaultParameterPath ?? string.Empty) + "|" + info.UseDispatcher) };
                }
            }
        }

        private static string? GetParameterPath(AttributeData attributeData, SemanticModel semanticModel)
        {
            foreach (var namedArgument in attributeData.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, "ParameterPath", System.StringComparison.Ordinal) && namedArgument.Value.Value is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return s;
                }
            }

            if (attributeData.ApplicationSyntaxReference?.GetSyntax() is AttributeSyntax syntax)
            {
                return GetParameterPath(syntax, semanticModel);
            }

            return null;
        }

        private static string? GetParameterPath(AttributeSyntax attributeSyntax, SemanticModel semanticModel)
        {
            if (attributeSyntax.ArgumentList == null)
            {
                return null;
            }

            foreach (var argument in attributeSyntax.ArgumentList.Arguments)
            {
                if (argument.NameEquals?.Name.Identifier.Text == "ParameterPath")
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

        private Diagnostic? ValidateParameterPath(IMethodSymbol invokeMethod, string eventName, string parameterPath, Location? diagnosticLocation, Compilation? compilation, ref bool requiresInternal, out ImmutableArray<ParameterPathSegment> segments)
        {
            var builder = ImmutableArray.CreateBuilder<ParameterPathSegment>();
            var parts = parameterPath.Split('.');
            var currentType = invokeMethod.Parameters.LastOrDefault()?.Type;
            foreach (var part in parts)
            {
                if (currentType is null)
                {
                    segments = ImmutableArray<ParameterPathSegment>.Empty;
                    return Diagnostic.Create(EventCommandInvalidParameterPathDiagnostic, diagnosticLocation ?? Location.None, parameterPath, eventName);
                }

                var property = currentType.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => string.Equals(p.Name, part, System.StringComparison.Ordinal));
                if (property?.GetMethod is null)
                {
                    segments = ImmutableArray<ParameterPathSegment>.Empty;
                    return Diagnostic.Create(EventCommandInvalidParameterPathDiagnostic, diagnosticLocation ?? Location.None, parameterPath, eventName);
                }

                if (!IsAccessibleToGenerator(property, compilation) || !IsAccessibleToGenerator(property.GetMethod, compilation))
                {
                    var memberName = $"{currentType.ToDisplayString()}.{property.Name}";
                    segments = ImmutableArray<ParameterPathSegment>.Empty;
                    return Diagnostic.Create(EventCommandParameterPathNotAccessibleDiagnostic, diagnosticLocation ?? Location.None, parameterPath, eventName, memberName);
                }

                if (ContainsInternalType(property.Type))
                {
                    requiresInternal = true;
                }

                var isNullableValueType = property.Type.IsValueType &&
                                          property.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
                var needsNullCheck = !property.Type.IsValueType || isNullableValueType;
                builder.Add(new ParameterPathSegment(property.Name, isNullableValueType, needsNullCheck));
                currentType = property.Type;
            }

            segments = builder.ToImmutable();
            return null;
        }
    }
}
