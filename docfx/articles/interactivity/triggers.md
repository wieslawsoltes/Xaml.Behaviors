# Triggers

Triggers are behaviors that listen for specific conditions or events and invoke a collection of actions when those conditions are met.

## The Trigger Base Class

The `Trigger` class is a specialized `Behavior` that contains an `Actions` collection. When the trigger is activated, it executes the actions in this collection.

```csharp
public abstract class Trigger : Behavior, ITrigger
{
    public ActionCollection Actions { get; }
}
```

## EventTriggerBase

`EventTriggerBase` is a common base class for triggers that listen for events. It provides the `EventName` and `SourceObject` properties.

*   **`EventName`**: The name of the event to listen for.
*   **`SourceObject`**: The object that raises the event. If not set, it defaults to the `AssociatedObject`.

### Creating a Custom Event Trigger

To create a custom event trigger, you can inherit from `EventTriggerBase` or `EventTriggerBase<T>`.

```csharp
using Avalonia.Xaml.Interactivity;

public class MyEventTrigger : EventTriggerBase<Button>
{
    protected override void OnEvent(object? sender, object? eventArgs)
    {
        // Custom logic before invoking actions
        base.OnEvent(sender, eventArgs);
    }
}
```

## Invoking Actions

Triggers are responsible for invoking their actions. The `Interaction.ExecuteActions` method is used to execute the actions associated with a trigger.

If you are implementing a custom trigger that is not based on `EventTriggerBase`, you can invoke actions manually:

```csharp
protected void InvokeActions(object parameter)
{
    Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
}
```

## StyledElementTrigger

Like behaviors, triggers also have a `StyledElement` version: `StyledElementTrigger`. This class inherits from `StyledElementBehavior` and supports `DataContext` synchronization and logical tree attachment. `EventTriggerBase` inherits from `StyledElementTrigger`.

Use `StyledElementTrigger` when your trigger needs to bind to properties or access resources in the logical tree.

## Conditions

The SDK also provides a `Condition` class that can be used to define complex criteria for triggers. While primarily used by `DataTriggerBehavior` (in the Interactions package), `Condition` is part of the core Interactivity package.

A `Condition` allows you to compare a bound value against a specified value using various comparison operators (Equal, NotEqual, GreaterThan, etc.).
