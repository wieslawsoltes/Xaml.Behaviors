# Event Contracts

The `Xaml.Behaviors.Interactivity` package includes an event abstraction layer located in the `Events` namespace. This system allows `EventTriggerBase` to subscribe to events on objects without relying solely on reflection, and provides a way to support custom event subscription logic.

## Core Components

The system is built around a registry and an interface:

*   **`IAddEventHandler`**: An interface that defines how to check if a handler applies to a specific object/event pair, and how to add the handler.
*   **`AddEventHandlerRegistry`**: A static registry that holds a collection of `IAddEventHandler` implementations.

## How It Works

When `EventTriggerBase` attempts to subscribe to an event (specified by `EventName`), it follows this process:

1.  It calls `AddEventHandlerRegistry.TryRegisterEventHandler(source, eventName, handler)`.
2.  The registry iterates through its registered `IAddEventHandler` implementations.
3.  If an implementation returns `true` for `Matches(source, eventName)`, its `AddHandler` method is called.
4.  The `AddHandler` method subscribes to the event and returns an `IDisposable` that will unsubscribe when disposed.
5.  If no matching handler is found in the registry, `EventTriggerBase` falls back to standard .NET reflection to find and subscribe to the event.

## IAddEventHandler Interface

```csharp
public interface IAddEventHandler
{
    bool Matches(object source, string eventName);
    IDisposable? AddHandler(object source, string eventName, Action<object?, object> handler);
}
```

## FuncAddEventHandler

The SDK provides a generic implementation called `FuncAddEventHandler<TTarget, TEventArgs>` which simplifies registering handlers for standard .NET events.

```csharp
// Example registration for Button.Click
AddEventHandlerRegistry.Register(new FuncAddEventHandler<Button, RoutedEventArgs>(
    nameof(Button.Click), 
    (button, handler) => button.Click += handler, 
    (button, handler) => button.Click -= handler));
```

## Registering Custom Handlers

You can register your own handlers to optimize performance for frequently used events or to support events that don't follow the standard pattern.

```csharp
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Register a fast path for Button.Click
        AddEventHandlerRegistry.Register(new FuncAddEventHandler<Button, RoutedEventArgs>(
            "Click",
            (btn, handler) => btn.Click += handler,
            (btn, handler) => btn.Click -= handler));
    }
}
```

By registering a handler, you avoid the overhead of reflection every time an `EventTriggerBehavior` attaches to a `Button`'s `Click` event.
