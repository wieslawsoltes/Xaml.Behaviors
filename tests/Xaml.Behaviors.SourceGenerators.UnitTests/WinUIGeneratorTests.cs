using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

/// <summary>
/// Compile-only tests of the WinUI / Uno Platform emission (sources compiled against the Uno.WinUI reference
/// assemblies and Xaml.Behaviors.Uno.Interactivity).
/// </summary>
public class WinUIGeneratorTests
{
    private const string Usings = """
        using System;
        using System.ComponentModel;
        using System.Threading.Tasks;
        using System.Windows.Input;
        using Microsoft.UI.Xaml;
        using Microsoft.UI.Xaml.Controls;
        using Xaml.Behaviors.SourceGenerators;

        """;

    private static WinUIGeneratorRun AssertCompiles(string source, string? platformOverride = null)
    {
        var run = WinUIGeneratorTestHelper.Run(Usings + source, platformOverride);
        Assert.True(
            run.CompilationErrors.IsEmpty && run.GeneratorDiagnostics.All(static d => d.Severity != DiagnosticSeverity.Error),
            WinUIGeneratorTestHelper.Describe(run));
        Assert.True(run.GeneratedCodeWarnings.IsEmpty || platformOverride is not null, WinUIGeneratorTestHelper.Describe(run));
        Assert.NotEmpty(run.GeneratedSources);
        return run;
    }

    private static WinUIGeneratorRun AssertDiagnostic(string source, string id)
    {
        var run = WinUIGeneratorTestHelper.Run(Usings + source);
        Assert.True(run.GeneratorDiagnostics.Any(d => d.Id == id), WinUIGeneratorTestHelper.Describe(run));
        Assert.True(run.CompilationErrors.IsEmpty, WinUIGeneratorTestHelper.Describe(run));
        return run;
    }

    [Fact]
    public void Action_Emits_DependencyProperties_And_WinUI_Base_Type()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public partial class ViewModel
            {
                [GenerateTypedAction]
                public void Submit(string? text, int count) { }
            }
            """);

        var source = run.AllSources;
        Assert.Contains(": global::Xaml.Interactivity.StyledElementAction", source);
        Assert.Contains("global::Microsoft.UI.Xaml.DependencyProperty.Register(nameof(Text), typeof(string), typeof(SubmitAction)", source);
        Assert.Contains("global::Microsoft.UI.Xaml.DependencyProperty.Register(nameof(Count), typeof(int), typeof(SubmitAction)", source);
        Assert.Contains("get => (int)GetValue(CountProperty)!;", source);
        Assert.DoesNotContain("Avalonia", source);
    }

    [Fact]
    public void Action_With_EventHandler_Signature_And_Assembly_Attribute()
    {
        AssertCompiles("""
            [assembly: GenerateTypedAction(typeof(WinUITests.Handlers), "OnClick")]

            namespace WinUITests;

            public class Handlers
            {
                public void OnClick(object? sender, RoutedEventArgs? e) { }
            }
            """);
    }

    [Fact]
    public void Async_Action_With_Dispatcher_Emits_DispatcherQueue_Helpers()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public partial class ViewModel
            {
                [GenerateTypedAction(UseDispatcher = true)]
                public Task LoadAsync(int page) => Task.CompletedTask;

                [GenerateTypedAction]
                public ValueTask SaveAsync() => default;
            }
            """);

        var source = run.AllSources;
        Assert.Contains("PostOnUIThread(() =>", source);
        Assert.Contains("global::Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread()", source);
        Assert.Contains("nameof(LastError), typeof(System.Exception)", source);
    }

    [Fact]
    public void Trigger_On_RoutedEvent_TypedEvent_And_Custom_Events()
    {
        var run = AssertCompiles("""
            [assembly: GenerateTypedTrigger(typeof(Button), "Click")]
            [assembly: GenerateTypedTrigger(typeof(FrameworkElement), "ActualThemeChanged")]
            [assembly: GenerateTypedTrigger(typeof(UIElement), "PointerPressed")]
            [assembly: GenerateTypedTrigger(typeof(TextBox), "TextChanged")]

            namespace WinUITests;

            public class Clock
            {
                [GenerateTypedTrigger]
                public event EventHandler<int>? Ticked;

                [GenerateTypedTrigger]
                public event Action? Stopped;

                public void Raise()
                {
                    Ticked?.Invoke(this, 1);
                    Stopped?.Invoke();
                }
            }
            """);

        var source = run.AllSources;
        Assert.Contains(": global::Xaml.Interactivity.StyledElementTrigger", source);
        Assert.Contains("typedSource.Click += _proxy.OnEvent;", source);
        Assert.Contains("typedSource.ActualThemeChanged += _proxy.OnEvent;", source);
        Assert.Contains("OnPropertyChanged(global::Microsoft.UI.Xaml.DependencyPropertyChangedEventArgs change)", source);
    }

    [Fact]
    public void ChangePropertyAction_Emits_Reversible_WinUI_Action()
    {
        var run = AssertCompiles("""
            [assembly: GenerateTypedChangePropertyAction(typeof(TextBlock), "Text")]
            [assembly: GenerateTypedChangePropertyAction(typeof(FrameworkElement), "Width", UseDispatcher = true)]

            namespace WinUITests;

            public class Settings
            {
                [GenerateTypedChangePropertyAction]
                public double? Scale { get; set; }
            }
            """);

        var source = run.AllSources;
        Assert.Contains("global::Xaml.Interactivity.IReversibleAction", source);
        Assert.Contains("ReversiblePropertyChange<global::Microsoft.UI.Xaml.Controls.TextBlock, string>", source);
        Assert.Contains("typeof(double?)", source);
        Assert.Contains("HasUIThreadAccess()", source);
        Assert.Contains("InvokeOnUIThread(() =>", source);
    }

    [Fact]
    public void DataTriggers_For_Value_And_Reference_Types()
    {
        var run = AssertCompiles("""
            [assembly: GenerateTypedDataTrigger(typeof(int))]
            [assembly: GenerateTypedDataTrigger(typeof(string))]
            [assembly: GenerateTypedDataTrigger(typeof(Visibility))]

            namespace WinUITests;

            public class Marker { }
            """);

        var source = run.AllSources;
        Assert.Contains("partial class Int32DataTrigger : global::Xaml.Interactivity.StyledElementTrigger", source);
        Assert.Contains("typeof(ComparisonConditionType)", source);
    }

    [Fact]
    public void MultiDataTrigger_Uses_DependencyPropertyChangedEventArgs()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            [GenerateTypedMultiDataTrigger]
            public partial class RangeTrigger : Xaml.Interactivity.StyledElementTrigger
            {
                [TriggerProperty]
                private int _minimum;

                [TriggerProperty]
                private string? _label;

                private bool Evaluate() => _minimum > 0 && _label is not null;
            }
            """);

        Assert.Contains("this._minimum = (int)change.NewValue!;", run.AllSources);
    }

    [Fact]
    public void InvokeCommandAction_With_Dispatcher()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            [GenerateTypedInvokeCommandAction(UseDispatcher = true)]
            public partial class RunCommandAction : Xaml.Interactivity.StyledElementAction
            {
                [ActionCommand]
                private ICommand? _command;

                [ActionParameter]
                private object? _parameter;
            }
            """);

        Assert.Contains("PostOnUIThread(() =>", run.AllSources);
    }

    [Fact]
    public void PropertyTrigger_On_Framework_Static_DependencyProperty_Identifiers()
    {
        var run = AssertCompiles("""
            [assembly: GeneratePropertyTrigger(typeof(TextBox), "TextProperty")]
            [assembly: GeneratePropertyTrigger(typeof(FrameworkElement), "Width", SourceName = "Host")]

            namespace WinUITests;

            public class Marker { }
            """);

        var source = run.AllSources;
        Assert.Contains("RegisterPropertyChangedCallback(global::Microsoft.UI.Xaml.Controls.TextBox.TextProperty, OnSourcePropertyChanged)", source);
        Assert.Contains("var current = (string)typed.GetValue(global::Microsoft.UI.Xaml.Controls.TextBox.TextProperty)!;", source);
        Assert.Contains("current.FindName(sourceName)", source);
        Assert.Contains("nameof(SourceName), typeof(string), typeof(FrameworkElementWidthPropertyTrigger), new global::Microsoft.UI.Xaml.PropertyMetadata(\"Host\"", source);
    }

    [Fact]
    public void PropertyTrigger_On_Custom_DependencyProperty_Field_And_Property()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public class Gauge : FrameworkElement
            {
                [GeneratePropertyTrigger]
                public static readonly DependencyProperty LevelProperty =
                    DependencyProperty.Register(nameof(Level), typeof(int), typeof(Gauge), new PropertyMetadata(0));

                public static readonly DependencyProperty CaptionProperty =
                    DependencyProperty.Register(nameof(Caption), typeof(string), typeof(Gauge), new PropertyMetadata(null));

                public int Level
                {
                    get => (int)GetValue(LevelProperty);
                    set => SetValue(LevelProperty, value);
                }

                [GeneratePropertyTrigger(Name = "CaptionTrigger", UseDispatcher = true)]
                public string? Caption
                {
                    get => (string?)GetValue(CaptionProperty);
                    set => SetValue(CaptionProperty, value);
                }
            }
            """);

        var source = run.AllSources;
        Assert.Contains("partial class LevelPropertyTrigger", source);
        Assert.Contains("private void OnObserved(int value)", source);
        Assert.Contains("partial class CaptionTrigger", source);
        Assert.Contains("private void OnObserved(string? value)", source);
    }

    [Fact]
    public void PropertyTrigger_On_NotifyPropertyChanged_Property()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public class ViewModel : INotifyPropertyChanged
            {
                private int _count;

                public event PropertyChangedEventHandler? PropertyChanged;

                [GeneratePropertyTrigger]
                public int Count
                {
                    get => _count;
                    set
                    {
                        _count = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    }
                }
            }
            """);

        var source = run.AllSources;
        Assert.Contains("_observedSource.PropertyChanged += OnSourcePropertyChanged;", source);
        Assert.Contains("e.PropertyName == \"Count\"", source);
    }

    [Fact]
    public void EventCommand_With_ParameterPath_And_Dispatcher()
    {
        var run = AssertCompiles("""
            [assembly: GenerateEventCommand(typeof(Button), "Click", UseDispatcher = true)]
            [assembly: GenerateEventCommand(typeof(TextBox), "TextChanged", ParameterPath = "OriginalSource", Name = "TextChangedCommandTrigger")]

            namespace WinUITests;

            public class Marker { }
            """);

        var source = run.AllSources;
        Assert.Contains("typeof(ICommand)", source);
        Assert.Contains("IsSet(ParameterProperty)", source);
        Assert.Contains("TryResolveParameterPath", source);
    }

    [Fact]
    public void EventArgsAction_With_Projection()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public partial class Handler
            {
                [GenerateEventArgsAction(Project = "OriginalSource")]
                public void Handle(RoutedEventArgs args) { }

                [GenerateEventArgsAction(UseDispatcher = true)]
                public Task HandleAsync(RoutedEventArgs args) => Task.CompletedTask;
            }
            """);

        Assert.Contains("nameof(OriginalSource), typeof(object)", run.AllSources);
    }

    [Fact]
    public void Async_And_Observable_Triggers()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public class Source
            {
                [GenerateAsyncTrigger]
                public Task<int>? Loading { get; set; }

                [GenerateAsyncTrigger(UseDispatcher = false)]
                public Task? Saving { get; set; }

                [GenerateObservableTrigger]
                public IObservable<string>? Messages { get; set; }
            }
            """);

        var source = run.AllSources;
        Assert.Contains("typeof(global::System.Threading.Tasks.Task<int>)", source);
        Assert.Contains("nameof(LastResult), typeof(int)", source);
        Assert.Contains("nameof(LastValue), typeof(string)", source);
    }

    [Fact]
    public void MultiDataTrigger_Without_WinUI_Base_Reports_XBG036()
    {
        AssertDiagnostic("""
            namespace WinUITests;

            [GenerateTypedMultiDataTrigger]
            public partial class NotATrigger
            {
                [TriggerProperty]
                private int _value;

                private bool Evaluate() => _value > 0;
            }
            """, "XBG036");
    }

    [Fact]
    public void InvokeCommandAction_Without_WinUI_Base_Reports_XBG036()
    {
        AssertDiagnostic("""
            namespace WinUITests;

            [GenerateTypedInvokeCommandAction]
            public partial class NotAnAction
            {
                [ActionCommand]
                private ICommand? _command;
            }
            """, "XBG036");
    }

    [Fact]
    public void PropertyTrigger_On_Plain_Property_Reports_XBG037()
    {
        AssertDiagnostic("""
            namespace WinUITests;

            public class Plain
            {
                [GeneratePropertyTrigger]
                public int Value { get; set; }
            }
            """, "XBG037");
    }

    [Fact]
    public void PropertyTrigger_On_DependencyProperty_Of_Static_Class_Reports_XBG038()
    {
        AssertDiagnostic("""
            namespace WinUITests;

            public static class Attached
            {
                [GeneratePropertyTrigger]
                public static readonly DependencyProperty ModeProperty =
                    DependencyProperty.RegisterAttached("Mode", typeof(int), typeof(Attached), new PropertyMetadata(0));

                public static int GetMode(DependencyObject element) => (int)element.GetValue(ModeProperty);
            }
            """, "XBG038");
    }

    [Fact]
    public void PropertyTrigger_SourceName_On_Non_FrameworkElement_Reports_XBG022()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public class ViewModel : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler? PropertyChanged;

                [GeneratePropertyTrigger(SourceName = "Other")]
                public string? Name { get; set; }

                public void Raise() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
            """);

        Assert.Contains(run.GeneratorDiagnostics, static d => d.Id == "XBG022");
    }

    [Fact]
    public void Platform_Override_Selects_Avalonia_Emission()
    {
        var run = WinUIGeneratorTestHelper.Run(Usings + """
            namespace WinUITests;

            public partial class ViewModel
            {
                [GenerateTypedAction]
                public void Submit() { }
            }
            """, platformOverride: "Avalonia");

        var source = run.AllSources;
        Assert.Contains("using Avalonia.Xaml.Interactivity;", source);
        Assert.Contains("AvaloniaProperty.Register<SubmitAction, object?>(nameof(TargetObject));", source);
    }

    [Fact]
    public void Platform_Override_WinUI_Is_Accepted()
    {
        var run = AssertCompiles("""
            namespace WinUITests;

            public partial class ViewModel
            {
                [GenerateTypedAction]
                public void Submit() { }
            }
            """, platformOverride: "WinUI");

        Assert.Contains("global::Xaml.Interactivity.StyledElementAction", run.AllSources);
    }
}
