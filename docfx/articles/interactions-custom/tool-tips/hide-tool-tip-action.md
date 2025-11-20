# HideToolTipAction

Hides the `ToolTip` of the associated or target control when executed.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TargetControl | `Control` | Gets or sets the control whose tooltip will be hidden. If not set, the sender is used. |

## Usage

```xml
<Button Content="Hide ToolTip">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <HideToolTipAction TargetControl="{Binding #MyButton}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<Button x:Name="MyButton" Content="Hover Me" ToolTip.Tip="I am a tooltip!" />
```
