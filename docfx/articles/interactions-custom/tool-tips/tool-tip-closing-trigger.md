# ToolTipClosingTrigger

Trigger that listens for the `ToolTip.ToolTipClosingEvent`.

`ToolTipClosingEvent` uses direct routing. The trigger subscribes with the matching routing strategy and passes the `RoutedEventArgs` instance to its actions.

## Usage

```xml
<Button Content="Hover Me" ToolTip.Tip="I am a tooltip!">
    <Interaction.Behaviors>
        <ToolTipClosingTrigger>
            <InvokeCommandAction Command="{Binding ToolTipClosingCommand}" />
        </ToolTipClosingTrigger>
    </Interaction.Behaviors>
</Button>
```

The same event can be handled by `EventTriggerBehavior` with `EventName="ToolTipClosing"`. Prefer `ToolTipClosingTrigger` when trimming or native AOT compatibility is required.
