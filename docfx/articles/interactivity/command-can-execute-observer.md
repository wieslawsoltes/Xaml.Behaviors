# CommandCanExecuteObserver

`CommandCanExecuteObserver` is a public helper for command-backed behaviors that need to expose a bindable command availability state.

It observes `ICommand.CanExecute(parameter)`, listens to `CanExecuteChanged`, and reports the current value through a callback. If the command is `null`, it reports `true`.

The observer uses a weak event target for `CanExecuteChanged`, so a long-lived command cannot keep the observer or owning behavior alive if cleanup is missed. You should still call `Stop()` or `Dispose()` when the behavior is detached so the command subscription is removed immediately.

## Behavior lifecycle

Use one observer per command state property:

* Call `Start(command, parameter)` when the behavior is attached.
* Call `Update(command, parameter)` when the command or command parameter changes.
* Call `Stop()` or `Dispose()` when the behavior is detached.

## Custom behavior example

```csharp
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

public sealed class MyCommandBehavior : StyledElementBehavior<Control>
{
    private readonly CommandCanExecuteObserver _canExecuteObserver;
    private bool _canExecuteCommand = true;

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<MyCommandBehavior, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<MyCommandBehavior, object?>(nameof(CommandParameter));

    public static readonly DirectProperty<MyCommandBehavior, bool> CanExecuteCommandProperty =
        AvaloniaProperty.RegisterDirect<MyCommandBehavior, bool>(
            nameof(CanExecuteCommand),
            behavior => behavior.CanExecuteCommand);

    public MyCommandBehavior()
    {
        _canExecuteObserver = new CommandCanExecuteObserver(value => CanExecuteCommand = value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool CanExecuteCommand
    {
        get => _canExecuteCommand;
        private set => SetAndRaise(CanExecuteCommandProperty, ref _canExecuteCommand, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        _canExecuteObserver.Start(Command, CommandParameter);
    }

    protected override void OnDetaching()
    {
        _canExecuteObserver.Stop();
        base.OnDetaching();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CommandProperty || change.Property == CommandParameterProperty)
        {
            _canExecuteObserver.Update(Command, CommandParameter);
        }
    }
}
```

Bind the associated control to the exposed state:

```xml
<Button Content="Run"
        IsEnabled="{Binding #CommandBehavior.CanExecuteCommand}">
    <Interaction.Behaviors>
        <local:MyCommandBehavior x:Name="CommandBehavior"
                                 Command="{Binding RunCommand}" />
    </Interaction.Behaviors>
</Button>
```
