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
        private record TriggerParameter(string Name, string Type, RefKind RefKind);

        private record TriggerInfo(
            string? Namespace,
            string ClassName,
            string TargetTypeName,
            string EventName,
            string EventHandlerType,
            ImmutableArray<TriggerParameter> Parameters,
            Diagnostic? Diagnostic = null);

        private void RegisterTriggerGeneration(IncrementalGeneratorInitializationContext context)
        {
            var triggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedTriggerAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetTriggerToGenerate(ctx))
                .SelectMany((x, _) => x);

            var assemblyTriggers = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node, "GenerateTypedTrigger"),
                    transform: (ctx, _) => GetAssemblyTriggerFromAttributeSyntax(ctx))
                .Where(info => info is not null)
                .Select((info, _) => info!);

            var uniqueTriggers = triggers
                .Collect()
                .Combine(assemblyTriggers.Collect())
                .SelectMany((data, _) => EnsureUniqueTriggers(data.Left.Concat(data.Right)));

            context.RegisterSourceOutput(uniqueTriggers, ExecuteGenerateTrigger);
        }

        private ImmutableArray<TriggerInfo> GetTriggerToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<TriggerInfo>();
            var symbol = context.TargetSymbol;
            if (symbol is IAssemblySymbol)
            {
                return results.ToImmutable();
            }

            var eventSymbol = symbol as IEventSymbol;
            if (eventSymbol == null && symbol is IFieldSymbol fieldSymbol)
            {
                if (fieldSymbol.ContainingType != null)
                {
                    eventSymbol = FindEvent(fieldSymbol.ContainingType, fieldSymbol.Name);
                }
            }

            if (eventSymbol != null)
            {
                var targetType = eventSymbol.ContainingType;
                if (targetType != null)
                {
                    var ns = targetType.ContainingNamespace.ToDisplayString();
                    var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                    var className = $"{eventSymbol.Name}Trigger";
                    var targetTypeName = ToDisplayStringWithNullable(targetType);
                    var eventName = eventSymbol.Name;
                    var eventHandlerType = ToDisplayStringWithNullable(eventSymbol.Type);
                    var diagnosticLocation = context.TargetNode?.GetLocation() ?? Location.None;

                    var validationDiagnostic = ValidateEventSymbol(eventSymbol, diagnosticLocation);
                    if (validationDiagnostic != null)
                    {
                        results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, validationDiagnostic));
                        return results.ToImmutable();
                    }

                    var invokeMethod = (eventSymbol.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
                    if (invokeMethod == null)
                    {
                        results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateDiagnostic, diagnosticLocation, eventName, eventHandlerType)));
                        return results.ToImmutable();
                    }

                    if (!invokeMethod.ReturnsVoid)
                    {
                        results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateReturnTypeDiagnostic, diagnosticLocation, eventName, eventHandlerType)));
                        return results.ToImmutable();
                    }

                    var parameters = invokeMethod.Parameters.Select(p =>
                    {
                        var name = EscapeIdentifier(p.Name);
                        var typeName = ToDisplayStringWithNullable(p.Type);
                        return new TriggerParameter(name, typeName, p.RefKind);
                    }).ToImmutableArray();

                    if (parameters.Any(p => p.RefKind == RefKind.Out))
                    {
                        results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, parameters, Diagnostic.Create(TriggerUnsupportedDelegateOutParameterDiagnostic, context.TargetNode?.GetLocation() ?? Location.None, eventName, eventHandlerType)));
                        return results.ToImmutable();
                    }

                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, parameters));
                }
            }

            return results.ToImmutable();
        }

        private ImmutableArray<TriggerInfo> GetAssemblyTriggers(Compilation compilation)
        {
            var results = ImmutableArray.CreateBuilder<TriggerInfo>();

            foreach (var attributeData in compilation.Assembly.GetAttributes())
            {
                if (!IsAttribute(attributeData, GenerateTypedTriggerAttributeName)) continue;
                if (attributeData.ConstructorArguments.Length != 2) continue;

                var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
                var eventName = attributeData.ConstructorArguments[1].Value as string;

                if (targetType == null || string.IsNullOrEmpty(eventName))
                {
                    results.Add(new TriggerInfo("", "", "", "", "", ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerEventNotFoundDiagnostic, Location.None, eventName ?? "<unknown>", targetType?.Name ?? "<unknown>")));
                    continue;
                }

                var evt = FindEvent(targetType, eventName!);
                if (evt == null)
                {
                    results.Add(new TriggerInfo("", "", "", eventName!, "", ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerEventNotFoundDiagnostic, Location.None, eventName!, targetType.Name)));
                    continue;
                }

                var ns = targetType.ContainingNamespace.ToDisplayString();
                var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var className = $"{eventName}Trigger";
                var targetTypeName = ToDisplayStringWithNullable(targetType);
                var eventHandlerType = ToDisplayStringWithNullable(evt.Type);
                var diagnosticLocation = Location.None;

                var validationDiagnostic = ValidateEventSymbol(evt, diagnosticLocation);
                if (validationDiagnostic != null)
                {
                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, validationDiagnostic));
                    continue;
                }

                var invokeMethod = (evt.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
                if (invokeMethod == null)
                {
                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateDiagnostic, diagnosticLocation, eventName!, eventHandlerType)));
                    continue;
                }

                if (!invokeMethod.ReturnsVoid)
                {
                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateReturnTypeDiagnostic, diagnosticLocation, eventName!, eventHandlerType)));
                    continue;
                }

                var parameters = invokeMethod.Parameters.Select(p =>
                {
                    var name = EscapeIdentifier(p.Name);
                    var typeName = ToDisplayStringWithNullable(p.Type);
                    return new TriggerParameter(name, typeName, p.RefKind);
                }).ToImmutableArray();

                if (parameters.Any(p => p.RefKind == RefKind.Out))
                {
                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, parameters, Diagnostic.Create(TriggerUnsupportedDelegateOutParameterDiagnostic, diagnosticLocation, eventName!, eventHandlerType)));
                    continue;
                }

                results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, parameters));
            }

            return results.ToImmutable();
        }

        private void ExecuteGenerateTrigger(SourceProductionContext spc, TriggerInfo info)
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
            sb.AppendLine("using Avalonia.Xaml.Interactivity;");
            sb.AppendLine("using Avalonia.Controls;");
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    public partial class {info.ClassName} : Avalonia.Xaml.Interactivity.StyledElementTrigger");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static readonly StyledProperty<object?> SourceObjectProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, object?>(nameof(SourceObject));");
            sb.AppendLine();
            sb.AppendLine("        public object? SourceObject");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(SourceObjectProperty);");
            sb.AppendLine("            set => SetValue(SourceObjectProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private EventProxy? _proxy;");
            sb.AppendLine($"        private object? _resolvedSource;");
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
            sb.AppendLine("            if (change.Property == SourceObjectProperty)");
            sb.AppendLine("            {");
            sb.AppendLine("                UpdateSource();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void UpdateSource()");
            sb.AppendLine("        {");
            sb.AppendLine("            var newSource = SourceObject ?? AssociatedObject;");
            sb.AppendLine("            if (newSource != _resolvedSource)");
            sb.AppendLine("            {");
            sb.AppendLine("                if (_resolvedSource is not null) UnregisterEvent(_resolvedSource);");
            sb.AppendLine("                _resolvedSource = newSource;");
            sb.AppendLine("                if (_resolvedSource is not null) RegisterEvent(_resolvedSource);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void RegisterEvent(object source)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (source is {info.TargetTypeName} typedSource)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _proxy = new EventProxy(this);");
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
            var parameters = FormatParameters(info.Parameters);
            var args = FormatArguments(info.Parameters);
            var eventArgsArgument = info.Parameters.Length == 0 ? "null" : info.Parameters.Last().Name;
            sb.AppendLine($"        private void OnEvent({parameters})");
            sb.AppendLine("        {");
            sb.AppendLine($"            Interaction.ExecuteActions(AssociatedObject, this.Actions, {eventArgsArgument});");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private class EventProxy");
            sb.AppendLine("        {");
            sb.AppendLine($"            private readonly WeakReference<{info.ClassName}> _trigger;");
            sb.AppendLine();
            sb.AppendLine($"            public EventProxy({info.ClassName} trigger)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _trigger = new WeakReference<{info.ClassName}>(trigger);");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            public void OnEvent({parameters})");
            sb.AppendLine("            {");
            sb.AppendLine("                if (_trigger.TryGetTarget(out var trigger))");
            sb.AppendLine("                {");
            sb.AppendLine($"                    trigger.OnEvent({args});");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                {");
            if (info.Parameters.Length > 0)
            {
                var firstParam = info.Parameters[0];
                if (IsObjectType(firstParam.Type))
                {
                    sb.AppendLine($"                    if ({firstParam.Name} is {info.TargetTypeName} typedSource)");
                    sb.AppendLine("                    {");
                    sb.AppendLine($"                        typedSource.{info.EventName} -= OnEvent;");
                    sb.AppendLine("                    }");
                }
                else if (firstParam.Type == info.TargetTypeName)
                {
                    sb.AppendLine($"                    {info.TargetTypeName} typedSource = {firstParam.Name};");
                    sb.AppendLine($"                    typedSource.{info.EventName} -= OnEvent;");
                }
            }
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine("}");
            }

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private static Diagnostic? ValidateEventSymbol(IEventSymbol eventSymbol, Location? diagnosticLocation)
        {
            var location = diagnosticLocation ?? Location.None;

            if (eventSymbol.IsStatic)
            {
                return Diagnostic.Create(StaticMemberNotSupportedDiagnostic, location, eventSymbol.Name);
            }

            var containingTypeDiagnostic = ValidateTypeAccessibility(eventSymbol.ContainingType, location);
            if (containingTypeDiagnostic != null)
            {
                return containingTypeDiagnostic;
            }

            var accessSymbol = (ISymbol?)eventSymbol.AddMethod ?? eventSymbol;
            if (!IsAccessibleToGenerator(accessSymbol))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, eventSymbol.Name, eventSymbol.ContainingType.ToDisplayString());
            }

            if (ContainsTypeParameter(eventSymbol.ContainingType) || ContainsTypeParameter(eventSymbol.Type))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location, eventSymbol.Name);
            }

            return null;
        }

        private TriggerInfo? GetAssemblyTriggerFromAttributeSyntax(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return null;

            if (attributeSyntax.ArgumentList?.Arguments.Count != 2)
                return null;

            if (attributeSyntax.ArgumentList.Arguments[0].Expression is not TypeOfExpressionSyntax typeOfExpression)
                return null;

            if (attributeSyntax.ArgumentList.Arguments[1].Expression is not LiteralExpressionSyntax eventLiteral)
                return null;

            var eventName = eventLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(eventName))
                return null;

            var info = CreateTriggerInfo(targetType, eventName, context.Node.GetLocation());
            return info;
        }

        private TriggerInfo CreateTriggerInfo(INamedTypeSymbol targetType, string eventName, Location? diagnosticLocation = null)
        {
            var evt = FindEvent(targetType, eventName);
            if (evt == null)
            {
                return new TriggerInfo("", "", "", eventName, "", ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerEventNotFoundDiagnostic, diagnosticLocation, eventName, targetType.Name));
            }

            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var className = $"{eventName}Trigger";
            var targetTypeName = ToDisplayStringWithNullable(targetType);
            var eventHandlerType = ToDisplayStringWithNullable(evt.Type);
            var validationDiagnostic = ValidateEventSymbol(evt, diagnosticLocation);
            if (validationDiagnostic != null)
            {
                return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, validationDiagnostic);
            }

            var invokeMethod = (evt.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
            if (invokeMethod == null)
            {
                return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateDiagnostic, diagnosticLocation, eventName, eventHandlerType));
            }

            if (!invokeMethod.ReturnsVoid)
            {
                return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateReturnTypeDiagnostic, diagnosticLocation, eventName, eventHandlerType));
            }

            var parameters = invokeMethod.Parameters.Select(p =>
            {
                var name = EscapeIdentifier(p.Name);
                var typeName = ToDisplayStringWithNullable(p.Type);
                return new TriggerParameter(name, typeName, p.RefKind);
            }).ToImmutableArray();

            if (parameters.Any(p => p.RefKind == RefKind.Out))
            {
                return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, parameters, Diagnostic.Create(TriggerUnsupportedDelegateOutParameterDiagnostic, diagnosticLocation, eventName, eventHandlerType));
            }

            return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, parameters);
        }

        private static IEnumerable<TriggerInfo> EnsureUniqueTriggers(IEnumerable<TriggerInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => (info.Namespace, info.ClassName)))
            {
                var distinct = group
                    .GroupBy(info => (info.TargetTypeName, info.EventName))
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
