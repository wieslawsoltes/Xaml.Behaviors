using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
                    predicate: (node, _) => node is MethodDeclarationSyntax || node is CompilationUnitSyntax,
                    transform: (ctx, _) => GetActionToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(actions, ExecuteGenerateAction);

            // GenerateTypedTrigger
            var triggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedTriggerAttributeName,
                    predicate: (node, _) => node is EventFieldDeclarationSyntax || node is EventDeclarationSyntax || node is CompilationUnitSyntax,
                    transform: (ctx, _) => GetTriggerToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(triggers, ExecuteGenerateTrigger);

            // GenerateTypedChangePropertyAction
            var propertyActions = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedChangePropertyActionAttributeName,
                    predicate: (node, _) => node is PropertyDeclarationSyntax || node is CompilationUnitSyntax,
                    transform: (ctx, _) => GetChangePropertyActionToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(propertyActions, ExecuteGenerateChangePropertyAction);

            // GenerateTypedDataTrigger
            var dataTriggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateTypedDataTriggerAttributeName,
                    predicate: (node, _) => node is CompilationUnitSyntax,
                    transform: (ctx, _) => GetDataTriggerToGenerate(ctx))
                .SelectMany((x, _) => x);

            context.RegisterSourceOutput(dataTriggers, ExecuteGenerateDataTrigger);

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
        private record ActionInfo(string? Namespace, string ClassName, string TargetTypeName, string MethodName, ImmutableArray<ActionParameter> Parameters, bool IsAwaitable, bool IsValueTask);

        private ImmutableArray<ActionInfo> GetActionToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<ActionInfo>();
            var symbol = context.TargetSymbol;

            if (symbol is IMethodSymbol methodSymbol)
            {
                // Attribute on Method
                var targetType = methodSymbol.ContainingType;
                if (targetType != null)
                {
                    var namespaceName = targetType.ContainingNamespace.ToDisplayString();
                    var className = $"{methodSymbol.Name}Action";
                    var targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var methodName = methodSymbol.Name;
                    
                    var parameters = methodSymbol.Parameters.Select(p => new ActionParameter(p.Name, p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))).ToImmutableArray();

                    var returnType = methodSymbol.ReturnType;
                    bool isAwaitable = IsAwaitableType(returnType);
                    bool isValueTask = IsValueTaskType(returnType);

                    results.Add(new ActionInfo(namespaceName, className, targetTypeName, methodName, parameters, isAwaitable, isValueTask));
                }
            }
            else if (context.TargetSymbol is IAssemblySymbol)
            {
                // Attribute on Assembly
                // [GenerateTypedAction(typeof(Type), "MethodName")]
                foreach (var attributeData in context.Attributes)
                {
                    if (attributeData.AttributeClass?.ToDisplayString() != GenerateTypedActionAttributeName) continue;
                    if (attributeData.ConstructorArguments.Length != 2) continue;

                    var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
                    var methodName = attributeData.ConstructorArguments[1].Value as string;

                    if (targetType == null || string.IsNullOrEmpty(methodName)) continue;

                    // Find the method on the type to check parameters
                    var method = FindMethod(targetType, methodName!);
                    
                    var parameters = method != null 
                        ? method.Parameters.Select(p => new ActionParameter(p.Name, p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))).ToImmutableArray()
                        : ImmutableArray<ActionParameter>.Empty;

                    var returnType = method?.ReturnType;
                    bool isAwaitable = returnType != null && IsAwaitableType(returnType);
                    bool isValueTask = returnType != null && IsValueTaskType(returnType);

                    var ns = targetType.ContainingNamespace.ToDisplayString();
                    var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                    var className = $"{methodName}Action";
                    var targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    results.Add(new ActionInfo(namespaceName, className, targetTypeName, methodName!, parameters, isAwaitable, isValueTask));
                }
            }

            return results.ToImmutable();
        }

        private bool IsAwaitableType(ITypeSymbol typeSymbol)
        {
             var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
             // Check for Task, Task<T>, ValueTask, ValueTask<T>
             return typeName.StartsWith("global::System.Threading.Tasks.Task") || 
                    typeName.StartsWith("global::System.Threading.Tasks.ValueTask");
        }

        private bool IsValueTaskType(ITypeSymbol typeSymbol)
        {
             var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
             return typeName.StartsWith("global::System.Threading.Tasks.ValueTask");
        }

        private void ExecuteGenerateAction(SourceProductionContext spc, ActionInfo info)
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
                 var p1Type = info.Parameters[0].Type;
                 var p2Type = info.Parameters[1].Type;

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
                
                sb.AppendLine("                if (t != null)");
                sb.AppendLine("                {");
                sb.AppendLine("                    if (!t.IsCompleted)");
                sb.AppendLine("                    {");
                sb.AppendLine("                        IsExecuting = true;");
                sb.AppendLine("                        t.ContinueWith(temp => ");
                sb.AppendLine("                        {");
                sb.AppendLine("                            Avalonia.Threading.Dispatcher.UIThread.Post(() => IsExecuting = false);");
                sb.AppendLine("                        });");
                sb.AppendLine("                    }");
                sb.AppendLine("                }");
            }
            else
            {
                sb.AppendLine($"                {invocation};");
            }

            sb.AppendLine("                return true;");
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
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

        private record TriggerInfo(string? Namespace, string ClassName, string TargetTypeName, string EventName, string EventHandlerType, string EventArgsType, string? Error = null);

        private ImmutableArray<TriggerInfo> GetTriggerToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<TriggerInfo>();
            var symbol = context.TargetSymbol;
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
                    var targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var eventName = eventSymbol.Name;
                    var eventHandlerType = eventSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    
                    // Try to determine EventArgs type from the delegate Invoke method
                    var invokeMethod = (eventSymbol.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
                    var eventArgsType = "System.EventArgs";
                    if (invokeMethod != null && invokeMethod.Parameters.Length == 2)
                    {
                        eventArgsType = invokeMethod.Parameters[1].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    }

                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName, eventHandlerType, eventArgsType));
                }
            }
            else if (context.TargetSymbol is IAssemblySymbol)
            {
                foreach (var attributeData in context.Attributes)
                {
                    if (attributeData.AttributeClass?.ToDisplayString() != GenerateTypedTriggerAttributeName) continue;
                    if (attributeData.ConstructorArguments.Length != 2) continue;

                    var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
                    var eventName = attributeData.ConstructorArguments[1].Value as string;

                    if (targetType == null || string.IsNullOrEmpty(eventName)) 
                    {
                        results.Add(new TriggerInfo("", "", "", "", "", "", "TargetType or EventName null"));
                        continue;
                    }

                    var evt = FindEvent(targetType, eventName!);
                    if (evt == null) 
                    {
                        results.Add(new TriggerInfo("", "", "", eventName!, "", "", $"Event {eventName} not found on {targetType.Name}"));
                        continue;
                    }

                    var ns = targetType.ContainingNamespace.ToDisplayString();
                    var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                    var className = $"{eventName}Trigger";
                    var targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var eventHandlerType = evt.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    
                    var invokeMethod = (evt.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
                    var eventArgsType = "System.EventArgs";
                    if (invokeMethod != null && invokeMethod.Parameters.Length == 2)
                    {
                        eventArgsType = invokeMethod.Parameters[1].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    }

                    results.Add(new TriggerInfo(namespaceName, className, targetTypeName, eventName!, eventHandlerType, eventArgsType));
                }
            }

            return results.ToImmutable();
        }

        private void ExecuteGenerateTrigger(SourceProductionContext spc, TriggerInfo info)
        {
            if (info.Error != null)
            {
                spc.AddSource($"Error_{Guid.NewGuid()}.g.cs", SourceText.From($"/* Error: {info.Error} */", Encoding.UTF8));
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
            sb.AppendLine($"        private void OnEvent(object? sender, {info.EventArgsType} e)");
            sb.AppendLine("        {");
            sb.AppendLine("            Interaction.ExecuteActions(AssociatedObject, this.Actions, e);");
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
            sb.AppendLine($"            public void OnEvent(object? sender, {info.EventArgsType} e)");
            sb.AppendLine("            {");
            sb.AppendLine("                if (_trigger.TryGetTarget(out var trigger))");
            sb.AppendLine("                {");
            sb.AppendLine("                    trigger.OnEvent(sender, e);");
            sb.AppendLine("                }");
            sb.AppendLine("                else");
            sb.AppendLine("                {");
            sb.AppendLine($"                    if (sender is {info.TargetTypeName} typedSource)");
            sb.AppendLine("                    {");
            sb.AppendLine($"                        typedSource.{info.EventName} -= OnEvent;");
            sb.AppendLine("                    }");
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

        private record ChangePropertyInfo(string? Namespace, string ClassName, string TargetTypeName, string PropertyName, string PropertyType, string? Error = null);

        private ImmutableArray<ChangePropertyInfo> GetChangePropertyActionToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<ChangePropertyInfo>();
            var symbol = context.TargetSymbol;
            
            if (symbol is IPropertySymbol propertySymbol)
            {
                var targetType = propertySymbol.ContainingType;
                if (targetType != null)
                {
                    var ns = targetType.ContainingNamespace.ToDisplayString();
                    var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                    var className = $"Set{propertySymbol.Name}Action";
                    var targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var propertyName = propertySymbol.Name;
                    var propertyType = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    results.Add(new ChangePropertyInfo(namespaceName, className, targetTypeName, propertyName, propertyType));
                }
            }
            else if (context.TargetSymbol is IAssemblySymbol)
            {
                foreach (var attributeData in context.Attributes)
                {
                    if (attributeData.AttributeClass?.ToDisplayString() != GenerateTypedChangePropertyActionAttributeName) continue;
                    if (attributeData.ConstructorArguments.Length != 2) continue;

                    var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
                    var propertyName = attributeData.ConstructorArguments[1].Value as string;

                    if (targetType == null || string.IsNullOrEmpty(propertyName)) 
                    {
                        results.Add(new ChangePropertyInfo("", "", "", "", "", "TargetType or PropertyName null"));
                        continue;
                    }

                    // Re-resolve type to ensure we have the best symbol?
                    var compilation = context.SemanticModel.Compilation;
                    var typeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    if (typeName.StartsWith("global::")) typeName = typeName.Substring(8);
                    
                    var resolvedType = compilation.GetTypeByMetadataName(typeName);
                    if (resolvedType != null) targetType = resolvedType;

                    var propertyTypeSymbol = FindPropertyType(targetType, propertyName!);
                    if (propertyTypeSymbol == null) 
                    {
                        results.Add(new ChangePropertyInfo("", "", "", propertyName!, "", $"Property {propertyName} not found on {targetType.Name}"));
                        continue;
                    }

                    var propertyType = propertyTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    var ns = targetType.ContainingNamespace.ToDisplayString();
                    var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                    var className = $"Set{propertyName}Action";
                    var targetTypeName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    results.Add(new ChangePropertyInfo(namespaceName, className, targetTypeName, propertyName!, propertyType));
                }
            }
            return results.ToImmutable();
        }

        private void ExecuteGenerateChangePropertyAction(SourceProductionContext spc, ChangePropertyInfo info)
        {
            if (info.Error != null)
            {
                spc.AddSource($"Error_ChangeProperty_{Guid.NewGuid()}.g.cs", SourceText.From($"/* Error: {info.Error} */", Encoding.UTF8));
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

        private ImmutableArray<DataTriggerInfo> GetDataTriggerToGenerate(GeneratorAttributeSyntaxContext context)
        {
            var results = ImmutableArray.CreateBuilder<DataTriggerInfo>();
            if (context.TargetSymbol is IAssemblySymbol)
            {
                foreach (var attributeData in context.Attributes)
                {
                    if (attributeData.AttributeClass?.ToDisplayString() != GenerateTypedDataTriggerAttributeName) continue;
                    if (attributeData.ConstructorArguments.Length != 1) continue;

                    var type = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
                    if (type == null) continue;

                    var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var simpleName = type.Name;
                    
                    var namespaceName = "Xaml.Behaviors.Generated";
                    var className = $"{simpleName}DataTrigger";

                    results.Add(new DataTriggerInfo(namespaceName, className, typeName));
                }
            }
            return results.ToImmutable();
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
                    var typeName = member.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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
                    var typeName = member.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    commandProp = new TriggerPropertyInfo(propertyName, typeName, fieldName);
                }
                if (member.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ActionParameterAttributeName))
                {
                    var fieldName = member.Name;
                    var propertyName = fieldName.TrimStart('_');
                    if (propertyName.Length > 0) propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                    var typeName = member.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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

        private static IMethodSymbol? FindMethod(INamedTypeSymbol? type, string name)
        {
            while (type != null)
            {
                var methods = type.GetMembers(name).OfType<IMethodSymbol>().ToList();
                if (!methods.Any())
                {
                    methods = type.GetMembers().OfType<IMethodSymbol>().Where(m => m.Name == name).ToList();
                }

                var method = methods.FirstOrDefault(m => m.Parameters.Length == 2) 
                             ?? methods.FirstOrDefault(m => m.Parameters.Length == 0)
                             ?? methods.FirstOrDefault();
                
                if (method != null) return method;
                type = type.BaseType;
            }
            return null;
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
    }
}
