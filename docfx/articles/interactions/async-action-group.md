# AsyncActionGroup

The `AsyncActionGroup` is a container action that manages the asynchronous execution flow of its child actions. It allows you to execute actions either sequentially (awaiting each one) or in parallel.

This is particularly useful when combined with `DelayAction` or other actions that return a `Task`, enabling complex workflows directly in XAML (e.g., Animation -> Wait -> Data Load -> Animation).

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Mode` | `AsyncActionMode` | The execution mode: `Sequence` or `Parallel`. |
| `Actions` | `ActionCollection` | The collection of actions to execute. |

### AsyncActionMode

-   **Sequence**: Actions are executed one by one. If an action returns a `Task`, the group awaits it before starting the next action.
-   **Parallel**: All actions are started immediately. The group awaits all of them to complete (if awaited itself).

## DelayAction

A helper action that simply waits for a specified duration. It returns a `Task` that completes after the delay.

| Property | Type | Description |
| --- | --- | --- |
| `Duration` | `TimeSpan` | The time to wait. |

## Example

### Sequential Flow

This example updates a text block, waits for 1 second, updates it again, waits another second, and then finishes.

```xml
<Button Content="Start Sequence">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <AsyncActionGroup Mode="Sequence">
                <ChangePropertyAction TargetObject="{Binding #StatusText}" PropertyName="Text" Value="Step 1" />
                <DelayAction Duration="0:0:1" />
                <ChangePropertyAction TargetObject="{Binding #StatusText}" PropertyName="Text" Value="Step 2" />
                <DelayAction Duration="0:0:1" />
                <ChangePropertyAction TargetObject="{Binding #StatusText}" PropertyName="Text" Value="Finished" />
            </AsyncActionGroup>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
<TextBlock Name="StatusText" Text="Ready" />
```
