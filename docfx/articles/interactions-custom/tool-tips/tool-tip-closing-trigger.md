# ToolTipClosingTrigger

Trigger that listens for the `ToolTip.ToolTipClosingEvent`.

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
