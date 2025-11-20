# ShowBehaviorBase

Base class for behaviors that show a control in response to an event.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TargetControl | `Control` | Gets or sets the target control to show. |
| EventRoutingStrategy | `RoutingStrategies` | Gets or sets the routing strategy used for the triggering event. Default is `Bubble`. |

## Methods

| Method | Description |
| --- | --- |
| `Show()` | Shows the `TargetControl` when the behavior is triggered. Returns true if the control was shown. |
