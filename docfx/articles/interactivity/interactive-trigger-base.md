# InteractiveTriggerBase

`InteractiveTriggerBase` is a base class for triggers that listen for routed events on `Interactive` objects.

## Properties

*   **`RoutingStrategies`**: Defines the routing strategy for the event subscription (e.g., `Bubble`, `Tunnel`, `Direct`). The default is `Bubble`.

## Usage

Use this base class when you want to create a trigger that listens for routed events and needs to support different routing strategies.

```csharp
public class MyRoutedEventTrigger : InteractiveTriggerBase
{
    // ...
}
```
