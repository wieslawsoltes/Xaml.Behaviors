# ToolTipOpeningTrigger

Trigger that listens for the `ToolTip.ToolTipOpeningEvent`.

`ToolTipOpeningEvent` uses direct routing. The trigger subscribes with the matching routing strategy and passes the `CancelRoutedEventArgs` instance to its actions.

## Usage

```xml
<Button Content="Hover Me" ToolTip.Tip="I am a tooltip!">
    <Interaction.Behaviors>
        <ToolTipOpeningTrigger>
            <InvokeCommandAction Command="{Binding ToolTipOpeningCommand}" />
        </ToolTipOpeningTrigger>
    </Interaction.Behaviors>
</Button>
```

The same event can be handled by `EventTriggerBehavior` with `EventName="ToolTipOpening"`. Prefer `ToolTipOpeningTrigger` when trimming or native AOT compatibility is required.
