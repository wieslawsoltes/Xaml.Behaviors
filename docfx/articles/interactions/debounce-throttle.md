# Debounce and Throttle Actions

The `DebounceAction` and `ThrottleAction` are wrapper actions that control the execution frequency of their child actions. They are useful for handling rapid user input or events that fire too frequently.

## DebounceAction

The `DebounceAction` executes its child actions only after a specified delay has passed without the action being invoked again. If the action is invoked again before the delay elapses, the timer is reset.

This is ideal for scenarios like:
- Search boxes (wait until user stops typing).
- Window resizing (wait until resize is finished).

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `Delay` | `TimeSpan` | The wait time before executing the actions. |
| `Actions` | `ActionCollection` | The collection of actions to execute. |

### Example

```xml
<TextBox Name="SearchBox" Watermark="Type something...">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="TextChanged">
            <DebounceAction Delay="0:0:0.5">
                <InvokeCommandAction Command="{Binding SearchCommand}" CommandParameter="{Binding #SearchBox.Text}" />
            </DebounceAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</TextBox>
```

## ThrottleAction

The `ThrottleAction` executes its child actions immediately, but ignores subsequent invocations until a specified interval has passed.

This is ideal for scenarios like:
- Preventing double-clicks on buttons.
- Limiting the rate of expensive updates (e.g., layout calculations) during continuous events like scrolling or mouse movement.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `Interval` | `TimeSpan` | The minimum time between executions. |
| `Actions` | `ActionCollection` | The collection of actions to execute. |

### Example

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ThrottleAction Interval="0:0:1">
                <CallMethodAction TargetObject="{Binding}" MethodName="ProcessClick" />
            </ThrottleAction>
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
