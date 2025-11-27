using System;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;
using Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

[assembly: GenerateTypedAction(typeof(TestControl), "TestMethod")]
[assembly: GenerateTypedAction(typeof(TestControl), "TestMethodWithParameter")]
[assembly: GenerateTypedTrigger(typeof(TestControl), "TestEvent")]
[assembly: GenerateTypedChangePropertyAction(typeof(TestControl), "Tag")]
[assembly: GenerateTypedDataTrigger(typeof(string))]
[assembly: GenerateEventCommand(typeof(Avalonia.Controls.Button), "Click")]
[assembly: GeneratePropertyTrigger(typeof(TestControl), "Tag")]
[assembly: GeneratePropertyTrigger(typeof(RuntimePropertyHost), "FooProperty")]
[assembly: GenerateTypedTrigger(typeof(SourceTrackingControl), "SourceEvent")]

namespace Avalonia.Xaml.Behaviors.SourceGenerators.UnitTests;

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
    public static readonly Avalonia.StyledProperty<string?> FooProperty =
        Avalonia.AvaloniaProperty.Register<RuntimePropertyHost, string?>(nameof(Foo));

    public string? Foo
    {
        get => GetValue(FooProperty);
        set => SetValue(FooProperty, value);
    }
}

public class DispatcherEventSource : Control
{
    [GenerateEventCommand(UseDispatcher = true)]
    public event EventHandler? Fired;

    public int SubscriptionCount => Fired?.GetInvocationList().Length ?? 0;

    internal Delegate? FirstHandler => Fired;

    public void Raise() => Fired?.Invoke(this, EventArgs.Empty);
}
