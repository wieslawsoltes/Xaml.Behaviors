using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators.UnitTests;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedAction(typeof(TestControl), "TestMethod")]
[assembly: GenerateTypedAction(typeof(TestControl), "TestMethodWithParameter")]
[assembly: GenerateTypedTrigger(typeof(TestControl), "TestEvent")]
[assembly: GenerateTypedChangePropertyAction(typeof(TestControl), "Tag")]
[assembly: GenerateTypedDataTrigger(typeof(string))]
#if !UNO
// Uno Platform: the Button.Click event command trigger is declared by the Uno-only tests (AssemblyInfo.cs).
[assembly: GenerateEventCommand(typeof(Avalonia.Controls.Button), "Click")]
#endif
[assembly: GeneratePropertyTrigger(typeof(TestControl), "Tag")]
[assembly: GeneratePropertyTrigger(typeof(RuntimePropertyHost), "FooProperty")]
[assembly: GenerateTypedTrigger(typeof(SourceTrackingControl), "SourceEvent")]

#if UNO
namespace Xaml.Behaviors.SourceGenerators.UnitTests;
#else
namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;
#endif

[GenerateTypedMultiDataTrigger]
public partial class TypedMultiDataTrigger : StyledElementTrigger
{
    [TriggerProperty]
    public string? _value1;

    [TriggerProperty]
    public string? _value2;

    public bool Evaluate()
    {
        return _value1 == "A" && _value2 == "B";
    }
}

[GenerateTypedInvokeCommandAction]
public partial class TypedInvokeCommandAction : StyledElementAction
{
    [ActionCommand]
    public System.Windows.Input.ICommand? _command;

    [ActionParameter]
    public object? _commandParameter;
}

public class RuntimePropertyHost : Control
{
#if UNO
    public static readonly DependencyProperty FooProperty =
        DependencyProperty.Register(nameof(Foo), typeof(string), typeof(RuntimePropertyHost), new PropertyMetadata(null));

    public string? Foo
    {
        get => (string?)GetValue(FooProperty);
        set => SetValue(FooProperty, value);
    }
#else
    public static readonly Avalonia.StyledProperty<string?> FooProperty =
        Avalonia.AvaloniaProperty.Register<RuntimePropertyHost, string?>(nameof(Foo));

    public string? Foo
    {
        get => GetValue(FooProperty);
        set => SetValue(FooProperty, value);
    }
#endif
}

public class DispatcherEventSource : Control
{
    [GenerateEventCommand(UseDispatcher = true)]
    public event EventHandler? Fired;

    public int SubscriptionCount => Fired?.GetInvocationList().Length ?? 0;

    internal Delegate? FirstHandler => Fired;

    public void Raise() => Fired?.Invoke(this, EventArgs.Empty);
}
