# Actions

Actions are objects that perform a specific operation when invoked. They are typically used within a `Trigger` to respond to events or conditions.

## The Action Base Class

The `Action` class is the base class for all actions. It defines an abstract `Execute` method that must be implemented by derived classes.

```csharp
public abstract class Action : AvaloniaObject, IAction
{
    public abstract object? Execute(object? sender, object? parameter);
}
```

*   **`sender`**: The object that triggered the action (usually the `AssociatedObject` of the trigger).
*   **`parameter`**: Optional parameter passed by the trigger (e.g., event arguments).

## Creating Custom Actions

To create a custom action, inherit from `Action` or `StyledElementAction` and implement the `Execute` method.

```csharp
using Avalonia;
using Avalonia.Xaml.Interactivity;

public class MyCustomAction : Action
{
    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<MyCustomAction, string>(nameof(Message));

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public override object? Execute(object? sender, object? parameter)
    {
        System.Diagnostics.Debug.WriteLine($"Action executed: {Message}");
        return null;
    }
}
```

## StyledElementAction

If your action needs to support data binding or access the logical tree, use `StyledElementAction`. This class inherits from `StyledElement` and ensures that the action participates in the Avalonia styling and binding system.

```csharp
public class BindableAction : StyledElementAction
{
    // ...
}
```

## InvokeCommandActionBase

`InvokeCommandActionBase` is a useful base class for actions that invoke an `ICommand`. It provides properties for `Command`, `CommandParameter`, and `InputConverter`.

*   **`Command`**: The command to execute.
*   **`CommandParameter`**: The parameter to pass to the command.
*   **`InputConverter`**: A converter to process the parameter before passing it to the command.

This class is useful when you want to create an action that bridges UI events to ViewModel commands.
