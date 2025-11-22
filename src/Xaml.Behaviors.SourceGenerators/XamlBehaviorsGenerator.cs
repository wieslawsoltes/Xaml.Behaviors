using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Xaml.Behaviors.SourceGenerators
{
    [Generator]
    public class XamlBehaviorsGenerator : IIncrementalGenerator
    {
        private static readonly DiagnosticDescriptor TriggerUnsupportedDelegateDiagnostic = new(
            id: "XBG001",
            title: "Unsupported trigger delegate",
            messageFormat: "Event '{0}' uses delegate '{1}' which is not supported for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor TriggerUnsupportedDelegateReturnTypeDiagnostic = new(
            id: "XBG002",
            title: "Unsupported trigger delegate return type",
            messageFormat: "Event '{0}' delegate '{1}' must return void for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor TriggerUnsupportedDelegateOutParameterDiagnostic = new(
            id: "XBG003",
            title: "Unsupported trigger delegate parameter",
            messageFormat: "Event '{0}' delegate '{1}' uses an out parameter which is not supported for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor TriggerEventNotFoundDiagnostic = new(
            id: "XBG004",
            title: "Event not found",
            messageFormat: "Event '{0}' could not be found on type '{1}' for trigger generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ChangePropertyNotFoundDiagnostic = new(
            id: "XBG005",
            title: "Property not found",
            messageFormat: "Property '{0}' could not be found on type '{1}' for change property generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ActionMethodNotFoundDiagnostic = new(
            id: "XBG006",
            title: "Action method not found",
            messageFormat: "Method '{0}' could not be found on type '{1}' for action generation",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor ActionMethodAmbiguousDiagnostic = new(
            id: "XBG007",
            title: "Action method ambiguous",
            messageFormat: "Method '{0}' on type '{1}' has multiple overloads; action generation requires an unambiguous target",
            category: "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        private static readonly SymbolDisplayFormat FullyQualifiedNullableFormat =
            SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
                SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        private const string GenerateTypedActionAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedActionAttribute";
        private const string GenerateTypedTriggerAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedTriggerAttribute";
        private const string GenerateTypedChangePropertyActionAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedChangePropertyActionAttribute";
        private const string GenerateTypedDataTriggerAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedDataTriggerAttribute";
        private const string GenerateTypedMultiDataTriggerAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedMultiDataTriggerAttribute";
        private const string TriggerPropertyAttributeName = "Xaml.Behaviors.SourceGenerators.TriggerPropertyAttribute";
        private const string GenerateTypedInvokeCommandActionAttributeName = "Xaml.Behaviors.SourceGenerators.GenerateTypedInvokeCommandActionAttribute";
        private const string ActionCommandAttributeName = "Xaml.Behaviors.SourceGenerators.ActionCommandAttribute";
        private const string ActionParameterAttributeName = "Xaml.Behaviors.SourceGenerators.ActionParameterAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("GenerateTypedActionAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedActionAttribute : Attribute
                        {
                            public GenerateTypedActionAttribute()
                            {
                            }

                            public GenerateTypedActionAttribute(Type targetType, string methodName)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedTriggerAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Event | AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedTriggerAttribute : Attribute
                        {
                            public GenerateTypedTriggerAttribute()
                            {
                            }

                            public GenerateTypedTriggerAttribute(Type targetType, string eventName)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedChangePropertyActionAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedChangePropertyActionAttribute : Attribute
                        {
                            public GenerateTypedChangePropertyActionAttribute()
                            {
                            }

                            public GenerateTypedChangePropertyActionAttribute(Type targetType, string propertyName)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedDataTriggerAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
                        internal class GenerateTypedDataTriggerAttribute : Attribute
                        {
                            public GenerateTypedDataTriggerAttribute(Type type)
                            {
                            }
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedMultiDataTriggerAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                        internal sealed class GenerateTypedMultiDataTriggerAttribute : Attribute
                        {
                        }

                        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                        internal sealed class TriggerPropertyAttribute : Attribute
                        {
                        }
                    }
                    """, Encoding.UTF8));

                ctx.AddSource("GenerateTypedInvokeCommandActionAttribute.g.cs", SourceText.From("""
                    using System;

                    namespace Xaml.Behaviors.SourceGenerators
                    {
                        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                        internal sealed class GenerateTypedInvokeCommandActionAttribute : Attribute
                        {
                        }

                        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                        internal sealed class ActionCommandAttribute : Attribute
                        {
                        }

                        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
                        internal sealed class ActionParameterAttribute : Attribute
                        {
                        }
                    }
                    """, Encoding.UTF8));
            });

            // GenerateTypedAction
            var actions = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedActionAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetActionToGenerate(ctx))
                .SelectMany((x, _) => x);

            var assemblyActions = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node, "GenerateTypedAction"),
                    transform: (ctx, _) => GetAssemblyActionFromAttributeSyntax(ctx))
                .Where(info => info is not null)
                .Select((info, _) => info!);

            var uniqueActions = actions
                .Collect()
                .Combine(assemblyActions.Collect())
                .SelectMany((data, _) => EnsureUniqueActions(data.Left.Concat(data.Right)));

            context.RegisterSourceOutput(uniqueActions, ExecuteGenerateAction);

            // GenerateTypedTrigger
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

            // GenerateTypedChangePropertyAction
            var propertyActions = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedChangePropertyActionAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetChangePropertyActionToGenerate(ctx))
                .SelectMany((x, _) => x);

            var assemblyPropertyActions = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node, "GenerateTypedChangePropertyAction"),
                    transform: (ctx, _) => GetAssemblyChangePropertyFromAttributeSyntax(ctx))
                .Where(info => info is not null)
                .Select((info, _) => info!);

            var uniquePropertyActions = propertyActions
                .Collect()
                .Combine(assemblyPropertyActions.Collect())
                .SelectMany((data, _) => EnsureUniqueChangePropertyActions(data.Left.Concat(data.Right)));

            context.RegisterSourceOutput(uniquePropertyActions, ExecuteGenerateChangePropertyAction);

            // GenerateTypedDataTrigger
            var dataTriggers = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsDataTriggerAttributeSyntax(node),
                    transform: (ctx, _) => GetDataTriggerFromAttributeSyntax(ctx))
                .Where(info => info is not null)
                .Select((info, _) => info!);

            var uniqueDataTriggers = dataTriggers
                .Collect()
                .SelectMany((infos, _) => EnsureUniqueDataTriggers(infos));

            context.RegisterSourceOutput(uniqueDataTriggers, ExecuteGenerateDataTrigger);

            // GenerateTypedMultiDataTrigger
            var multiDataTriggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedMultiDataTriggerAttributeName,
                    predicate: (node, _) => node is ClassDeclarationSyntax,
                    transform: (ctx, _) => GetMultiDataTriggerToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(multiDataTriggers, ExecuteGenerateMultiDataTrigger);

            // GenerateTypedInvokeCommandAction
            var invokeCommandActions = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedInvokeCommandActionAttributeName,
                    predicate: (node, _) => node is ClassDeclarationSyntax,
                    transform: (ctx, _) => GetInvokeCommandActionToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(invokeCommandActions, ExecuteGenerateInvokeCommandAction);
        }

        // ----------------------------------------------------------------------------------------
        // GenerateTypedAction
        // ----------------------------------------------------------------------------------------

        private record ActionParameter(string Name, string Type);
        private record ActionInfo(string? Namespace, string ClassName, string TargetTypeName, string MethodName, ImmutableArray<ActionParameter> Parameters, bool IsAwaitable, bool IsValueTask, Diagnostic? Diagnostic = null);

        private ImmutableArray<ActionInfo> GetActionToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<ActionInfo>();
            var symbol = context.TargetSymbol;

            if (symbol is IAssemblySymbol)
            {
                return results.ToImmutable();
            }

            if (symbol is IMethodSymbol methodSymbol)
            {
                // Attribute on Method
                var targetType = methodSymbol.ContainingType;
                if (targetType != null)
                {
                    var namespaceName = targetType.ContainingNamespace.ToDisplayString();
                    var className = $"{methodSymbol.Name}Action";
                    var targetTypeName = ToDisplayStringWithNullable(targetType);
                    var methodName = methodSymbol.Name;
                    
                    var parameters = methodSymbol.Parameters.Select(p => new ActionParameter(p.Name, ToDisplayStringWithNullable(p.Type))).ToImmutableArray();

                    var returnType = methodSymbol.ReturnType;
                    bool isAwaitable = IsAwaitableType(returnType);
                    bool isValueTask = IsValueTaskType(returnType);

                    results.Add(new ActionInfo(namespaceName, className, targetTypeName, methodName, parameters, isAwaitable, isValueTask));
                }
            }

            return results.ToImmutable();
        }

        private bool IsAwaitableType(ITypeSymbol typeSymbol)
        {
             var typeName = ToDisplayStringWithNullable(typeSymbol);
             // Check for Task, Task<T>, ValueTask, ValueTask<T>
             return typeName.StartsWith("global::System.Threading.Tasks.Task") || 
                    typeName.StartsWith("global::System.Threading.Tasks.ValueTask");
        }

        private bool IsValueTaskType(ITypeSymbol typeSymbol)
        {
             var typeName = ToDisplayStringWithNullable(typeSymbol);
             return typeName.StartsWith("global::System.Threading.Tasks.ValueTask");
        }

        private ImmutableArray<ActionInfo> GetAssemblyActions(Compilation compilation)
        {
            var results = ImmutableArray.CreateBuilder<ActionInfo>();
            foreach (var attributeData in compilation.Assembly.GetAttributes())
            {
                if (!IsAttribute(attributeData, GenerateTypedActionAttributeName)) continue;
                if (attributeData.ConstructorArguments.Length != 2) continue;

                var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
                var methodName = attributeData.ConstructorArguments[1].Value as string;

                if (targetType == null || string.IsNullOrEmpty(methodName)) continue;

                var (method, diagnostic) = ResolveMethod(targetType, methodName!);
                if (diagnostic != null)
                {
                    results.Add(new ActionInfo("", "", "", methodName!, ImmutableArray<ActionParameter>.Empty, false, false, diagnostic));
                    continue;
                }

                results.Add(CreateActionInfo(targetType, method!));
            }

            return results.ToImmutable();
        }

        private void ExecuteGenerateAction(SourceProductionContext spc, ActionInfo info)
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
            sb.AppendLine($"    public partial class {info.ClassName} : Avalonia.Xaml.Interactivity.StyledElementAction");
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

            // Check if we should use the "Event Handler" style invocation
            // Heuristic: 2 parameters, first is object (sender), second is compatible with EventArgs (or just object for now to be safe)
            bool isEventHandlerStyle = false;
            if (info.Parameters.Length == 2)
            {
                var p1Type = info.Parameters[0].Type;
                // Check for object or System.Object (with or without global::), ignoring nullability
                if (p1Type.Contains("System.Object") || p1Type.Contains("object"))
                {
                    isEventHandlerStyle = true;
                }
            }

            if (!isEventHandlerStyle)
            {
                // Generate properties for parameters
                foreach (var param in info.Parameters)
                {
                    var propName = char.ToUpper(param.Name[0]) + param.Name.Substring(1);
                    // Avoid naming conflict with TargetObject
                    if (propName == "TargetObject") propName = "MethodParameter" + propName;

                    sb.AppendLine($"        public static readonly StyledProperty<{param.Type}> {propName}Property =");
                    sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {param.Type}>(nameof({propName}));");
                    sb.AppendLine();
                    sb.AppendLine($"        public {param.Type} {propName}");
                    sb.AppendLine("        {");
                    sb.AppendLine($"            get => GetValue({propName}Property);");
                    sb.AppendLine($"            set => SetValue({propName}Property, value);");
                    sb.AppendLine("        }");
                    sb.AppendLine();
                }
            }

            if (info.IsAwaitable)
            {
                sb.AppendLine($"        public static readonly StyledProperty<bool> IsExecutingProperty =");
                sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, bool>(nameof(IsExecuting));");
                sb.AppendLine();
                sb.AppendLine("        public bool IsExecuting");
                sb.AppendLine("        {");
                sb.AppendLine("            get => GetValue(IsExecutingProperty);");
                sb.AppendLine("            private set => SetValue(IsExecutingProperty, value);");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("        public override object Execute(object? sender, object? parameter)");
            sb.AppendLine("        {");
            sb.AppendLine("            var target = TargetObject ?? sender;");
            sb.AppendLine($"            if (target is {info.TargetTypeName} typedTarget)");
            sb.AppendLine("            {");
            
            string invocation;
            if (info.Parameters.Length == 0)
            {
                 invocation = $"typedTarget.{info.MethodName}()";
            }
            else if (isEventHandlerStyle)
            {
                 // Event Handler Style: Pass sender and parameter from Execute
                 var p1Type = TrimNullableAnnotation(info.Parameters[0].Type);
                 var p2Type = TrimNullableAnnotation(info.Parameters[1].Type);

                 sb.AppendLine($"                var p1 = sender is {p1Type} s ? s : default;");
                 sb.AppendLine($"                var p2 = parameter is {p2Type} p ? p : default;");
                 invocation = $"typedTarget.{info.MethodName}(p1, p2)";
            }
            else
            {
                 // Bindable Parameters Style: Pass values from properties
                 var args = new List<string>();
                 foreach (var param in info.Parameters)
                 {
                     var propName = char.ToUpper(param.Name[0]) + param.Name.Substring(1);
                     if (propName == "TargetObject") propName = "MethodParameter" + propName;
                 args.Add(propName);
                 }
                 invocation = $"typedTarget.{info.MethodName}({string.Join(", ", args)})";
            }

            if (info.IsAwaitable)
            {
                sb.AppendLine($"                var task = {invocation};");
                if (info.IsValueTask)
                {
                    sb.AppendLine("                var t = task.AsTask();");
                }
                else
                {
                    sb.AppendLine("                var t = task;");
                }
                sb.AppendLine("                return TrackTask(t);");
            }
            else
            {
                sb.AppendLine($"                {invocation};");
                sb.AppendLine("                return true;");
            }

            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            if (info.IsAwaitable)
            {
                sb.AppendLine();
                sb.AppendLine("        private bool TrackTask(System.Threading.Tasks.Task? task)");
                sb.AppendLine("        {");
                sb.AppendLine("            if (task == null)");
                sb.AppendLine("            {");
                sb.AppendLine("                return true;");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            if (task.IsCompleted)");
                sb.AppendLine("            {");
                sb.AppendLine("                ObserveCompletedTask(task);");
                sb.AppendLine("                return true;");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            IsExecuting = true;");
                sb.AppendLine("            _ = ObserveTaskAsync(task);");
                sb.AppendLine("            return true;");
                sb.AppendLine("        }");
                sb.AppendLine();
                sb.AppendLine("        private void ObserveCompletedTask(System.Threading.Tasks.Task task)");
                sb.AppendLine("        {");
                sb.AppendLine("            if (task.IsFaulted)");
                sb.AppendLine("            {");
                sb.AppendLine("                if (task.Exception is not null)");
                sb.AppendLine("                {");
                sb.AppendLine("                    throw task.Exception;");
                sb.AppendLine("                }");
                sb.AppendLine("                throw new System.InvalidOperationException(\"Action task faulted.\");");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            if (task.IsCanceled)");
                sb.AppendLine("            {");
                sb.AppendLine("                throw new System.OperationCanceledException(\"Action task was canceled.\");");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            IsExecuting = false;");
                sb.AppendLine("        }");
                sb.AppendLine();
                sb.AppendLine("        private async System.Threading.Tasks.Task ObserveTaskAsync(System.Threading.Tasks.Task task)");
                sb.AppendLine("        {");
                sb.AppendLine("            try");
                sb.AppendLine("            {");
                sb.AppendLine("                await task.ConfigureAwait(false);");
                sb.AppendLine("            }");
                sb.AppendLine("            catch (System.Exception ex)");
                sb.AppendLine("            {");
                sb.AppendLine("                Avalonia.Threading.Dispatcher.UIThread.Post(() => throw new System.InvalidOperationException(\"Action task faulted.\", ex));");
                sb.AppendLine("            }");
                sb.AppendLine("            finally");
                sb.AppendLine("            {");
                sb.AppendLine("                Avalonia.Threading.Dispatcher.UIThread.Post(() => IsExecuting = false);");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
            }
            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine("}");
            }

            spc.AddSource($"{info.ClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        // ----------------------------------------------------------------------------------------
        // GenerateTypedTrigger
        // ----------------------------------------------------------------------------------------

        private record TriggerParameter(string Name, string Type, RefKind RefKind);
        private record TriggerInfo(string? Namespace, string ClassName, string TargetTypeName, string EventName, string EventHandlerType, ImmutableArray<TriggerParameter> Parameters, Diagnostic? Diagnostic = null);

        private ImmutableArray<TriggerInfo> GetTriggerToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<TriggerInfo>();
            var symbol = context.TargetSymbol;
            if (symbol is IAssemblySymbol)
            {
                return results.ToImmutable();
            }
            var eventSymbol = symbol as IEventSymbol;
            
            // Handle field-like events where the attribute might be attached to the field symbol
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

                    var invokeMethod = (eventSymbol.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
                    if (invokeMethod == null)
                    {
                        results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateDiagnostic, context.TargetNode?.GetLocation() ?? Location.None, eventName, eventHandlerType)));
                        return results.ToImmutable();
                    }

                    if (!invokeMethod.ReturnsVoid)
                    {
                        results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateReturnTypeDiagnostic, context.TargetNode?.GetLocation() ?? Location.None, eventName, eventHandlerType)));
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

                var invokeMethod = (evt.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
                if (invokeMethod == null)
                {
                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateDiagnostic, Location.None, eventName!, eventHandlerType)));
                    continue;
                }

                if (!invokeMethod.ReturnsVoid)
                {
                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateReturnTypeDiagnostic, Location.None, eventName!, eventHandlerType)));
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
                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, parameters, Diagnostic.Create(TriggerUnsupportedDelegateOutParameterDiagnostic, Location.None, eventName!, eventHandlerType)));
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

            spc.AddSource($"{info.ClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        // ----------------------------------------------------------------------------------------
        // GenerateTypedChangePropertyAction
        // ----------------------------------------------------------------------------------------

        private record ChangePropertyInfo(string? Namespace, string ClassName, string TargetTypeName, string PropertyName, string PropertyType, Diagnostic? Diagnostic = null);

        private ImmutableArray<ChangePropertyInfo> GetChangePropertyActionToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<ChangePropertyInfo>();
            var symbol = context.TargetSymbol;
            if (symbol is IAssemblySymbol)
            {
                return results.ToImmutable();
            }
            
            if (symbol is IPropertySymbol propertySymbol)
            {
                    var targetType = propertySymbol.ContainingType;
                    if (targetType != null)
                    {
                        var ns = targetType.ContainingNamespace.ToDisplayString();
                        var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                        var className = $"Set{propertySymbol.Name}Action";
                        var targetTypeName = ToDisplayStringWithNullable(targetType);
                        var propertyName = propertySymbol.Name;
                        var propertyType = ToDisplayStringWithNullable(propertySymbol.Type);

                    results.Add(new ChangePropertyInfo(namespaceName, className, targetTypeName, propertyName, propertyType));
                }
            }
            return results.ToImmutable();
        }

        private void ExecuteGenerateChangePropertyAction(SourceProductionContext spc, ChangePropertyInfo info)
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
            sb.AppendLine($"    public partial class {info.ClassName} : Avalonia.Xaml.Interactivity.StyledElementAction");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static readonly StyledProperty<object?> TargetObjectProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, object?>(nameof(TargetObject));");
            sb.AppendLine();
            sb.AppendLine($"        public static readonly StyledProperty<{info.PropertyType}> ValueProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {info.PropertyType}>(nameof(Value));");
            sb.AppendLine();
            sb.AppendLine("        public object? TargetObject");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(TargetObjectProperty);");
            sb.AppendLine("            set => SetValue(TargetObjectProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {info.PropertyType} Value");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(ValueProperty);");
            sb.AppendLine("            set => SetValue(ValueProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public override object Execute(object? sender, object? parameter)");
            sb.AppendLine("        {");
            sb.AppendLine("            var target = TargetObject ?? sender;");
            sb.AppendLine($"            if (target is {info.TargetTypeName} typedTarget)");
            sb.AppendLine("            {");
            sb.AppendLine($"                typedTarget.{info.PropertyName} = Value;");
            sb.AppendLine("                return true;");
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource($"{info.ClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        // ----------------------------------------------------------------------------------------
        // GenerateTypedDataTrigger
        // ----------------------------------------------------------------------------------------

        private record DataTriggerInfo(string Namespace, string ClassName, string TypeName);

        private static DataTriggerInfo? GetDataTriggerFromAttributeSyntax(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return null;

            if (attributeSyntax.ArgumentList?.Arguments.Count != 1)
                return null;

            if (attributeSyntax.ArgumentList.Arguments[0].Expression is not TypeOfExpressionSyntax typeOfExpression)
                return null;

            var typeInfo = context.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
            const string namespaceName = "Xaml.Behaviors.Generated";

            if (typeInfo != null)
            {
                var className = $"{typeInfo.Name}DataTrigger";
                var typeName = ToDisplayStringWithNullable(typeInfo);
                return new DataTriggerInfo(namespaceName, className, typeName);
            }

            var simpleName = typeOfExpression.Type switch
            {
                IdentifierNameSyntax id => id.Identifier.Text,
                QualifiedNameSyntax q => q.Right.Identifier.Text,
                _ => typeOfExpression.Type.ToString()
            };

            return new DataTriggerInfo(namespaceName, $"{simpleName}DataTrigger", typeOfExpression.Type.ToString());
        }

        private void ExecuteGenerateDataTrigger(SourceProductionContext spc, DataTriggerInfo info)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("#nullable enable");
            sb.AppendLine("using System;");
            sb.AppendLine("using Avalonia;");
            sb.AppendLine("using Avalonia.Xaml.Interactivity;");
            sb.AppendLine("using Avalonia.Controls;");
            sb.AppendLine();
            sb.AppendLine($"namespace {info.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public partial class {info.ClassName} : Avalonia.Xaml.Interactivity.StyledElementTrigger");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static readonly StyledProperty<{info.TypeName}> BindingProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {info.TypeName}>(nameof(Binding));");
            sb.AppendLine();
            sb.AppendLine($"        public static readonly StyledProperty<ComparisonConditionType> ComparisonConditionProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, ComparisonConditionType>(nameof(ComparisonCondition));");
            sb.AppendLine();
            sb.AppendLine($"        public static readonly StyledProperty<{info.TypeName}> ValueProperty =");
            sb.AppendLine($"            AvaloniaProperty.Register<{info.ClassName}, {info.TypeName}>(nameof(Value));");
            sb.AppendLine();
            sb.AppendLine($"        public {info.TypeName} Binding");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(BindingProperty);");
            sb.AppendLine("            set => SetValue(BindingProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public ComparisonConditionType ComparisonCondition");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(ComparisonConditionProperty);");
            sb.AppendLine("            set => SetValue(ComparisonConditionProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {info.TypeName} Value");
            sb.AppendLine("        {");
            sb.AppendLine("            get => GetValue(ValueProperty);");
            sb.AppendLine("            set => SetValue(ValueProperty, value);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnPropertyChanged(change);");
            sb.AppendLine("            if (change.Property == BindingProperty || change.Property == ComparisonConditionProperty || change.Property == ValueProperty)");
            sb.AppendLine("            {");
            sb.AppendLine("                Evaluate();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Evaluate()");
            sb.AppendLine("        {");
            sb.AppendLine("             // Simple comparison logic for the specific type");
            sb.AppendLine("             bool result = false;");
            sb.AppendLine("             var left = Binding;");
            sb.AppendLine("             var right = Value;");
            sb.AppendLine("             switch (ComparisonCondition)");
            sb.AppendLine("             {");
            sb.AppendLine("                 case ComparisonConditionType.Equal: result = System.Collections.Generic.EqualityComparer<" + info.TypeName + ">.Default.Equals(left, right); break;");
            sb.AppendLine("                 case ComparisonConditionType.NotEqual: result = !System.Collections.Generic.EqualityComparer<" + info.TypeName + ">.Default.Equals(left, right); break;");
            
            // For LessThan/GreaterThan, we need IComparable or operators.
            // Since we are generating for specific types, we can check if it implements IComparable.
            // But for simplicity in this generic generator, we might assume IComparable for now or just generate code that might fail if type doesn't support it.
            // A safer bet for "TypedDataTrigger" is usually for primitives (double, int, string).
            // Let's assume IComparable<T> or IComparable.
            
            sb.AppendLine("                 default:");
            sb.AppendLine("                     if (left is IComparable cmp)");
            sb.AppendLine("                     {");
            sb.AppendLine("                         var diff = cmp.CompareTo(right);");
            sb.AppendLine("                         switch (ComparisonCondition)");
            sb.AppendLine("                         {");
            sb.AppendLine("                             case ComparisonConditionType.LessThan: result = diff < 0; break;");
            sb.AppendLine("                             case ComparisonConditionType.LessThanOrEqual: result = diff <= 0; break;");
            sb.AppendLine("                             case ComparisonConditionType.GreaterThan: result = diff > 0; break;");
            sb.AppendLine("                             case ComparisonConditionType.GreaterThanOrEqual: result = diff >= 0; break;");
            sb.AppendLine("                         }");
            sb.AppendLine("                     }");
            sb.AppendLine("                     break;");
            sb.AppendLine("             }");
            sb.AppendLine();
            sb.AppendLine("             if (result)");
            sb.AppendLine("             {");
            sb.AppendLine("                 Interaction.ExecuteActions(AssociatedObject, Actions, null);");
            sb.AppendLine("             }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            spc.AddSource($"{info.ClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        // ----------------------------------------------------------------------------------------
        // GenerateTypedMultiDataTrigger
        // ----------------------------------------------------------------------------------------

        private record TriggerPropertyInfo(string Name, string Type, string FieldName);
        private record MultiDataTriggerInfo(string? Namespace, string ClassName, ImmutableArray<TriggerPropertyInfo> Properties);

        private ImmutableArray<MultiDataTriggerInfo> GetMultiDataTriggerToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<MultiDataTriggerInfo>();
            var symbol = context.TargetSymbol as INamedTypeSymbol;
            if (symbol == null) return results.ToImmutable();

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
                results.Add(new MultiDataTriggerInfo(namespaceName, className, properties.ToImmutable()));
            }

            return results.ToImmutable();
        }

        private void ExecuteGenerateMultiDataTrigger(SourceProductionContext spc, MultiDataTriggerInfo info)
        {
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
            sb.AppendLine($"    public partial class {info.ClassName}");
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

            spc.AddSource($"{info.ClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        // ----------------------------------------------------------------------------------------
        // GenerateTypedInvokeCommandAction
        // ----------------------------------------------------------------------------------------

        private record InvokeCommandActionInfo(string? Namespace, string ClassName, TriggerPropertyInfo? Command, TriggerPropertyInfo? Parameter);

        private ImmutableArray<InvokeCommandActionInfo> GetInvokeCommandActionToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<InvokeCommandActionInfo>();
            var symbol = context.TargetSymbol as INamedTypeSymbol;
            if (symbol == null) return results.ToImmutable();

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
                results.Add(new InvokeCommandActionInfo(namespaceName, className, commandProp, parameterProp));
            }

            return results.ToImmutable();
        }

        private void ExecuteGenerateInvokeCommandAction(SourceProductionContext spc, InvokeCommandActionInfo info)
        {
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
            sb.AppendLine($"    public partial class {info.ClassName}");
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
            
            // Sync fields
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

            spc.AddSource($"{info.ClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private (IMethodSymbol? Method, Diagnostic? Diagnostic) ResolveMethod(INamedTypeSymbol targetType, string name)
        {
            var current = targetType;
            while (current != null)
            {
                var methods = current
                    .GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(m => m.MethodKind == MethodKind.Ordinary && m.Name == name)
                    .ToList();

                if (methods.Count == 1)
                {
                    return (methods[0], null);
                }

                if (methods.Count > 1)
                {
                    return (null, Diagnostic.Create(ActionMethodAmbiguousDiagnostic, Location.None, name, targetType.Name));
                }

                current = current.BaseType;
            }

            return (null, Diagnostic.Create(ActionMethodNotFoundDiagnostic, Location.None, name, targetType.Name));
        }

        private static IEventSymbol? FindEvent(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var evt = type.GetMembers(name).OfType<IEventSymbol>().FirstOrDefault();
                if (evt != null) return evt;

                evt = type.GetMembers().OfType<IEventSymbol>().FirstOrDefault(e => e.Name == name);
                if (evt != null) return evt;

                type = type.BaseType;
            }
            return null;
        }

        private static IPropertySymbol? FindProperty(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var prop = type.GetMembers(name).OfType<IPropertySymbol>().FirstOrDefault();
                if (prop != null) return prop;

                prop = type.GetMembers().OfType<IPropertySymbol>().FirstOrDefault(p => p.Name == name);
                if (prop != null) return prop;

                // Fallback: look for get_Name method and get associated property
                var getMethod = type.GetMembers($"get_{name}").OfType<IMethodSymbol>().FirstOrDefault();
                if (getMethod != null && getMethod.AssociatedSymbol is IPropertySymbol associatedProp)
                {
                    return associatedProp;
                }

                type = type.BaseType;
            }
            return null;
        }

        private static ITypeSymbol? FindPropertyType(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var prop = type.GetMembers(name).OfType<IPropertySymbol>().FirstOrDefault();
                if (prop != null) return prop.Type;

                var getMethod = type.GetMembers($"get_{name}").OfType<IMethodSymbol>().FirstOrDefault();
                if (getMethod != null) return getMethod.ReturnType;

                type = type.BaseType;
            }
            return null;
        }

        private ImmutableArray<ChangePropertyInfo> GetAssemblyChangePropertyActions(Compilation compilation)
        {
            var results = ImmutableArray.CreateBuilder<ChangePropertyInfo>();

            foreach (var attributeData in compilation.Assembly.GetAttributes())
            {
                if (!IsAttribute(attributeData, GenerateTypedChangePropertyActionAttributeName)) continue;
                if (attributeData.ConstructorArguments.Length != 2) continue;

                var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
                var propertyName = attributeData.ConstructorArguments[1].Value as string;

                if (targetType == null || string.IsNullOrEmpty(propertyName))
                {
                    var diagnostic = Diagnostic.Create(ChangePropertyNotFoundDiagnostic, Location.None, propertyName ?? "<unknown>", targetType?.Name ?? "<unknown>");
                    results.Add(new ChangePropertyInfo("", "", "", "", "", diagnostic));
                    continue;
                }

                var propertyTypeSymbol = FindPropertyType(targetType, propertyName!);
                if (propertyTypeSymbol == null)
                {
                    var diagnostic = Diagnostic.Create(ChangePropertyNotFoundDiagnostic, Location.None, propertyName!, targetType.Name);
                    results.Add(new ChangePropertyInfo("", "", "", propertyName!, "", diagnostic));
                    continue;
                }

                var propertyType = ToDisplayStringWithNullable(propertyTypeSymbol);

                var ns = targetType.ContainingNamespace.ToDisplayString();
                var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var className = $"Set{propertyName}Action";
                var targetTypeName = ToDisplayStringWithNullable(targetType);

                results.Add(new ChangePropertyInfo(namespaceName, className, targetTypeName, propertyName!, propertyType));
            }

            return results.ToImmutable();
        }

        private static string ToDisplayStringWithNullable(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ToDisplayString(FullyQualifiedNullableFormat);
        }

        private static string EscapeIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "arg";
            }

            var kind = SyntaxFacts.GetKeywordKind(name);
            return kind != SyntaxKind.None ? "@" + name : name;
        }

        private static string FormatParameters(ImmutableArray<TriggerParameter> parameters)
        {
            return string.Join(", ", parameters.Select(FormatParameter));
        }

        private static string FormatParameter(TriggerParameter parameter)
        {
            return $"{FormatRefKind(parameter.RefKind)}{parameter.Type} {parameter.Name}";
        }

        private static string FormatArguments(ImmutableArray<TriggerParameter> parameters)
        {
            return string.Join(", ", parameters.Select(FormatArgument));
        }

        private static string FormatArgument(TriggerParameter parameter)
        {
            return $"{FormatRefKind(parameter.RefKind)}{parameter.Name}";
        }

        private static string FormatRefKind(RefKind refKind)
        {
            return refKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => string.Empty
            };
        }

        private static bool IsObjectType(string typeName)
        {
            return typeName == "object" || typeName == "System.Object" || typeName == "global::System.Object";
        }

        private static string TrimNullableAnnotation(string typeName)
        {
            if (typeName.EndsWith("?", StringComparison.Ordinal) && typeName.Length > 0)
            {
                return typeName.Substring(0, typeName.Length - 1);
            }

            return typeName;
        }

        private static string MakeUniqueName(string baseName, INamedTypeSymbol scope)
        {
            var fullyQualified = scope.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return MakeUniqueName(baseName, fullyQualified);
        }

        private static string MakeUniqueName(string baseName, string scope)
        {
            return $"{baseName}_{ComputeHash(scope)}";
        }

        private static bool IsDataTriggerAttribute(AttributeData attributeData)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass == null)
                return false;

            var displayName = attributeClass.ToDisplayString();
            if (displayName == GenerateTypedDataTriggerAttributeName)
                return true;

            var fullyQualified = attributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (fullyQualified.StartsWith("global::", StringComparison.Ordinal))
            {
                fullyQualified = fullyQualified.Substring("global::".Length);
            }

            if (fullyQualified == GenerateTypedDataTriggerAttributeName)
                return true;

            return attributeClass.Name == "GenerateTypedDataTriggerAttribute";
        }

        private static bool IsAssemblyAttribute(SyntaxNode node, string attributeName)
        {
            if (node is not AttributeSyntax attributeSyntax)
                return false;

            if (!attributeSyntax.Name.ToString().Contains(attributeName, StringComparison.Ordinal))
                return false;

            if (attributeSyntax.Parent is AttributeListSyntax list && list.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true)
                return true;

            return false;
        }

        private ActionInfo? GetAssemblyActionFromAttributeSyntax(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return null;

            if (attributeSyntax.ArgumentList?.Arguments.Count != 2)
                return null;

            if (attributeSyntax.ArgumentList.Arguments[0].Expression is not TypeOfExpressionSyntax typeOfExpression)
                return null;

            if (attributeSyntax.ArgumentList.Arguments[1].Expression is not LiteralExpressionSyntax methodLiteral)
                return null;

            var methodName = methodLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(methodName))
                return null;

            var info = CreateActionInfo(targetType, methodName);
            return info;
        }

        private ActionInfo CreateActionInfo(INamedTypeSymbol targetType, string methodName)
        {
            var (method, diagnostic) = ResolveMethod(targetType, methodName);
            if (diagnostic != null)
            {
                return new ActionInfo("", "", "", methodName, ImmutableArray<ActionParameter>.Empty, false, false, diagnostic);
            }

            return CreateActionInfo(targetType, method!);
        }

        private ActionInfo CreateActionInfo(INamedTypeSymbol targetType, IMethodSymbol methodSymbol)
        {
            var parameters = methodSymbol.Parameters.Select(p => new ActionParameter(p.Name, ToDisplayStringWithNullable(p.Type))).ToImmutableArray();

            var returnType = methodSymbol.ReturnType;
            bool isAwaitable = IsAwaitableType(returnType);
            bool isValueTask = IsValueTaskType(returnType);

            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var className = $"{methodSymbol.Name}Action";
            var targetTypeName = ToDisplayStringWithNullable(targetType);

            return new ActionInfo(namespaceName, className, targetTypeName, methodSymbol.Name, parameters, isAwaitable, isValueTask);
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

            var info = CreateTriggerInfo(targetType, eventName);
            return info;
        }

        private TriggerInfo CreateTriggerInfo(INamedTypeSymbol targetType, string eventName)
        {
            var evt = FindEvent(targetType, eventName);
            if (evt == null)
            {
                return new TriggerInfo("", "", "", eventName, "", ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerEventNotFoundDiagnostic, Location.None, eventName, targetType.Name));
            }

            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var className = $"{eventName}Trigger";
            var targetTypeName = ToDisplayStringWithNullable(targetType);
            var eventHandlerType = ToDisplayStringWithNullable(evt.Type);

            var invokeMethod = (evt.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
            if (invokeMethod == null)
            {
                return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateDiagnostic, Location.None, eventName, eventHandlerType));
            }

            if (!invokeMethod.ReturnsVoid)
            {
                return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, ImmutableArray<TriggerParameter>.Empty, Diagnostic.Create(TriggerUnsupportedDelegateReturnTypeDiagnostic, Location.None, eventName, eventHandlerType));
            }

            var parameters = invokeMethod.Parameters.Select(p =>
            {
                var name = EscapeIdentifier(p.Name);
                var typeName = ToDisplayStringWithNullable(p.Type);
                return new TriggerParameter(name, typeName, p.RefKind);
            }).ToImmutableArray();

            if (parameters.Any(p => p.RefKind == RefKind.Out))
            {
                return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, parameters, Diagnostic.Create(TriggerUnsupportedDelegateOutParameterDiagnostic, Location.None, eventName, eventHandlerType));
            }

            return new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, parameters);
        }

        private ChangePropertyInfo? GetAssemblyChangePropertyFromAttributeSyntax(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return null;

            if (attributeSyntax.ArgumentList?.Arguments.Count != 2)
                return null;

            if (attributeSyntax.ArgumentList.Arguments[0].Expression is not TypeOfExpressionSyntax typeOfExpression)
                return null;

            if (attributeSyntax.ArgumentList.Arguments[1].Expression is not LiteralExpressionSyntax propertyLiteral)
                return null;

            var propertyName = propertyLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeOfExpression.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(propertyName))
            {
                var diagnostic = Diagnostic.Create(ChangePropertyNotFoundDiagnostic, Location.None, propertyName ?? "<unknown>", targetType?.Name ?? "<unknown>");
                return new ChangePropertyInfo("", "", "", propertyName ?? "<unknown>", "", diagnostic);
            }

            var info = CreateChangePropertyInfo(targetType, propertyName);
            return info;
        }

        private ChangePropertyInfo CreateChangePropertyInfo(INamedTypeSymbol targetType, string propertyName)
        {
            var propertyTypeSymbol = FindPropertyType(targetType, propertyName);
            if (propertyTypeSymbol == null)
            {
                var diagnostic = Diagnostic.Create(ChangePropertyNotFoundDiagnostic, Location.None, propertyName, targetType.Name);
                return new ChangePropertyInfo("", "", "", propertyName, "", diagnostic);
            }

            var propertyType = ToDisplayStringWithNullable(propertyTypeSymbol);

            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var className = $"Set{propertyName}Action";
            var targetTypeName = ToDisplayStringWithNullable(targetType);

            return new ChangePropertyInfo(namespaceName, className, targetTypeName, propertyName, propertyType);
        }

        private static IEnumerable<ActionInfo> EnsureUniqueActions(IEnumerable<ActionInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => info.ClassName))
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

        private static IEnumerable<TriggerInfo> EnsureUniqueTriggers(IEnumerable<TriggerInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => info.ClassName))
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

        private static IEnumerable<ChangePropertyInfo> EnsureUniqueChangePropertyActions(IEnumerable<ChangePropertyInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => info.ClassName))
            {
                var distinct = group
                    .GroupBy(info => (info.TargetTypeName, info.PropertyName))
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

        private static IEnumerable<DataTriggerInfo> EnsureUniqueDataTriggers(IEnumerable<DataTriggerInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => info.ClassName))
            {
                var distinct = group
                    .GroupBy(info => info.TypeName)
                    .Select(g => g.First())
                    .ToList();

                if (distinct.Count == 1)
                {
                    yield return distinct[0];
                    continue;
                }

                foreach (var info in distinct)
                {
                    yield return info with { ClassName = MakeUniqueName(info.ClassName, info.TypeName) };
                }
            }
        }

        private static bool IsAttribute(AttributeData attributeData, string metadataName)
        {
            var attributeClass = attributeData.AttributeClass;
            if (attributeClass == null)
                return false;

            var displayName = attributeClass.ToDisplayString();
            if (displayName == metadataName)
                return true;

            var fullyQualified = attributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (fullyQualified.StartsWith("global::", StringComparison.Ordinal))
            {
                fullyQualified = fullyQualified.Substring("global::".Length);
            }

            if (fullyQualified == metadataName)
                return true;

            return attributeClass.Name == metadataName.Split('.').Last();
        }

        private static bool IsDataTriggerAttributeSyntax(SyntaxNode node)
        {
            if (node is not AttributeSyntax attributeSyntax)
                return false;

            var name = attributeSyntax.Name.ToString();
            return name.Contains("GenerateTypedDataTrigger", StringComparison.Ordinal);
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(8);
            for (int i = 0; i < 4 && i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
