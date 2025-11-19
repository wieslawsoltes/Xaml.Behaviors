# InteractiveBehaviorBase

`InteractiveBehaviorBase` is a base class for behaviors that attach to `Interactive` objects (objects that support routed events) and need to specify routing strategies.

## Properties

*   **`RoutingStrategies`**: Defines the routing strategy for the event subscription (e.g., `Bubble`, `Tunnel`, `Direct`). The default is `Bubble`.

## Usage

This class is useful when you are creating a behavior that listens to routed events and you want to give the user control over which phase of the event route the behavior should respond to.

```csharp
public class MyRoutedEventBehavior : InteractiveBehaviorBase
{
    protected override void OnAttached()
    {
        base.OnAttached();
        // Use RoutingStrategies property when subscribing
        AssociatedObject.AddHandler(Button.ClickEvent, OnClick, RoutingStrategies);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.RemoveHandler(Button.ClickEvent, OnClick);
    }

    private void OnClick(object? sender, RoutedEventArgs e)
    {
        // ...
    }
}
```
