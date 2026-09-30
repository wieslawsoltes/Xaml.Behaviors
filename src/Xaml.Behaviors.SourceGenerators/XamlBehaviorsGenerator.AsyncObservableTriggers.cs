// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xaml.Behaviors.SourceGenerators.Platforms;

namespace Xaml.Behaviors.SourceGenerators
{
    public partial class XamlBehaviorsGenerator
    {
        private record AsyncTriggerInfo(
            string? Namespace,
            string ClassName,
            string Accessibility,
            string TargetTypeName,
            string PropertyName,
            string PropertyTypeName,
            string? ResultTypeName,
            bool UseDispatcher,
            bool FireOnAttach,
            Diagnostic? Diagnostic = null,
            string? PropertyTypeOf = null,
            string? ResultTypeOf = null);

        private record ObservableTriggerInfo(
            string? Namespace,
            string ClassName,
            string Accessibility,
            string TargetTypeName,
            string PropertyName,
            string PropertyTypeName,
            string ValueTypeName,
            bool UseDispatcher,
            bool FireOnAttach,
            Diagnostic? Diagnostic = null,
            string? PropertyTypeOf = null,
            string? ValueTypeOf = null);

        private void RegisterAsyncObservableTriggerGeneration(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<TargetPlatform> platform)
        {
            var asyncTriggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateAsyncTriggerAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetAsyncTriggerFromAttribute(ctx))
                .SelectMany((x, _) => x);

            var asyncAssembly = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node),
                    transform: (ctx, _) => GetAsyncTriggerFromAssemblyAttribute(ctx))
                .SelectMany((x, _) => x);

            var observableTriggers = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateObservableTriggerAttributeName,
                    predicate: static (_, _) => true,
                    transform: (ctx, _) => GetObservableTriggerFromAttribute(ctx))
                .SelectMany((x, _) => x);

            var observableAssembly = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => IsAssemblyAttribute(node),
                    transform: (ctx, _) => GetObservableTriggerFromAssemblyAttribute(ctx))
                .SelectMany((x, _) => x);

            var uniqueAsync = asyncTriggers
                .Collect()
                .Combine(asyncAssembly.Collect())
                .SelectMany((data, _) => EnsureUniqueAsyncTriggers(data.Left.Concat(data.Right)));

            var uniqueObs = observableTriggers
                .Collect()
                .Combine(observableAssembly.Collect())
                .SelectMany((data, _) => EnsureUniqueObservableTriggers(data.Left.Concat(data.Right)));

            context.RegisterSourceOutput(uniqueAsync.Combine(platform), (spc, source) => ExecuteGenerateAsyncTrigger(spc, source.Left, XamlPlatforms.Get(source.Right)));
            context.RegisterSourceOutput(uniqueObs.Combine(platform), (spc, source) => ExecuteGenerateObservableTrigger(spc, source.Left, XamlPlatforms.Get(source.Right)));
        }

        private ImmutableArray<AsyncTriggerInfo> GetAsyncTriggerFromAttribute(GeneratorAttributeSyntaxContext context)
        {
            var builder = ImmutableArray.CreateBuilder<AsyncTriggerInfo>();
            if (context.TargetSymbol is IAssemblySymbol)
                return builder.ToImmutable();

            var classNameCounts = new Dictionary<string, int>(StringComparer.Ordinal);
            if (context.TargetSymbol is IPropertySymbol propertySymbol)
            {
                foreach (var attribute in context.Attributes)
                {
                    var useDispatcher = GetUseDispatcherFlag(attribute, context.SemanticModel, defaultValue: true);
                    var fireOnAttach = GetBoolNamedArgument(attribute, "FireOnAttach", defaultValue: true);
                    var nameOverride = GetNameOverride(attribute, context.SemanticModel);
                    var location = attribute.ApplicationSyntaxReference?.GetSyntax()?.GetLocation() ?? context.TargetNode?.GetLocation();
                    var info = CreateAsyncTriggerInfo(propertySymbol, location, context.SemanticModel.Compilation, includeTypeNamePrefix: false, useDispatcher, fireOnAttach, nameOverride);
                    if (classNameCounts.TryGetValue(info.ClassName, out var count))
                    {
                        classNameCounts[info.ClassName] = count + 1;
                        var scope = $"{info.TargetTypeName}|{info.PropertyName}|{info.UseDispatcher}|{info.FireOnAttach}|{count + 1}";
                        info = info with { ClassName = MakeUniqueName(info.ClassName, scope) };
                    }
                    else
                    {
                        classNameCounts[info.ClassName] = 1;
                    }
                    builder.Add(info);
                }
            }

            return builder.ToImmutable();
        }

        private ImmutableArray<AsyncTriggerInfo> GetAsyncTriggerFromAssemblyAttribute(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return ImmutableArray<AsyncTriggerInfo>.Empty;

            var attributeType = context.SemanticModel.GetTypeInfo(attributeSyntax).Type;
            if (!IsAttributeType(attributeType, GenerateAsyncTriggerAttributeName))
                return ImmutableArray<AsyncTriggerInfo>.Empty;

            if (attributeSyntax.ArgumentList?.Arguments == null)
                return ImmutableArray<AsyncTriggerInfo>.Empty;

            var positional = attributeSyntax.ArgumentList.Arguments
                .Where(a => a.NameEquals is null && a.NameColon is null)
                .ToList();

            if (positional.Count < 2)
                return ImmutableArray<AsyncTriggerInfo>.Empty;

            if (positional[0].Expression is not TypeOfExpressionSyntax typeExpr)
                return ImmutableArray<AsyncTriggerInfo>.Empty;

            if (positional[1].Expression is not LiteralExpressionSyntax propLiteral)
                return ImmutableArray<AsyncTriggerInfo>.Empty;

            var propertyName = propLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeExpr.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(propertyName))
                return ImmutableArray<AsyncTriggerInfo>.Empty;

            var useDispatcher = GetBoolNamedArgument(attributeSyntax, context.SemanticModel, "UseDispatcher", defaultValue: true);
            var fireOnAttach = GetBoolNamedArgument(attributeSyntax, context.SemanticModel, "FireOnAttach", defaultValue: true);
            var nameOverride = GetNameOverride(attributeSyntax, context.SemanticModel);

            return CreateAsyncTriggerInfos(targetType, propertyName, context.Node.GetLocation(), includeTypeNamePrefix: true, context.SemanticModel.Compilation, useDispatcher, fireOnAttach, nameOverride);
        }

        private ImmutableArray<ObservableTriggerInfo> GetObservableTriggerFromAttribute(GeneratorAttributeSyntaxContext context)
        {
            var builder = ImmutableArray.CreateBuilder<ObservableTriggerInfo>();
            if (context.TargetSymbol is IAssemblySymbol)
                return builder.ToImmutable();

            var classNameCounts = new Dictionary<string, int>(StringComparer.Ordinal);
            if (context.TargetSymbol is IPropertySymbol propertySymbol)
            {
                foreach (var attribute in context.Attributes)
                {
                    var useDispatcher = GetUseDispatcherFlag(attribute, context.SemanticModel, defaultValue: true);
                    var fireOnAttach = GetBoolNamedArgument(attribute, "FireOnAttach", defaultValue: true);
                    var nameOverride = GetNameOverride(attribute, context.SemanticModel);
                    var location = attribute.ApplicationSyntaxReference?.GetSyntax()?.GetLocation() ?? context.TargetNode?.GetLocation();
                    var info = CreateObservableTriggerInfo(propertySymbol, location, context.SemanticModel.Compilation, includeTypeNamePrefix: false, useDispatcher, fireOnAttach, nameOverride);
                    if (classNameCounts.TryGetValue(info.ClassName, out var count))
                    {
                        classNameCounts[info.ClassName] = count + 1;
                        var scope = $"{info.TargetTypeName}|{info.PropertyName}|{info.UseDispatcher}|{info.FireOnAttach}|{count + 1}";
                        info = info with { ClassName = MakeUniqueName(info.ClassName, scope) };
                    }
                    else
                    {
                        classNameCounts[info.ClassName] = 1;
                    }
                    builder.Add(info);
                }
            }

            return builder.ToImmutable();
        }

        private ImmutableArray<ObservableTriggerInfo> GetObservableTriggerFromAssemblyAttribute(GeneratorSyntaxContext context)
        {
            if (context.Node is not AttributeSyntax attributeSyntax)
                return ImmutableArray<ObservableTriggerInfo>.Empty;

            var attributeType = context.SemanticModel.GetTypeInfo(attributeSyntax).Type;
            if (!IsAttributeType(attributeType, GenerateObservableTriggerAttributeName))
                return ImmutableArray<ObservableTriggerInfo>.Empty;

            if (attributeSyntax.ArgumentList?.Arguments == null)
                return ImmutableArray<ObservableTriggerInfo>.Empty;

            var positional = attributeSyntax.ArgumentList.Arguments
                .Where(a => a.NameEquals is null && a.NameColon is null)
                .ToList();

            if (positional.Count < 2)
                return ImmutableArray<ObservableTriggerInfo>.Empty;

            if (positional[0].Expression is not TypeOfExpressionSyntax typeExpr)
                return ImmutableArray<ObservableTriggerInfo>.Empty;

            if (positional[1].Expression is not LiteralExpressionSyntax propLiteral)
                return ImmutableArray<ObservableTriggerInfo>.Empty;

            var propertyName = propLiteral.Token.ValueText;
            var targetType = context.SemanticModel.GetTypeInfo(typeExpr.Type).Type as INamedTypeSymbol;
            if (targetType == null || string.IsNullOrEmpty(propertyName))
                return ImmutableArray<ObservableTriggerInfo>.Empty;

            var useDispatcher = GetBoolNamedArgument(attributeSyntax, context.SemanticModel, "UseDispatcher", defaultValue: true);
            var fireOnAttach = GetBoolNamedArgument(attributeSyntax, context.SemanticModel, "FireOnAttach", defaultValue: true);
            var nameOverride = GetNameOverride(attributeSyntax, context.SemanticModel);

            return CreateObservableTriggerInfos(targetType, propertyName, context.Node.GetLocation(), includeTypeNamePrefix: true, context.SemanticModel.Compilation, useDispatcher, fireOnAttach, nameOverride);
        }

        private void ExecuteGenerateAsyncTrigger(SourceProductionContext spc, AsyncTriggerInfo info, IXamlPlatform platform)
        {
            if (info.Diagnostic != null)
            {
                spc.ReportDiagnostic(info.Diagnostic);
                return;
            }

            var properties = platform.Properties;
            var sourceObject = new PropertySpec("SourceObject", "object?", "object");
            var taskProperty = new PropertySpec(info.PropertyName, info.PropertyTypeName, info.PropertyTypeOf ?? TrimNullableAnnotation(info.PropertyTypeName));
            var isExecuting = new PropertySpec("IsExecuting", "bool", "bool", PrivateSetter: true);
            var lastError = new PropertySpec("LastError", "Exception?", ExceptionPropertyTypeOf, PrivateSetter: true);
            var lastResult = info.ResultTypeName is null
                ? null
                : new PropertySpec("LastResult", info.ResultTypeName, info.ResultTypeOf ?? TrimNullableAnnotation(info.ResultTypeName), PrivateSetter: true);
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("#nullable enable");
            platform.Usings.Append(sb, GeneratedSourceKind.AsyncTrigger);
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    {info.Accessibility} partial class {info.ClassName} : {platform.Types.StyledElementTrigger}");
            sb.AppendLine("    {");
            foreach (var property in new[] { sourceObject, taskProperty, isExecuting, lastError, lastResult })
            {
                if (property is not null)
                {
                    properties.AppendField(sb, info.ClassName, property);
                    sb.AppendLine();
                }
            }
            foreach (var property in new[] { sourceObject, taskProperty, isExecuting, lastError })
            {
                properties.AppendAccessor(sb, property);
                sb.AppendLine();
            }
            sb.AppendLine("        private int _taskVersion;");
            sb.AppendLine();
            if (lastResult is not null)
            {
                properties.AppendAccessor(sb, lastResult);
                sb.AppendLine();
            }

            sb.AppendLine("        protected override void OnAttached()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnAttached();");
            sb.AppendLine("            if (FireOnAttach)");
            sb.AppendLine("            {");
            sb.AppendLine("                TrackTask(GetCurrentTask());");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnDetaching()");
            sb.AppendLine("        {");
            sb.AppendLine("            Interlocked.Increment(ref _taskVersion);");
            sb.AppendLine("            IsExecuting = false;");
            sb.AppendLine("            LastError = null;");
            if (info.ResultTypeName != null)
            {
                sb.AppendLine("            LastResult = default!;");
            }
            sb.AppendLine("            base.OnDetaching();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        protected override void OnPropertyChanged({properties.ChangedEventArgsType} change)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnPropertyChanged(change);");
            sb.AppendLine($"            if (AssociatedObject is not null && (change.Property == {info.PropertyName}Property || change.Property == SourceObjectProperty))");
            sb.AppendLine("            {");
            sb.AppendLine("                TrackTask(GetCurrentTask());");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            var trimmedTarget = TrimNullableAnnotation(info.TargetTypeName);
            sb.AppendLine("        private object? GetCurrentTask()");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (IsSet({info.PropertyName}Property))");
            sb.AppendLine("            {");
            sb.AppendLine($"                return {info.PropertyName};");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            var source = SourceObject ?? AssociatedObject;");
            sb.AppendLine($"            if (source is {trimmedTarget} typedSource)");
            sb.AppendLine("            {");
            sb.AppendLine($"                return typedSource.{info.PropertyName};");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void TrackTask(object? taskObj)");
            sb.AppendLine("        {");
            sb.AppendLine("            var version = Interlocked.Increment(ref _taskVersion);");
            if (info.ResultTypeName != null)
            {
                sb.AppendLine("            LastResult = default!;");
            }
            sb.AppendLine("            if (taskObj is null)");
            sb.AppendLine("            {");
            sb.AppendLine("                IsExecuting = false;");
            sb.AppendLine("                LastError = null;");
            sb.AppendLine("                return;");
            sb.AppendLine("            }");
            if (info.ResultTypeName != null)
            {
                sb.AppendLine($"            if (taskObj is Task<{info.ResultTypeName}> tt) {{ TrackTyped(tt, version); return; }}");
                sb.AppendLine($"            if (taskObj is System.Threading.Tasks.ValueTask<{info.ResultTypeName}> vtt) {{ TrackTyped(vtt.AsTask(), version); return; }}");
            }
            sb.AppendLine("            if (taskObj is Task t) { TrackVoid(t, version); return; }");
            sb.AppendLine("            if (taskObj is System.Threading.Tasks.ValueTask vt) TrackVoid(vt.AsTask(), version);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void TrackVoid(Task task, int version)");
            sb.AppendLine("        {");
            sb.AppendLine("            IsExecuting = true;");
            sb.AppendLine("            LastError = null;");
            sb.AppendLine("            task.ContinueWith(t =>");
            sb.AppendLine("            {");
            sb.AppendLine("                if (version != Volatile.Read(ref _taskVersion))");
            sb.AppendLine("                {");
            sb.AppendLine("                    return;");
            sb.AppendLine("                }");
            sb.AppendLine("                void Complete()");
            sb.AppendLine("                {");
            sb.AppendLine("                    if (version != Volatile.Read(ref _taskVersion))");
            sb.AppendLine("                    {");
            sb.AppendLine("                        return;");
            sb.AppendLine("                    }");
            sb.AppendLine("                    IsExecuting = false;");
            sb.AppendLine("                    if (t.IsFaulted)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        LastError = t.Exception?.GetBaseException();");
            sb.AppendLine("                    }");
            sb.AppendLine("                    else if (t.Status == TaskStatus.RanToCompletion)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        Interaction.ExecuteActions(AssociatedObject, Actions, null);");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            if (info.UseDispatcher)
            {
                sb.AppendLine($"                {platform.Dispatcher.PostMethod}(Complete);");
            }
            else
            {
                sb.AppendLine("                Complete();");
            }
            sb.AppendLine("            }, System.Threading.CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);");
            sb.AppendLine("        }");
            if (info.ResultTypeName != null)
            {
                sb.AppendLine();
                sb.AppendLine($"        private void TrackTyped(Task<{info.ResultTypeName}> task, int version)");
                sb.AppendLine("        {");
                sb.AppendLine("            IsExecuting = true;");
                sb.AppendLine("            LastError = null;");
                sb.AppendLine("            LastResult = default!;");
                sb.AppendLine("            task.ContinueWith(t =>");
                sb.AppendLine("            {");
                sb.AppendLine("                if (version != Volatile.Read(ref _taskVersion))");
                sb.AppendLine("                {");
                sb.AppendLine("                    return;");
                sb.AppendLine("                }");
                sb.AppendLine("                void Complete()");
                sb.AppendLine("                {");
                sb.AppendLine("                    if (version != Volatile.Read(ref _taskVersion))");
                sb.AppendLine("                    {");
                sb.AppendLine("                        return;");
                sb.AppendLine("                    }");
                sb.AppendLine("                    IsExecuting = false;");
                sb.AppendLine("                    if (t.IsFaulted)");
                sb.AppendLine("                    {");
                sb.AppendLine("                        LastError = t.Exception?.GetBaseException();");
                sb.AppendLine("                    }");
                sb.AppendLine("                    else if (t.Status == TaskStatus.RanToCompletion)");
                sb.AppendLine("                    {");
                sb.AppendLine("                        LastResult = t.Result;");
                sb.AppendLine("                        Interaction.ExecuteActions(AssociatedObject, Actions, t.Result);");
                sb.AppendLine("                    }");
                sb.AppendLine("                }");
                if (info.UseDispatcher)
                {
                    sb.AppendLine($"                {platform.Dispatcher.PostMethod}(Complete);");
                }
                else
                {
                    sb.AppendLine("                Complete();");
                }
                sb.AppendLine("            }, System.Threading.CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);");
                sb.AppendLine("        }");
            }
            sb.AppendLine();
            sb.AppendLine("        private bool FireOnAttach { get; } = " + (info.FireOnAttach ? "true" : "false") + ";");
            platform.Dispatcher.AppendHelpers(sb, info.UseDispatcher ? DispatcherFeatures.Post : DispatcherFeatures.None);
            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine("}");
            }

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private void ExecuteGenerateObservableTrigger(SourceProductionContext spc, ObservableTriggerInfo info, IXamlPlatform platform)
        {
            if (info.Diagnostic != null)
            {
                spc.ReportDiagnostic(info.Diagnostic);
                return;
            }

            var properties = platform.Properties;
            var sourceObject = new PropertySpec("SourceObject", "object?", "object");
            var observableProperty = new PropertySpec(info.PropertyName, info.PropertyTypeName, info.PropertyTypeOf ?? TrimNullableAnnotation(info.PropertyTypeName));
            var lastValue = new PropertySpec("LastValue", info.ValueTypeName, info.ValueTypeOf ?? TrimNullableAnnotation(info.ValueTypeName), PrivateSetter: true);
            var lastError = new PropertySpec("LastError", "Exception?", ExceptionPropertyTypeOf, PrivateSetter: true);
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("#nullable enable");
            platform.Usings.Append(sb, GeneratedSourceKind.ObservableTrigger);
            sb.AppendLine();
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"    {info.Accessibility} partial class {info.ClassName} : {platform.Types.StyledElementTrigger}");
            sb.AppendLine("    {");
            foreach (var property in new[] { sourceObject, observableProperty, lastValue, lastError })
            {
                properties.AppendField(sb, info.ClassName, property);
                sb.AppendLine();
            }
            sb.AppendLine("        private IDisposable? _subscription;");
            sb.AppendLine($"        private IObservable<{info.ValueTypeName}>? _currentObservable;");
            sb.AppendLine();
            foreach (var property in new[] { sourceObject, observableProperty, lastValue, lastError })
            {
                properties.AppendAccessor(sb, property);
                sb.AppendLine();
            }
            sb.AppendLine("        protected override void OnAttached()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnAttached();");
            sb.AppendLine("            if (FireOnAttach)");
            sb.AppendLine("            {");
            sb.AppendLine("                Subscribe();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnDetaching()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnDetaching();");
            sb.AppendLine("            _subscription?.Dispose();");
            sb.AppendLine("            _subscription = null;");
            sb.AppendLine("            _currentObservable = null;");
            sb.AppendLine("            LastError = null;");
            sb.AppendLine("            LastValue = default!;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        protected override void OnPropertyChanged({properties.ChangedEventArgsType} change)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnPropertyChanged(change);");
            sb.AppendLine($"            if (AssociatedObject is not null && (change.Property == {info.PropertyName}Property || change.Property == SourceObjectProperty))");
            sb.AppendLine("            {");
            sb.AppendLine("                Subscribe();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Subscribe()");
            sb.AppendLine("        {");
            sb.AppendLine("            var obs = GetObservable();");
            sb.AppendLine("            if (ReferenceEquals(obs, _currentObservable))");
            sb.AppendLine("            {");
            sb.AppendLine("                return;");
            sb.AppendLine("            }");
            sb.AppendLine("            _subscription?.Dispose();");
            sb.AppendLine("            _subscription = null;");
            sb.AppendLine("            _currentObservable = obs;");
            sb.AppendLine("            LastError = null;");
            sb.AppendLine("            LastValue = default!;");
            sb.AppendLine("            if (obs is null) return;");
            sb.AppendLine($"            _subscription = obs.Subscribe(new SimpleObserver<{info.ValueTypeName}>(");
            sb.AppendLine("                onNext: value =>");
            sb.AppendLine("                {");
            sb.AppendLine($"                    void NextAction() {{ LastValue = value; Interaction.ExecuteActions(AssociatedObject, Actions, value); }}");
            if (info.UseDispatcher)
            {
                sb.AppendLine($"                    {platform.Dispatcher.PostMethod}(NextAction);");
            }
            else
            {
                sb.AppendLine("                    NextAction();");
            }
            sb.AppendLine("                },");
            sb.AppendLine("                onError: ex =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    void ErrAction() { LastError = ex; Interaction.ExecuteActions(AssociatedObject, Actions, ex); }");
            if (info.UseDispatcher)
            {
                sb.AppendLine($"                    {platform.Dispatcher.PostMethod}(ErrAction);");
            }
            else
            {
                sb.AppendLine("                    ErrAction();");
            }
            sb.AppendLine("                },");
            sb.AppendLine("                onCompleted: () =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    void CompleteAction() { Interaction.ExecuteActions(AssociatedObject, Actions, null); }");
            if (info.UseDispatcher)
            {
                sb.AppendLine($"                    {platform.Dispatcher.PostMethod}(CompleteAction);");
            }
            else
            {
                sb.AppendLine("                    CompleteAction();");
            }
            sb.AppendLine("                }));");
            sb.AppendLine();
            sb.AppendLine("            if (FireOnAttach)");
            sb.AppendLine("            {");
            sb.AppendLine("                // No immediate value; relies on observable emission.");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            var trimmedTarget = TrimNullableAnnotation(info.TargetTypeName);
            sb.AppendLine("        private IObservable<" + info.ValueTypeName + ">? GetObservable()");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (IsSet({info.PropertyName}Property))");
            sb.AppendLine("            {");
            sb.AppendLine($"                return {info.PropertyName};");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            var source = SourceObject ?? AssociatedObject;");
            sb.AppendLine($"            if (source is {trimmedTarget} typedSource)");
            sb.AppendLine("            {");
            sb.AppendLine($"                return typedSource.{info.PropertyName};");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private bool FireOnAttach { get; } = " + (info.FireOnAttach ? "true" : "false") + ";");
            sb.AppendLine();
            sb.AppendLine("        private sealed class SimpleObserver<T> : IObserver<T>");
            sb.AppendLine("        {");
            sb.AppendLine("            private readonly global::System.Action<T> _onNext;");
            sb.AppendLine("            private readonly global::System.Action<Exception> _onError;");
            sb.AppendLine("            private readonly global::System.Action _onCompleted;");
            sb.AppendLine();
            sb.AppendLine("            public SimpleObserver(global::System.Action<T> onNext, global::System.Action<Exception> onError, global::System.Action onCompleted)");
            sb.AppendLine("            {");
            sb.AppendLine("                _onNext = onNext;");
            sb.AppendLine("                _onError = onError;");
            sb.AppendLine("                _onCompleted = onCompleted;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            public void OnCompleted() => _onCompleted();");
            sb.AppendLine("            public void OnError(Exception error) => _onError(error);");
            sb.AppendLine("            public void OnNext(T value) => _onNext(value);");
            sb.AppendLine("        }");
            platform.Dispatcher.AppendHelpers(sb, info.UseDispatcher ? DispatcherFeatures.Post : DispatcherFeatures.None);
            sb.AppendLine();
            sb.AppendLine("    }");
            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine("}");
            }

            spc.AddSource(CreateHintName(info.Namespace, info.ClassName), SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        private ImmutableArray<AsyncTriggerInfo> CreateAsyncTriggerInfos(INamedTypeSymbol targetType, string propertyPattern, Location? diagnosticLocation, bool includeTypeNamePrefix, Compilation? compilation, bool useDispatcher, bool fireOnAttach, string? nameOverride)
        {
            var matched = FindMatchingProperties(targetType, propertyPattern);
            var builder = ImmutableArray.CreateBuilder<AsyncTriggerInfo>();
            foreach (var prop in matched)
            {
                var info = CreateAsyncTriggerInfo(prop, diagnosticLocation, compilation, includeTypeNamePrefix, useDispatcher, fireOnAttach, nameOverride);
                builder.Add(info);
            }
            if (builder.Count == 0)
            {
                var diagnostic = Diagnostic.Create(AsyncTriggerPropertyNotFoundDiagnostic, diagnosticLocation ?? Location.None, propertyPattern, targetType.Name);
                var ns = targetType.ContainingNamespace.ToDisplayString();
                var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType) : string.Empty;
                var baseName = nameOverride ?? $"{CreateSafeIdentifier(propertyPattern)}AsyncTrigger";
                var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
                var accessibility = GetAccessibilityKeyword(targetType);
                var targetTypeName = ToDisplayStringWithNullable(targetType);
                return ImmutableArray.Create(new AsyncTriggerInfo(namespaceName, className, accessibility, targetTypeName, propertyPattern, "object?", null, useDispatcher, fireOnAttach, diagnostic));
            }
            return builder.ToImmutable();
        }

        private ImmutableArray<ObservableTriggerInfo> CreateObservableTriggerInfos(INamedTypeSymbol targetType, string propertyPattern, Location? diagnosticLocation, bool includeTypeNamePrefix, Compilation? compilation, bool useDispatcher, bool fireOnAttach, string? nameOverride)
        {
            var matched = FindMatchingProperties(targetType, propertyPattern);
            var builder = ImmutableArray.CreateBuilder<ObservableTriggerInfo>();
            foreach (var prop in matched)
            {
                var info = CreateObservableTriggerInfo(prop, diagnosticLocation, compilation, includeTypeNamePrefix, useDispatcher, fireOnAttach, nameOverride);
                builder.Add(info);
            }
            if (builder.Count == 0)
            {
                var diagnostic = Diagnostic.Create(ObservableTriggerPropertyNotFoundDiagnostic, diagnosticLocation ?? Location.None, propertyPattern, targetType.Name);
                var ns = targetType.ContainingNamespace.ToDisplayString();
                var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
                var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType) : string.Empty;
                var baseName = nameOverride ?? $"{CreateSafeIdentifier(propertyPattern)}ObservableTrigger";
                var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
                var accessibility = GetAccessibilityKeyword(targetType);
                var targetTypeName = ToDisplayStringWithNullable(targetType);
                return ImmutableArray.Create(new ObservableTriggerInfo(namespaceName, className, accessibility, targetTypeName, propertyPattern, "object?", "object?", useDispatcher, fireOnAttach, diagnostic));
            }
            return builder.ToImmutable();
        }

        private AsyncTriggerInfo CreateAsyncTriggerInfo(IPropertySymbol propertySymbol, Location? diagnosticLocation, Compilation? compilation, bool includeTypeNamePrefix, bool useDispatcher, bool fireOnAttach, string? nameOverride)
        {
            var location = diagnosticLocation ?? Location.None;
            var validation = ValidateAsyncProperty(propertySymbol, location, compilation, out var resultType);
            var targetType = propertySymbol.ContainingType;
            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType) : string.Empty;
            var baseName = nameOverride ?? $"{propertySymbol.Name}AsyncTrigger";
            var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
            if (nameOverride is not null && (useDispatcher != true || fireOnAttach != true))
            {
                var scope = $"{targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}|{propertySymbol.Name}|{useDispatcher}|{fireOnAttach}";
                className = MakeUniqueName(className, scope);
            }
            var targetTypeName = ToDisplayStringWithNullable(targetType);
            var propertyTypeName = ToDisplayStringWithNullable(propertySymbol.Type);
            var accessibility = GetAccessibilityKeyword(targetType);

            if (validation != null)
            {
                return new AsyncTriggerInfo(namespaceName, className, accessibility, targetTypeName, propertySymbol.Name, propertyTypeName, resultType, useDispatcher, fireOnAttach, validation);
            }

            var resultTypeOf = resultType is not null && propertySymbol.Type is INamedTypeSymbol { TypeArguments.Length: 1 } taskType
                ? ToTypeOfString(taskType.TypeArguments[0])
                : null;
            return new AsyncTriggerInfo(namespaceName, className, accessibility, targetTypeName, propertySymbol.Name, propertyTypeName, resultType, useDispatcher, fireOnAttach, PropertyTypeOf: ToTypeOfString(propertySymbol.Type), ResultTypeOf: resultTypeOf);
        }

        private ObservableTriggerInfo CreateObservableTriggerInfo(IPropertySymbol propertySymbol, Location? diagnosticLocation, Compilation? compilation, bool includeTypeNamePrefix, bool useDispatcher, bool fireOnAttach, string? nameOverride)
        {
            var location = diagnosticLocation ?? Location.None;
            var validation = ValidateObservableProperty(propertySymbol, location, compilation, out var valueType);
            var targetType = propertySymbol.ContainingType;
            var ns = targetType.ContainingNamespace.ToDisplayString();
            var namespaceName = (targetType.ContainingNamespace.IsGlobalNamespace || ns == "<global namespace>") ? null : ns;
            var typePrefix = includeTypeNamePrefix ? GetTypeNamePrefix(targetType) : string.Empty;
            var baseName = nameOverride ?? $"{propertySymbol.Name}ObservableTrigger";
            var className = string.IsNullOrEmpty(typePrefix) ? baseName : typePrefix + baseName;
            if (nameOverride is not null && (useDispatcher != true || fireOnAttach != true))
            {
                var scope = $"{targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}|{propertySymbol.Name}|{useDispatcher}|{fireOnAttach}";
                className = MakeUniqueName(className, scope);
            }
            var targetTypeName = ToDisplayStringWithNullable(targetType);
            var propertyTypeName = ToDisplayStringWithNullable(propertySymbol.Type);
            var accessibility = GetAccessibilityKeyword(targetType);

            if (validation != null)
            {
                return new ObservableTriggerInfo(namespaceName, className, accessibility, targetTypeName, propertySymbol.Name, propertyTypeName, valueType ?? "object?", useDispatcher, fireOnAttach, validation);
            }

            return new ObservableTriggerInfo(namespaceName, className, accessibility, targetTypeName, propertySymbol.Name, propertyTypeName, valueType ?? "object?", useDispatcher, fireOnAttach, PropertyTypeOf: ToTypeOfString(propertySymbol.Type), ValueTypeOf: GetObservableValueTypeOf(propertySymbol.Type));
        }

        private Diagnostic? ValidateAsyncProperty(IPropertySymbol propertySymbol, Location location, Compilation? compilation, out string? resultType)
        {
            resultType = null;
            var targetType = propertySymbol.ContainingType;
            if (propertySymbol.IsStatic)
            {
                return Diagnostic.Create(StaticMemberNotSupportedDiagnostic, location, propertySymbol.Name);
            }

            if (ContainsTypeParameter(targetType))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location, propertySymbol.Name);
            }

            if (ContainsTypeParameter(propertySymbol.Type))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location, propertySymbol.Name);
            }

            if (!IsAccessibleToGenerator(propertySymbol, compilation) ||
                propertySymbol.GetMethod is not null && !IsAccessibleToGenerator(propertySymbol.GetMethod, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
            }

            if (!IsTaskLike(propertySymbol.Type, out resultType))
            {
                return Diagnostic.Create(AsyncTriggerInvalidPropertyTypeDiagnostic, location, propertySymbol.Name);
            }

            if (!IsAccessibleType(propertySymbol.Type, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
            }

            if (targetType.DeclaredAccessibility == Accessibility.Public && !AccessibilityHelper.IsPubliclyAccessibleType(propertySymbol.Type))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
            }

            return ValidateTypeAccessibility(targetType, location, compilation);
        }

        private Diagnostic? ValidateObservableProperty(IPropertySymbol propertySymbol, Location location, Compilation? compilation, out string? valueType)
        {
            valueType = null;
            var targetType = propertySymbol.ContainingType;
            if (propertySymbol.IsStatic)
            {
                return Diagnostic.Create(StaticMemberNotSupportedDiagnostic, location, propertySymbol.Name);
            }

            if (ContainsTypeParameter(targetType))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location, propertySymbol.Name);
            }

            if (ContainsTypeParameter(propertySymbol.Type))
            {
                return Diagnostic.Create(GenericMemberNotSupportedDiagnostic, location, propertySymbol.Name);
            }

            if (!IsAccessibleToGenerator(propertySymbol, compilation) ||
                propertySymbol.GetMethod is not null && !IsAccessibleToGenerator(propertySymbol.GetMethod, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
            }

            if (!IsObservableType(propertySymbol.Type, out valueType))
            {
                return Diagnostic.Create(ObservableTriggerInvalidPropertyTypeDiagnostic, location, propertySymbol.Name);
            }

            if (!IsAccessibleType(propertySymbol.Type, compilation))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
            }

            if (targetType.DeclaredAccessibility == Accessibility.Public && !AccessibilityHelper.IsPubliclyAccessibleType(propertySymbol.Type))
            {
                return Diagnostic.Create(MemberNotAccessibleDiagnostic, location, propertySymbol.Name, propertySymbol.ContainingType.ToDisplayString());
            }

            return ValidateTypeAccessibility(targetType, location, compilation);
        }

        private static bool IsTaskLike(ITypeSymbol typeSymbol, out string? resultType)
        {
            var display = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            resultType = null;
            if (display.StartsWith("global::System.Threading.Tasks.Task<", System.StringComparison.Ordinal))
            {
                if (typeSymbol is INamedTypeSymbol named && named.TypeArguments.Length == 1)
                {
                    resultType = ToDisplayStringWithNullable(named.TypeArguments[0]);
                }
                return true;
            }
            if (display == "global::System.Threading.Tasks.Task")
            {
                resultType = null;
                return true;
            }
            if (display.StartsWith("global::System.Threading.Tasks.ValueTask<", System.StringComparison.Ordinal))
            {
                if (typeSymbol is INamedTypeSymbol named && named.TypeArguments.Length == 1)
                {
                    resultType = ToDisplayStringWithNullable(named.TypeArguments[0]);
                }
                return true;
            }
            if (display == "global::System.Threading.Tasks.ValueTask")
            {
                resultType = null;
                return true;
            }
            return false;
        }

        private static string? GetObservableValueTypeOf(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is not INamedTypeSymbol named)
            {
                return null;
            }

            if (named.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.IObservable<T>")
            {
                return ToTypeOfString(named.TypeArguments[0]);
            }

            var observableInterface = named.AllInterfaces
                .FirstOrDefault(i => i.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.IObservable<T>");
            return observableInterface is null ? null : ToTypeOfString(observableInterface.TypeArguments[0]);
        }

        private static bool IsObservableType(ITypeSymbol typeSymbol, out string? valueType)
        {
            valueType = null;

            if (typeSymbol is INamedTypeSymbol named)
            {
                if (named.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.IObservable<T>")
                {
                    valueType = ToDisplayStringWithNullable(named.TypeArguments[0]);
                    return true;
                }

                var observableInterface = named.AllInterfaces
                    .OfType<INamedTypeSymbol>()
                    .FirstOrDefault(i => i.ConstructedFrom?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.IObservable<T>");

                if (observableInterface != null)
                {
                    valueType = ToDisplayStringWithNullable(observableInterface.TypeArguments[0]);
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<AsyncTriggerInfo> EnsureUniqueAsyncTriggers(IEnumerable<AsyncTriggerInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => (info.Namespace, info.ClassName)))
            {
                var distinct = group
                    .GroupBy(info => (info.TargetTypeName, info.PropertyName, info.UseDispatcher, info.FireOnAttach))
                    .Select(g => g.FirstOrDefault(info => info.Diagnostic is null) ?? g.First())
                    .ToList();

                if (distinct.Count == 1)
                {
                    yield return distinct[0];
                    continue;
                }

                foreach (var info in distinct)
                {
                    var scope = $"{info.TargetTypeName}|{info.PropertyName}|{info.UseDispatcher}|{info.FireOnAttach}";
                    yield return info with { ClassName = MakeUniqueName(info.ClassName, scope) };
                }
            }
        }

        private static IEnumerable<ObservableTriggerInfo> EnsureUniqueObservableTriggers(IEnumerable<ObservableTriggerInfo> infos)
        {
            foreach (var group in infos.GroupBy(info => (info.Namespace, info.ClassName)))
            {
                var distinct = group
                    .GroupBy(info => (info.TargetTypeName, info.PropertyName, info.UseDispatcher, info.FireOnAttach))
                    .Select(g => g.FirstOrDefault(info => info.Diagnostic is null) ?? g.First())
                    .ToList();

                if (distinct.Count == 1)
                {
                    yield return distinct[0];
                    continue;
                }

                foreach (var info in distinct)
                {
                    var scope = $"{info.TargetTypeName}|{info.PropertyName}|{info.UseDispatcher}|{info.FireOnAttach}";
                    yield return info with { ClassName = MakeUniqueName(info.ClassName, scope) };
                }
            }
        }
    }
}
