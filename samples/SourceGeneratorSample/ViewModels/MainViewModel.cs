using System;
using ReactiveUI;
using System.Windows.Input;
using System.Reactive;
using Xaml.Behaviors.SourceGenerators;
using SourceGeneratorSample.Models;
using System.Reactive.Linq;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Layout;
#endif
using System.Diagnostics.CodeAnalysis;

#if UNO
// The Uno Platform twin generates the same actions and triggers for the WinUI counterparts of the Avalonia types.
[assembly: GenerateTypedChangePropertyAction(typeof(Microsoft.UI.Xaml.Controls.TextBlock), "Foreground")]
[assembly: GenerateTypedChangePropertyAction(typeof(Microsoft.UI.Xaml.Controls.TextBlock), "Text")]
// Click is declared on ButtonBase in WinUI: the trigger is Microsoft.UI.Xaml.Controls.Primitives.ButtonBaseClickTrigger.
[assembly: GenerateTypedTrigger(typeof(Microsoft.UI.Xaml.Controls.Button), "Click")]
[assembly: GenerateTypedTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "ProcessingFinished")]
[assembly: GenerateTypedDataTrigger(typeof(double))]
[assembly: GenerateTypedDataTrigger(typeof(string))]
// WinUI routed event arguments expose the element that raised the event as OriginalSource (Avalonia: Source).
[assembly: GenerateEventCommand(typeof(Microsoft.UI.Xaml.Controls.Button), "Click", ParameterPath = "OriginalSource")]
[assembly: GenerateAsyncTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "DemoTask")]
[assembly: GenerateObservableTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "DemoObservable")]
// WinUI has no IsVisible and Bounds properties: Visibility is the visibility property, and the layout width (Width) is
// the closest observable dependency property to the bounds (ActualWidth does not raise property change notifications).
[assembly: GeneratePropertyTrigger(typeof(Microsoft.UI.Xaml.UIElement), "VisibilityProperty")]
[assembly: GeneratePropertyTrigger(typeof(Microsoft.UI.Xaml.FrameworkElement), "WidthProperty")]
[assembly: GeneratePropertyTrigger(typeof(Microsoft.UI.Xaml.Controls.TextBox), "TextProperty", UseDispatcher = true, Name = "DispatchingTextPropertyTrigger")]

// Wildcard examples, you can also use regex patterns (UIElement is the WinUI counterpart of Avalonia's InputElement).
[assembly: GenerateTypedAction(typeof(Microsoft.UI.Xaml.Controls.TextBox), "Copy*")]
[assembly: GenerateTypedTrigger(typeof(Microsoft.UI.Xaml.UIElement), "*")]
[assembly: GenerateTypedChangePropertyAction(typeof(Microsoft.UI.Xaml.UIElement), "*")]
#else
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Foreground")]
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Text")]
[assembly: GenerateTypedTrigger(typeof(Avalonia.Controls.Button), "Click")]
[assembly: GenerateTypedTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "ProcessingFinished")]
[assembly: GenerateTypedDataTrigger(typeof(double))]
[assembly: GenerateTypedDataTrigger(typeof(string))]
[assembly: GenerateEventCommand(typeof(Avalonia.Controls.Button), "Click", ParameterPath = "Source")]
[assembly: GenerateAsyncTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "DemoTask")]
[assembly: GenerateObservableTrigger(typeof(SourceGeneratorSample.ViewModels.MainViewModel), "DemoObservable")]
[assembly: GeneratePropertyTrigger(typeof(Avalonia.Visual), "IsVisibleProperty")]
[assembly: GeneratePropertyTrigger(typeof(Avalonia.Visual), "BoundsProperty")]
[assembly: GeneratePropertyTrigger(typeof(Avalonia.Controls.TextBox), "TextProperty", UseDispatcher = true, Name = "DispatchingTextPropertyTrigger")]

// Wildcard examples, you can also use regex patterns.
[assembly: GenerateTypedAction(typeof(Avalonia.Controls.TextBox), "Copy*")]
[assembly: GenerateTypedTrigger(typeof(Avalonia.Input.InputElement), "*")]
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Input.InputElement), "*")]
#endif

namespace SourceGeneratorSample.ViewModels
{
#pragma warning disable IL2026, IL3050 // Sample uses ReactiveCommand for demo purposes; trimming/AOT not targeted here.
    public partial class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            TriggerExternalCommand = ReactiveCommand.Create<string?>(_ => TriggerExternal());
            EventCommand = ReactiveCommand.Create<object?>(OnEventCommand);
            StartDemoTaskCommand = ReactiveCommand.Create(StartDemoTask);
            StartDemoObservableCommand = ReactiveCommand.Create(StartDemoObservable);
        }

        private string _statusText = "Ready";
        [GenerateTypedChangePropertyAction]
        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }

        private string _externalTriggerStatus = "Waiting...";
        public string ExternalTriggerStatus
        {
            get => _externalTriggerStatus;
            set => this.RaiseAndSetIfChanged(ref _externalTriggerStatus, value);
        }

        private double _value = 0.0;
        public double Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private int _count = 0;
        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public ExternalLibraryClass ExternalObject { get; } = new ExternalLibraryClass();
        public ICommand TriggerExternalCommand { get; }
        public ICommand EventCommand { get; }
        public ICommand StartDemoTaskCommand { get; }
        public ICommand StartDemoObservableCommand { get; }

        [GenerateTypedAction]
        public void Submit()
        {
            StatusText = "Submitted!";
            InternalSubmitMessage = "Submitted successfully!";
            Console.WriteLine("Submit called");
            ProcessingFinished?.Invoke(this, EventArgs.Empty);
        }

        [GenerateTypedAction]
        public void Reset(object? sender, object? parameter)
        {
             StatusText = "Reset";
             InternalSubmitMessage = "Reset";
             Value = 0;
        }

        [GenerateTypedAction]
        public void UpdateMessage(string message, int count)
        {
            StatusText = $"{message} (Count: {count})";
        }

        [GenerateTypedTrigger]
        public event EventHandler? ProcessingFinished;

        private string _title = "Sample App";
        [GenerateTypedChangePropertyAction]
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        private string _externalEventMessage = "Waiting for event...";
        [GenerateTypedChangePropertyAction]
        public string ExternalEventMessage
        {
            get => _externalEventMessage;
            set => this.RaiseAndSetIfChanged(ref _externalEventMessage, value);
        }

        private string _processingMessage = "Waiting for process...";
        [GenerateTypedChangePropertyAction]
        public string ProcessingMessage
        {
            get => _processingMessage;
            set => this.RaiseAndSetIfChanged(ref _processingMessage, value);
        }

        private string _eventCommandMessage = "No clicks yet";
        public string EventCommandMessage
        {
            get => _eventCommandMessage;
            set => this.RaiseAndSetIfChanged(ref _eventCommandMessage, value);
        }

        private Task<int>? _demoTask;
        [GenerateAsyncTrigger]
        public Task<int>? DemoTask
        {
            get => _demoTask;
            set => this.RaiseAndSetIfChanged(ref _demoTask, value);
        }

        private IObservable<int>? _demoObservable;
        [GenerateObservableTrigger]
        public IObservable<int>? DemoObservable
        {
            get => _demoObservable;
            set => this.RaiseAndSetIfChanged(ref _demoObservable, value);
        }

        private string _asyncTriggerStatus = "Idle";
        public string AsyncTriggerStatus
        {
            get => _asyncTriggerStatus;
            set => this.RaiseAndSetIfChanged(ref _asyncTriggerStatus, value);
        }

        private string _observableTriggerStatus = "No values yet";
        public string ObservableTriggerStatus
        {
            get => _observableTriggerStatus;
            set => this.RaiseAndSetIfChanged(ref _observableTriggerStatus, value);
        }

        private string _pointerStatus = "Click or tap the surface";
        public string PointerStatus
        {
            get => _pointerStatus;
            set => this.RaiseAndSetIfChanged(ref _pointerStatus, value);
        }

        private string _keyStatus = "Press any key to capture it";
        public string KeyStatus
        {
            get => _keyStatus;
            set => this.RaiseAndSetIfChanged(ref _keyStatus, value);
        }

        private string _propertyTriggerStatus = "Waiting for property triggers...";

        [GenerateTypedChangePropertyAction]
        public string PropertyTriggerStatus
        {
            get => _propertyTriggerStatus;
            set => this.RaiseAndSetIfChanged(ref _propertyTriggerStatus, value);
        }

        private bool _showRegion = true;
        public bool ShowRegion
        {
            get => _showRegion;
            set => this.RaiseAndSetIfChanged(ref _showRegion, value);
        }

        private double _boundsWidth = 200;
        public double BoundsWidth
        {
            get => _boundsWidth;
            set => this.RaiseAndSetIfChanged(ref _boundsWidth, value);
        }

        private string _internalSubmitMessage = "Waiting for submit...";
        public string InternalSubmitMessage
        {
            get => _internalSubmitMessage;
            set => this.RaiseAndSetIfChanged(ref _internalSubmitMessage, value);
        }

#if UNO
        // WinUI pointer arguments (PointerRoutedEventArgs) have no click count: the pointer device type is shown instead.
        [GenerateEventArgsAction(Project = "KeyModifiers")]
        public void OnPointerPressed(PointerPressedEventArgs args)
        {
            var modifiers = args.KeyModifiers == KeyModifiers.None ? "no modifiers" : args.KeyModifiers.ToString();
            PointerStatus = $"Pointer pressed ({args.Pointer.PointerDeviceType}, {modifiers})";
        }

        // WinUI key arguments (KeyRoutedEventArgs) have no modifiers: they are read from the keyboard state.
        [GenerateEventArgsAction(UseDispatcher = true, Project = "Key")]
        public void OnKeyCaptured(KeyEventArgs args)
        {
            var modifiers = GetKeyModifiers();
            KeyStatus = $"Key: {args.Key} ({(modifiers == KeyModifiers.None ? "no modifiers" : modifiers.ToString())})";
        }

        private static KeyModifiers GetKeyModifiers()
        {
            var modifiers = KeyModifiers.None;
            if (IsKeyDown(Windows.System.VirtualKey.Control))
            {
                modifiers |= KeyModifiers.Control;
            }
            if (IsKeyDown(Windows.System.VirtualKey.Shift))
            {
                modifiers |= KeyModifiers.Shift;
            }
            if (IsKeyDown(Windows.System.VirtualKey.Menu))
            {
                modifiers |= KeyModifiers.Menu;
            }
            return modifiers;

            static bool IsKeyDown(Windows.System.VirtualKey key)
                => Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
        }
#else
        [GenerateEventArgsAction(Project = "KeyModifiers,ClickCount")]
        public void OnPointerPressed(PointerPressedEventArgs args)
        {
            var modifiers = args.KeyModifiers == KeyModifiers.None ? "no modifiers" : args.KeyModifiers.ToString();
            PointerStatus = $"Pointer pressed ({args.ClickCount}x, {modifiers})";
        }

        [GenerateEventArgsAction(UseDispatcher = true, Project = "Key,KeyModifiers")]
        public void OnKeyCaptured(KeyEventArgs args)
        {
            var modifiers = args.KeyModifiers == KeyModifiers.None ? "no modifiers" : args.KeyModifiers.ToString();
            KeyStatus = $"Key: {args.Key} ({modifiers})";
        }
#endif
        
        public void TriggerExternal()
        {
            ExternalObject.RaiseExternalEvent();
            ExternalTriggerStatus = $"External triggered at {DateTime.Now:T}";
        }

        private void OnEventCommand(object? parameter)
        {
            EventCommandMessage = $"EventCommand fired with parameter: {parameter ?? "<null>"}";
        }

        private void StartDemoTask()
        {
            DemoTask = DemoAsync();
        }

        private async Task<int> DemoAsync()
        {
            AsyncTriggerStatus = "Running demo task...";
            await Task.Delay(500);
            AsyncTriggerStatus = "Demo task finished";
            return 42;
        }

        private void StartDemoObservable()
        {
            ObservableTriggerStatus = "Observable started";
            DemoObservable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(3).Select(i => (int)i);
        }

        [GenerateTypedAction]
        public async System.Threading.Tasks.Task SubmitAsync()
        {
            StatusText = "Submitting Async...";
            await System.Threading.Tasks.Task.Delay(2000);
            StatusText = "Submitted Async!";
            ProcessingFinished?.Invoke(this, EventArgs.Empty);
        }

        [GenerateTypedAction]
        public async System.Threading.Tasks.ValueTask ResetAsync()
        {
             StatusText = "Resetting Async...";
             await System.Threading.Tasks.Task.Delay(1000);
             StatusText = "Reset Async!";
             Value = 0;
        }
    }
#pragma warning restore IL2026, IL3050
}
