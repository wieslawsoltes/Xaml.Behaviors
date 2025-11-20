# RoutedEventTriggerBase

`RoutedEventTriggerBase` is an abstract base class for triggers that listen for a specific `RoutedEvent` on a source object. It provides properties for configuring the event routing strategy and whether to mark the event as handled.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `EventRoutingStrategy` | `RoutingStrategies` | Gets or sets the routing strategy for the event (e.g., Bubble, Tunnel, Direct). Default is `Direct`. |
| `MarkAsHandled` | `bool` | Gets or sets a value indicating whether to mark the routed event as handled after the trigger executes. |
