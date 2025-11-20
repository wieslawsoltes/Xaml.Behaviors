# ToolTipOpeningTrigger

Trigger that listens for the `ToolTip.ToolTipOpeningEvent`.

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
