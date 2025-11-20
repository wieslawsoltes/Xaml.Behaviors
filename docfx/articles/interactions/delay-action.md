# DelayAction

The `DelayAction` is an action that waits for a specified duration. It returns a `Task` that completes after the delay.

This action is primarily designed to be used within an `AsyncActionGroup` to create pauses in a sequence of actions.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Duration` | `TimeSpan` | The amount of time to wait. |

## Example

```xml
<AsyncActionGroup Mode="Sequence">
    <ChangePropertyAction TargetObject="{Binding #StatusText}" PropertyName="Text" Value="Step 1" />
    <DelayAction Duration="0:0:1" />
    <ChangePropertyAction TargetObject="{Binding #StatusText}" PropertyName="Text" Value="Step 2" />
</AsyncActionGroup>
```
