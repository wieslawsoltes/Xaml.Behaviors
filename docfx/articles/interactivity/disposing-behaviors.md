# Disposing Behaviors and Triggers

Managing the lifecycle of resources, particularly event subscriptions and observables, is a critical aspect of writing robust behaviors and triggers. The `Xaml.Behaviors.Interactivity` package provides several tools to simplify this process and prevent memory leaks.

## DisposableAction

`DisposableAction` is a utility class that implements `IDisposable` and executes a specified `Action` when disposed. It is the fundamental building block for the disposing patterns in the SDK.

### Usage

```csharp
using Avalonia.Xaml.Interactivity;

var subscription = DisposableAction.Create(() => 
{
    // Cleanup logic here
    Console.WriteLine("Disposed!");
});

subscription.Dispose(); // Prints "Disposed!"
```

## DisposingBehavior&lt;T&gt;

`DisposingBehavior<T>` is a base class for behaviors that need to manage a single disposable resource (like an event subscription or an observable subscription) during their attachment lifecycle.

Instead of overriding `OnAttached` and `OnDetaching` to manually subscribe and unsubscribe, you override `OnAttachedOverride` which returns an `IDisposable`. The base class automatically disposes of this resource when the behavior is detached.

### Example

```csharp
public class MyBehavior : DisposingBehavior<Control>
{
    protected override IDisposable OnAttachedOverride()
    {
        // Subscribe to an event
        AssociatedObject.PointerPressed += OnPointerPressed;

        // Return a disposable that unsubscribes
        return DisposableAction.Create(() => 
        {
            AssociatedObject.PointerPressed -= OnPointerPressed;
        });
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Handle event
    }
}
```

This pattern is even more powerful when combined with Reactive Extensions (Rx), as `IObservable.Subscribe` returns an `IDisposable`.

```csharp
public class MyRxBehavior : DisposingBehavior<Control>
{
    protected override IDisposable OnAttachedOverride()
    {
        return AssociatedObject.GetObservable(Control.IsVisibleProperty)
            .Subscribe(isVisible => 
            {
                // React to changes
            });
    }
}
```

## DisposingTrigger&lt;T&gt;

`DisposingTrigger<T>` follows the same pattern as `DisposingBehavior<T>` but for triggers. It simplifies the creation of custom triggers that need to listen to events or observables.

### Example

```csharp
public class MyTrigger : DisposingTrigger<Control>
{
    protected override IDisposable OnAttachedOverride()
    {
        // Subscribe to an event and invoke actions when it fires
        AssociatedObject.KeyDown += OnKeyDown;

        return DisposableAction.Create(() => AssociatedObject.KeyDown -= OnKeyDown);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // Invoke the actions associated with this trigger
            Interaction.ExecuteActions(AssociatedObject, Actions, e);
        }
    }
}
```

## Benefits

*   **Reduced Boilerplate**: You don't need to store the subscription token or event handler in a private field.
*   **Safety**: The base class ensures that the resource is disposed of correctly when the behavior is detached, reducing the risk of memory leaks.
*   **Clarity**: The code clearly separates the setup logic (in `OnAttachedOverride`) from the cleanup logic (in the returned `IDisposable`), often keeping them close together.
