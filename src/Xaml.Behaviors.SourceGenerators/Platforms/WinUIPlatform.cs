// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;

namespace Xaml.Behaviors.SourceGenerators.Platforms
{
    /// <summary>
    /// WinUI / Uno Platform strategy (<c>Xaml.Interactivity</c> from <c>Xaml.Behaviors.Uno.Interactivity</c>,
    /// dependency properties, <c>DispatcherQueue</c>). Only public WinUI APIs are used because the code is
    /// generated into user assemblies.
    /// </summary>
    internal sealed class WinUIPlatform : IXamlPlatform
    {
        internal const string DependencyObject = "global::Microsoft.UI.Xaml.DependencyObject";
        internal const string DependencyProperty = "global::Microsoft.UI.Xaml.DependencyProperty";
        internal const string FrameworkElement = "global::Microsoft.UI.Xaml.FrameworkElement";

        public static WinUIPlatform Instance { get; } = new();

        private WinUIPlatform()
        {
        }

        public TargetPlatform Kind => TargetPlatform.WinUI;

        public IUsingDirectivesEmitter Usings { get; } = new WinUIUsingDirectivesEmitter();

        public IInteractivityTypeNames Types { get; } = new WinUIInteractivityTypeNames();

        public IPropertySystemEmitter Properties { get; } = new WinUIPropertySystemEmitter();

        public IDispatcherEmitter Dispatcher { get; } = new WinUIDispatcherEmitter();

        public INameScopeEmitter NameScope { get; } = new WinUINameScopeEmitter();

        public IPropertyObservationEmitter PropertyObservation { get; } = new WinUIPropertyObservationEmitter();
    }

    internal sealed class WinUIUsingDirectivesEmitter : IUsingDirectivesEmitter
    {
        private static readonly string[] s_namespaces =
        {
            "System",
            "System.Collections.Generic",
            "System.Threading",
            "System.Threading.Tasks",
            "System.Windows.Input",
            "Xaml.Interactivity",
        };

        public void Append(StringBuilder sb, GeneratedSourceKind kind)
        {
            // WinUI types are always referenced fully qualified; the runtime and BCL namespaces are shared by all kinds.
            foreach (var ns in s_namespaces)
            {
                sb.AppendLine($"using {ns};");
            }
        }
    }

    internal sealed class WinUIInteractivityTypeNames : IInteractivityTypeNames
    {
        public string StyledElementAction => "global::Xaml.Interactivity.StyledElementAction";

        public string StyledElementTrigger => "global::Xaml.Interactivity.StyledElementTrigger";

        public string ReversibleAction => "global::Xaml.Interactivity.IReversibleAction";
    }

    internal sealed class WinUIPropertySystemEmitter : IPropertySystemEmitter
    {
        public string ChangedEventArgsType => "global::Microsoft.UI.Xaml.DependencyPropertyChangedEventArgs";

        public void AppendField(StringBuilder sb, string ownerClassName, PropertySpec property)
        {
            var defaultValue = property.DefaultValue ?? $"default({property.Type})";
            sb.AppendLine($"        public static readonly {WinUIPlatform.DependencyProperty} {property.Name}Property =");
            sb.AppendLine($"            {WinUIPlatform.DependencyProperty}.Register(nameof({property.Name}), typeof({property.TypeOf}), typeof({ownerClassName}), new global::Microsoft.UI.Xaml.PropertyMetadata({defaultValue}, static (d, e) => (({ownerClassName})d).OnPropertyChanged(e)));");
        }

        public void AppendAccessor(StringBuilder sb, PropertySpec property)
        {
            sb.AppendLine($"        public {property.Type} {property.Name}");
            sb.AppendLine("        {");
            sb.AppendLine($"            get => ({property.Type})GetValue({property.Name}Property)!;");
            // WinUI has no read-only dependency properties (SetValue is public): the output properties (IsExecuting,
            // LastError, ...) keep a public setter so XAML can bind them two-way (WinUI has no OneWayToSource).
            sb.AppendLine($"            set => SetValue({property.Name}Property, value);");
            sb.AppendLine("        }");
        }

        public string NewValue(string changeVariable, string typeName)
        {
            return $"({typeName}){changeVariable}.NewValue!";
        }
    }

    internal sealed class WinUIDispatcherEmitter : IDispatcherEmitter
    {
        public string PostMethod => "PostOnUIThread";

        public string InvokeMethod => "InvokeOnUIThread";

        public string CheckAccessExpression => "HasUIThreadAccess()";

        public void AppendHelpers(StringBuilder sb, DispatcherFeatures features)
        {
            if (features == DispatcherFeatures.None)
            {
                return;
            }

            sb.AppendLine();
            sb.AppendLine("        private readonly global::Microsoft.UI.Dispatching.DispatcherQueue? _uiThreadDispatcherQueue = global::Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();");
            sb.AppendLine();
            sb.AppendLine("        private global::Microsoft.UI.Dispatching.DispatcherQueue? GetUIThreadDispatcherQueue()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return _uiThreadDispatcherQueue ?? (({WinUIPlatform.DependencyObject})this).DispatcherQueue ?? global::Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();");
            sb.AppendLine("        }");

            if ((features & DispatcherFeatures.Post) != 0)
            {
                sb.AppendLine();
                sb.AppendLine("        private void PostOnUIThread(global::System.Action action)");
                sb.AppendLine("        {");
                sb.AppendLine("            var queue = GetUIThreadDispatcherQueue();");
                sb.AppendLine("            if (queue is null || !queue.TryEnqueue(() => action()))");
                sb.AppendLine("            {");
                sb.AppendLine("                action();");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
            }

            if ((features & DispatcherFeatures.Invoke) != 0)
            {
                sb.AppendLine();
                sb.AppendLine("        private bool HasUIThreadAccess()");
                sb.AppendLine("        {");
                sb.AppendLine("            var queue = GetUIThreadDispatcherQueue();");
                sb.AppendLine("            return queue is null || queue.HasThreadAccess;");
                sb.AppendLine("        }");
                sb.AppendLine();
                sb.AppendLine("        private bool InvokeOnUIThread(global::System.Func<bool> function)");
                sb.AppendLine("        {");
                sb.AppendLine("            var queue = GetUIThreadDispatcherQueue();");
                sb.AppendLine("            if (queue is null || queue.HasThreadAccess)");
                sb.AppendLine("            {");
                sb.AppendLine("                return function();");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            var completion = new global::System.Threading.Tasks.TaskCompletionSource<bool>();");
                sb.AppendLine("            if (!queue.TryEnqueue(() =>");
                sb.AppendLine("                {");
                sb.AppendLine("                    try");
                sb.AppendLine("                    {");
                sb.AppendLine("                        completion.SetResult(function());");
                sb.AppendLine("                    }");
                sb.AppendLine("                    catch (global::System.Exception exception)");
                sb.AppendLine("                    {");
                sb.AppendLine("                        completion.SetException(exception);");
                sb.AppendLine("                    }");
                sb.AppendLine("                }))");
                sb.AppendLine("            {");
                sb.AppendLine("                return function();");
                sb.AppendLine("            }");
                sb.AppendLine();
                sb.AppendLine("            return completion.Task.GetAwaiter().GetResult();");
                sb.AppendLine("        }");
            }
        }
    }

    internal sealed class WinUINameScopeEmitter : INameScopeEmitter
    {
        public void AppendFindInNameScope(StringBuilder sb, NameScopeLookupNames names)
        {
            // WinUI has no logical tree: walk the element parents and query each name scope with FindName.
            sb.AppendLine($"        private static {WinUIPlatform.DependencyObject}? FindInNameScope({WinUIPlatform.DependencyObject}? source, string sourceName)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var current = source as {WinUIPlatform.FrameworkElement};");
            sb.AppendLine("            while (current is not null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                if (current.FindName(sourceName) is {WinUIPlatform.DependencyObject} found)");
            sb.AppendLine("                {");
            sb.AppendLine("                    return found;");
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine($"                current = (current.Parent ?? global::Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(current)) as {WinUIPlatform.FrameworkElement};");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return null;");
            sb.AppendLine("        }");
        }
    }

    internal sealed class WinUIPropertyObservationEmitter : IPropertyObservationEmitter
    {
        private const string NotifyPropertyChanged = "global::System.ComponentModel.INotifyPropertyChanged";

        public void AppendFields(StringBuilder sb, PropertyObservation observation)
        {
            if (observation.Kind == PropertyObservationKind.NotifyPropertyChanged)
            {
                sb.AppendLine($"        private {NotifyPropertyChanged}? _observedSource;");
                return;
            }

            sb.AppendLine($"        private {WinUIPlatform.DependencyObject}? _observedSource;");
            sb.AppendLine("        private long _propertyChangedToken;");
        }

        public void AppendSubscribe(StringBuilder sb, PropertyObservation observation)
        {
            sb.AppendLine($"            if (_resolvedSource is {observation.TargetTypeName} typed)");
            sb.AppendLine("            {");
            sb.AppendLine("                _observedSource = typed;");
            if (observation.Kind == PropertyObservationKind.NotifyPropertyChanged)
            {
                sb.AppendLine("                _observedSource.PropertyChanged += OnSourcePropertyChanged;");
            }
            else
            {
                sb.AppendLine($"                _propertyChangedToken = _observedSource.RegisterPropertyChangedCallback({observation.OwnerTypeName}.{observation.FieldName}, OnSourcePropertyChanged);");
            }

            sb.AppendLine("            }");
        }

        public void AppendUnsubscribe(StringBuilder sb, PropertyObservation observation)
        {
            sb.AppendLine("            if (_observedSource is not null)");
            sb.AppendLine("            {");
            if (observation.Kind == PropertyObservationKind.NotifyPropertyChanged)
            {
                sb.AppendLine("                _observedSource.PropertyChanged -= OnSourcePropertyChanged;");
            }
            else
            {
                sb.AppendLine($"                _observedSource.UnregisterPropertyChangedCallback({observation.OwnerTypeName}.{observation.FieldName}, _propertyChangedToken);");
            }

            sb.AppendLine("                _observedSource = null;");
            sb.AppendLine("            }");
        }

        public string CurrentValue(PropertyObservation observation, string sourceVariable)
        {
            return observation.Kind == PropertyObservationKind.NotifyPropertyChanged
                ? $"{sourceVariable}.{observation.ClrPropertyName}"
                : $"({observation.ValueTypeName}){sourceVariable}.GetValue({observation.OwnerTypeName}.{observation.FieldName})!";
        }

        public void AppendHelpers(StringBuilder sb, PropertyObservation observation)
        {
            sb.AppendLine();
            if (observation.Kind == PropertyObservationKind.NotifyPropertyChanged)
            {
                sb.AppendLine("        private void OnSourcePropertyChanged(object? sender, global::System.ComponentModel.PropertyChangedEventArgs e)");
                sb.AppendLine("        {");
                sb.AppendLine($"            if ((string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == \"{observation.ClrPropertyName}\") && sender is {observation.TargetTypeName} typed)");
                sb.AppendLine("            {");
                sb.AppendLine($"                OnObserved({CurrentValue(observation, "typed")});");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
                return;
            }

            sb.AppendLine($"        private void OnSourcePropertyChanged({WinUIPlatform.DependencyObject} sender, {WinUIPlatform.DependencyProperty} property)");
            sb.AppendLine("        {");
            sb.AppendLine($"            OnObserved(({observation.ValueTypeName})sender.GetValue(property)!);");
            sb.AppendLine("        }");
        }
    }
}
