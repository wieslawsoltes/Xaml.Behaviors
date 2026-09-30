// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;

namespace Xaml.Behaviors.SourceGenerators.Platforms
{
    /// <summary>
    /// Avalonia platform strategy (<c>Avalonia.Xaml.Interactivity</c>, styled properties, <c>Dispatcher.UIThread</c>).
    /// </summary>
    internal sealed class AvaloniaPlatform : IXamlPlatform
    {
        public static AvaloniaPlatform Instance { get; } = new();

        private AvaloniaPlatform()
        {
        }

        public TargetPlatform Kind => TargetPlatform.Avalonia;

        public IUsingDirectivesEmitter Usings { get; } = new AvaloniaUsingDirectivesEmitter();

        public IInteractivityTypeNames Types { get; } = new AvaloniaInteractivityTypeNames();

        public IPropertySystemEmitter Properties { get; } = new AvaloniaPropertySystemEmitter();

        public IDispatcherEmitter Dispatcher { get; } = new AvaloniaDispatcherEmitter();

        public INameScopeEmitter NameScope { get; } = new AvaloniaNameScopeEmitter();

        public IPropertyObservationEmitter PropertyObservation { get; } = new AvaloniaPropertyObservationEmitter();
    }

    internal sealed class AvaloniaUsingDirectivesEmitter : IUsingDirectivesEmitter
    {
        public void Append(StringBuilder sb, GeneratedSourceKind kind)
        {
            foreach (var ns in GetNamespaces(kind))
            {
                sb.AppendLine($"using {ns};");
            }
        }

        private static string[] GetNamespaces(GeneratedSourceKind kind)
        {
            return kind switch
            {
                GeneratedSourceKind.Action => new[] { "Avalonia", "Avalonia.Xaml.Interactivity", "Avalonia.Controls", "Avalonia.Threading" },
                GeneratedSourceKind.Trigger => new[] { "System", "Avalonia", "Avalonia.Xaml.Interactivity", "Avalonia.Controls" },
                GeneratedSourceKind.ChangePropertyAction => new[] { "Avalonia", "Avalonia.Xaml.Interactivity", "Avalonia.Controls", "Avalonia.Threading", "System.Threading" },
                GeneratedSourceKind.DataTrigger => new[] { "System", "Avalonia", "Avalonia.Xaml.Interactivity", "Avalonia.Controls" },
                GeneratedSourceKind.MultiDataTrigger => new[] { "Avalonia", "Avalonia.Xaml.Interactivity", "Avalonia.Controls" },
                GeneratedSourceKind.InvokeCommandAction => new[] { "Avalonia", "Avalonia.Xaml.Interactivity", "Avalonia.Controls", "Avalonia.Threading", "System.Windows.Input" },
                GeneratedSourceKind.PropertyTrigger => new[] { "System", "System.Collections.Generic", "Avalonia", "Avalonia.Controls", "Avalonia.LogicalTree", "Avalonia.Xaml.Interactivity", "Avalonia.Reactive" },
                GeneratedSourceKind.EventCommand => new[] { "System", "System.Windows.Input", "Avalonia", "Avalonia.Controls", "Avalonia.LogicalTree", "Avalonia.Xaml.Interactivity" },
                GeneratedSourceKind.EventArgsAction => new[] { "System", "Avalonia", "Avalonia.Controls", "Avalonia.Xaml.Interactivity" },
                GeneratedSourceKind.AsyncTrigger => new[] { "System", "System.Threading", "System.Threading.Tasks", "Avalonia", "Avalonia.Controls", "Avalonia.Xaml.Interactivity" },
                GeneratedSourceKind.ObservableTrigger => new[] { "System", "Avalonia", "Avalonia.Controls", "Avalonia.Xaml.Interactivity" },
                _ => new[] { "Avalonia", "Avalonia.Xaml.Interactivity" },
            };
        }
    }

    internal sealed class AvaloniaInteractivityTypeNames : IInteractivityTypeNames
    {
        public string StyledElementAction => "Avalonia.Xaml.Interactivity.StyledElementAction";

        public string StyledElementTrigger => "Avalonia.Xaml.Interactivity.StyledElementTrigger";

        public string ReversibleAction => "Avalonia.Xaml.Interactivity.IReversibleAction";
    }

    internal sealed class AvaloniaPropertySystemEmitter : IPropertySystemEmitter
    {
        public string ChangedEventArgsType => "AvaloniaPropertyChangedEventArgs";

        public void AppendField(StringBuilder sb, string ownerClassName, PropertySpec property)
        {
            var defaultValue = property.DefaultValue is null ? string.Empty : ", " + property.DefaultValue;
            sb.AppendLine($"        public static readonly StyledProperty<{property.Type}> {property.Name}Property =");
            sb.AppendLine($"            AvaloniaProperty.Register<{ownerClassName}, {property.Type}>(nameof({property.Name}){defaultValue});");
        }

        public void AppendAccessor(StringBuilder sb, PropertySpec property)
        {
            sb.AppendLine($"        public {property.Type} {property.Name}");
            sb.AppendLine("        {");
            sb.AppendLine($"            get => GetValue({property.Name}Property);");
            sb.AppendLine($"            {(property.PrivateSetter ? "private " : string.Empty)}set => SetValue({property.Name}Property, value);");
            sb.AppendLine("        }");
        }

        public string NewValue(string changeVariable, string typeName)
        {
            return $"{changeVariable}.GetNewValue<{typeName}>()";
        }
    }

    internal sealed class AvaloniaDispatcherEmitter : IDispatcherEmitter
    {
        public string PostMethod => "Avalonia.Threading.Dispatcher.UIThread.Post";

        public string InvokeMethod => "Avalonia.Threading.Dispatcher.UIThread.Invoke";

        public string CheckAccessExpression => "Avalonia.Threading.Dispatcher.UIThread.CheckAccess()";

        public void AppendHelpers(StringBuilder sb, DispatcherFeatures features)
        {
            // Avalonia exposes a global UI thread dispatcher; no helpers are required.
        }
    }

    internal sealed class AvaloniaNameScopeEmitter : INameScopeEmitter
    {
        public void AppendFindInNameScope(StringBuilder sb, NameScopeLookupNames names)
        {
            sb.AppendLine("        private static AvaloniaObject? FindInNameScope(AvaloniaObject? source, string sourceName)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (source is not ILogical {names.Logical})");
            sb.AppendLine("            {");
            sb.AppendLine("                return null;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            foreach (var {names.Ancestor} in {names.Logical}.GetSelfAndLogicalAncestors())");
            sb.AppendLine("            {");
            sb.AppendLine($"                if ({names.Ancestor} is StyledElement {names.Element})");
            sb.AppendLine("                {");
            sb.AppendLine($"                    var {names.Scope} = NameScope.GetNameScope({names.Element});");
            sb.AppendLine($"                    if ({names.Scope}?.Find(sourceName) is AvaloniaObject found)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        return found;");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");
        }
    }

    internal sealed class AvaloniaPropertyObservationEmitter : IPropertyObservationEmitter
    {
        public void AppendFields(StringBuilder sb, PropertyObservation observation)
        {
            sb.AppendLine("        private IDisposable? _subscription;");
        }

        public void AppendSubscribe(StringBuilder sb, PropertyObservation observation)
        {
            sb.AppendLine($"            if (_resolvedSource is {observation.TargetTypeName} typed)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _subscription = typed.GetObservable({observation.OwnerTypeName}.{observation.FieldName})");
            sb.AppendLine($"                    .Subscribe(new AnonymousObserver<{observation.ValueTypeName}>(OnObserved));");
            sb.AppendLine("            }");
        }

        public void AppendUnsubscribe(StringBuilder sb, PropertyObservation observation)
        {
            sb.AppendLine("            _subscription?.Dispose();");
            sb.AppendLine("            _subscription = null;");
        }

        public string CurrentValue(PropertyObservation observation, string sourceVariable)
        {
            return $"{sourceVariable}.GetValue({observation.OwnerTypeName}.{observation.FieldName})";
        }

        public void AppendHelpers(StringBuilder sb, PropertyObservation observation)
        {
            // GetObservable delivers typed values directly to OnObserved; no helpers are required.
        }
    }
}
