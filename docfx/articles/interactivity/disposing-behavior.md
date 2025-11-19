# DisposingBehavior&lt;T&gt;

`DisposingBehavior<T>` is a base class for behaviors that need to manage a single disposable resource (like an event subscription or an observable subscription) during their attachment lifecycle.

## Overview

Instead of overriding `OnAttached` and `OnDetaching` to manually subscribe and unsubscribe, you override `OnAttachedOverride` which returns an `IDisposable`. The base class automatically disposes of this resource when the behavior is detached.

## Usage

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

See [Disposing Behaviors](disposing-behaviors.md) for more details.
