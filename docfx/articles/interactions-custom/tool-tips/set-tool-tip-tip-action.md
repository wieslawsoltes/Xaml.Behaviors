# SetToolTipTipAction

Sets the `ToolTip.TipProperty` of the associated or target control when executed.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TargetControl | `Control` | Gets or sets the control whose tooltip will be updated. If not set, the sender is used. |
| Tip | `object` | Gets or sets the new tooltip content. |

## Usage

```xml
<Button Content="Change ToolTip">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SetToolTipTipAction TargetControl="{Binding #MyButton}" Tip="New ToolTip Content" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<Button x:Name="MyButton" Content="Hover Me" ToolTip.Tip="Original ToolTip" />
```
